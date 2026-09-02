// Pure chip-allocation solver logic - no DOM dependency, so this can be loaded and tested
// standalone (see verify-solver.js). Mirrors DoenaSoft.PokerChips.ChipCalculator (C#).
class Chip
{
    constructor(amount, value)
    {
        this._amount = parseInt(amount);
        this._value = parseInt(value);
    }

    getAmount()
    {
        return this._amount;
    }

    getValue()
    {
        return this._value;
    }
}

// Determines the maximum number of chips of a denomination that may be handed out per player,
// bounded both by how many are available per player and by the configured maximum chip count.
function getMaxAmount(caseChipAmount, amountPlayers, maxChipsPerValue)
{
    let chipAmount = Math.floor(caseChipAmount / amountPlayers);

    if(chipAmount > maxChipsPerValue)
    {
        chipAmount = maxChipsPerValue;
    }

    return chipAmount;
}

// Finds the largest sum (at most targetValue) that is achievable using all gathered
// denominations, preferring an exact match to targetValue when possible.
function findBestAchievableSum(isSumAchievable, denominationCount, targetValue)
{
    for(let candidateSum = targetValue; candidateSum >= 0; candidateSum--)
    {
        if(isSumAchievable[denominationCount][candidateSum] === true)
        {
            return candidateSum;
        }
    }

    return -1;
}

// Solves for the combination of chip counts (one count per case chip denomination) that sums up
// as closely as possible to targetValue, without exceeding it, while respecting each denomination's
// cap (see getMaxAmount). This is a classic "bounded knapsack" problem: each denomination may be
// used between 0 and its cap number of times, and we want the achievable sum closest to the target.
function solveChipAllocation(caseChips, targetValue, amountPlayers, maxChipsPerValue)
{
    let denominationValues = caseChips.map(caseChip => caseChip.getValue());
    let denominationCaps = caseChips.map(caseChip => getMaxAmount(caseChip.getAmount(), amountPlayers, maxChipsPerValue));

    let denominationOrder = getDenominationOrderDescendingByValue(denominationValues);

    let table = buildBoundedKnapsackTable(denominationOrder, denominationValues, denominationCaps, targetValue);

    let bestAchievableSum = findBestAchievableSum(table.isSumAchievable, denominationOrder.length, targetValue);

    if(bestAchievableSum < 0)
    {
        // No combination of chips (not even zero of everything) matched, which should not normally
        // happen since a sum of 0 is always achievable.
        return { chipCounts: new Array(denominationValues.length).fill(0), achievedValue: 0, denominationOrder: denominationOrder };
    }

    let chipCounts = applyChipCounts(denominationOrder, denominationValues, table.chipCountUsedForSum, bestAchievableSum);

    return { chipCounts: chipCounts, achievedValue: bestAchievableSum, denominationOrder: denominationOrder };
}

// Builds an index permutation over the denominations, ordered from highest to lowest value. The
// chip-count-maximizing tie-break in findMaxUsableChipCount only favors fewer, higher-value chips
// overall if the highest-value denominations are considered first, so this is independent of the
// order the case chips were originally passed in.
function getDenominationOrderDescendingByValue(denominationValues)
{
    let denominationOrder = [];

    for(let index = 0; index < denominationValues.length; index++)
    {
        denominationOrder.push(index);
    }

    denominationOrder.sort((left, right) => denominationValues[right] - denominationValues[left]);

    return denominationOrder;
}

// Runs the bounded-knapsack dynamic-programming pass over the denominations (in the given order),
// producing two tables:
// - isSumAchievable[denominationIndex][sum]: whether "sum" can be built exactly using only the
//   first "denominationIndex" denominations (each within its own cap).
// - chipCountUsedForSum[denominationIndex][sum]: how many chips of the denomination at
//   (denominationIndex - 1) were used to achieve "sum", so the choice can be reconstructed
//   afterwards by applyChipCounts.
function buildBoundedKnapsackTable(denominationOrder, denominationValues, denominationCaps, targetValue)
{
    let denominationCount = denominationOrder.length;

    let isSumAchievable = [];
    let chipCountUsedForSum = [];

    for(let index = 0; index <= denominationCount; index++)
    {
        isSumAchievable.push(new Array(targetValue + 1).fill(false));
        chipCountUsedForSum.push(new Array(targetValue + 1).fill(0));
    }

    // Base case: a sum of 0 is always achievable using zero denominations.
    isSumAchievable[0][0] = true;

    for(let denominationIndex = 1; denominationIndex <= denominationCount; denominationIndex++)
    {
        let originalIndex = denominationOrder[denominationIndex - 1];
        let chipValue = denominationValues[originalIndex];
        let chipCountCap = denominationCaps[originalIndex];

        fillAchievableSumsForDenomination(isSumAchievable, chipCountUsedForSum, denominationIndex, chipValue, chipCountCap, targetValue);
    }

    return { isSumAchievable: isSumAchievable, chipCountUsedForSum: chipCountUsedForSum };
}

// Fills in isSumAchievable/chipCountUsedForSum at row "denominationIndex" for every candidate sum
// from 0 to targetValue, given the denomination's value and chip-count cap.
function fillAchievableSumsForDenomination(isSumAchievable, chipCountUsedForSum, denominationIndex, chipValue, chipCountCap, targetValue)
{
    for(let candidateSum = 0; candidateSum <= targetValue; candidateSum++)
    {
        let chipCount = findMaxUsableChipCount(isSumAchievable, denominationIndex, chipValue, chipCountCap, candidateSum);

        if(chipCount < 0)
        {
            continue;
        }

        isSumAchievable[denominationIndex][candidateSum] = true;
        chipCountUsedForSum[denominationIndex][candidateSum] = chipCount;
    }
}

// Finds the largest chip count (at most chipCountCap) of the current denomination that still
// allows candidateSum to be reached, given what the earlier (already processed) denominations can
// achieve. Trying the largest count first favors fewer, higher-value chips overall (since
// higher-value denominations are considered first, in descending order). Returns -1 if no count works.
function findMaxUsableChipCount(isSumAchievable, denominationIndex, chipValue, chipCountCap, candidateSum)
{
    for(let chipCount = chipCountCap; chipCount >= 0; chipCount--)
    {
        let valueFromThisDenomination = chipCount * chipValue;

        if(valueFromThisDenomination > candidateSum)
        {
            continue;
        }

        let remainingSumForEarlierDenominations = candidateSum - valueFromThisDenomination;

        if(isSumAchievable[denominationIndex - 1][remainingSumForEarlierDenominations] === true)
        {
            return chipCount;
        }
    }

    return -1;
}

// Walks the denominations backwards (i.e. from lowest to highest value, the reverse of the
// descending order used to build the table), reading off how many chips of each were used to
// reach bestAchievableSum, and records them against their original denomination index.
function applyChipCounts(denominationOrder, denominationValues, chipCountUsedForSum, bestAchievableSum)
{
    let chipCounts = new Array(denominationOrder.length).fill(0);
    let valueLeftToAllocate = bestAchievableSum;

    for(let denominationIndex = denominationOrder.length; denominationIndex >= 1; denominationIndex--)
    {
        let chipCount = chipCountUsedForSum[denominationIndex][valueLeftToAllocate];
        let originalIndex = denominationOrder[denominationIndex - 1];
        let chipValue = denominationValues[originalIndex];

        chipCounts[originalIndex] = chipCount;

        valueLeftToAllocate -= (chipCount * chipValue);
    }

    return chipCounts;
}

// Adds the resolved player chips in the same order the C# solver (ChipCalculator.Solve) emits
// them: walking the descending-by-value denominationOrder backwards, i.e. lowest value first.
function addPlayerChips(caseChips, playerChips, stackSize, amountPlayers, maxChipsPerValue)
{
    let solution = solveChipAllocation(caseChips, stackSize, amountPlayers, maxChipsPerValue);

    for(let denominationIndex = solution.denominationOrder.length - 1; denominationIndex >= 0; denominationIndex--)
    {
        let originalIndex = solution.denominationOrder[denominationIndex];
        let chipCount = solution.chipCounts[originalIndex];

        if(chipCount > 0)
        {
            playerChips.push(new Chip(chipCount, caseChips[originalIndex].getValue()));
        }
    }

    let remainingValue = stackSize - solution.achievedValue;

    return remainingValue;
}
