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
// Chips with a value of 0 (used by the UI for unused case-chip color slots) can never contribute to
// the target sum, so they are filtered out here instead of being fed into the solver.
function solveChipAllocation(caseChips, targetValue, amountPlayers, maxChipsPerValue)
{
    // A chip with an amount greater than 0 but a value of 0 cannot be a legitimate case chip
    // (only the UI's unused, empty color slots are expected to have a value of 0, and those
    // always have an amount of 0 too), so reject it instead of silently ignoring it.
    caseChips.forEach(caseChip =>
    {
        if(caseChip.getValue() === 0 && caseChip.getAmount() > 0)
        {
            throw new Error("A chip with an amount greater than 0 must not have a value of 0.");
        }
    });

    let sortedCaseChips = caseChips.filter(caseChip => caseChip.getValue() !== 0);

    // Sort ascending by value. The chip-count-minimizing tie-break in findMinUsableChipCount only
    // favors fewer, higher-value chips being needed (i.e. prefers using smaller chips) if the
    // lowest-value denominations are considered first, so this is independent of the order the
    // case chips were originally fed in (e.g. the caller may feed them sorted ascending by value
    // already, but that is not relied upon here).
    sortedCaseChips.sort((left, right) => left.getValue() - right.getValue());

    let denominationValues = sortedCaseChips.map(caseChip => caseChip.getValue());
    let denominationCaps = sortedCaseChips.map(caseChip => getMaxAmount(caseChip.getAmount(), amountPlayers, maxChipsPerValue));

    // Every achievable sum is necessarily a multiple of the GCD of all denomination values, so
    // dividing the values (and the target) by that GCD shrinks the DP table without changing which
    // sums are achievable, just their scale.
    // Note that this is a multiple of the GCD of ALL denominations combined, not of any single
    // denomination's value. E.g. with denominations 25/100/200/500/1000 (GCD 25), a sum like
    // 4975 (scaled: 199) is a multiple of 25 but not of 100/200/500/1000 individually - it can
    // still be achievable via a mix of denominations (mostly 25s). Being a multiple of the GCD
    // is a NECESSARY condition for achievability, not a SUFFICIENT one, so isSumAchievable still
    // has to be checked per-cell; not every multiple of the GCD is actually reachable (e.g. if
    // the caps are too restrictive), but everything reachable will be a multiple of it.
    let denominationGcd = getDenominationGcd(denominationValues);

    let scaledTargetValue = Math.floor(targetValue / denominationGcd);

    let scaledValues = denominationValues.map(value => value / denominationGcd);

    let table = buildBoundedKnapsackTable(scaledValues, denominationCaps, scaledTargetValue);

    let bestAchievableScaledSum = findBestAchievableSum(table.isSumAchievable, scaledValues.length, scaledTargetValue);

    if(bestAchievableScaledSum < 0)
    {
        // No combination of chips (not even zero of everything) matched, which should not normally
        // happen since a sum of 0 is always achievable.
        return { chipCounts: new Array(denominationValues.length).fill(0), achievedValue: 0, sortedCaseChips: sortedCaseChips };
    }

    let chipCounts = applyChipCounts(denominationValues, scaledValues, table.chipCountUsedForSum, bestAchievableScaledSum);

    let achievedValue = bestAchievableScaledSum * denominationGcd;

    return { chipCounts: chipCounts, achievedValue: achievedValue, sortedCaseChips: sortedCaseChips };
}

// Returns the greatest common divisor shared by all denomination values (or 1 if there are none),
// used to shrink the DP table's column count.
function getDenominationGcd(denominationValues)
{
    if(denominationValues.length === 0)
    {
        return 1;
    }

    let result = denominationValues[0];

    for(let index = 1; index < denominationValues.length; index++)
    {
        result = gcd(result, denominationValues[index]);
    }

    return result;
}

// Returns the greatest common divisor (GCD) of two numbers, i.e. the largest number that divides
// both left and right without a remainder, computed via the Euclidean algorithm.
function gcd(left, right)
{
    while(right !== 0)
    {
        let remainder = left % right;

        left = right;
        right = remainder;
    }

    return left;
}

// Runs the bounded-knapsack dynamic-programming pass over the denominations (sorted ascending by
// value), producing two tables:
// - isSumAchievable[denominationIndex][sum]: whether "sum" can be built exactly using only the
//   first "denominationIndex" denominations (each within its own cap).
// - chipCountUsedForSum[denominationIndex][sum]: how many chips of the denomination at
//   (denominationIndex - 1) were used to achieve "sum", so the choice can be reconstructed
//   afterwards by applyChipCounts.
// A chipCountUsedForSum[denominationIndex][sum] entry is only meaningful when
// isSumAchievable[denominationIndex][sum] is true; both arrays are allocated as full rectangular
// arrays, so unreachable cells simply retain their default value (false/0) rather than encoding
// "0 chips achieves this sum".
function buildBoundedKnapsackTable(denominationValues, denominationCaps, targetValue)
{
    let denominationCount = denominationValues.length;

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
        let chipValue = denominationValues[denominationIndex - 1];
        let chipCountCap = denominationCaps[denominationIndex - 1];

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
        let chipCount = findMinUsableChipCount(isSumAchievable, denominationIndex, chipValue, chipCountCap, candidateSum);

        if(chipCount < 0)
        {
            continue;
        }

        isSumAchievable[denominationIndex][candidateSum] = true;
        chipCountUsedForSum[denominationIndex][candidateSum] = chipCount;
    }
}

// Finds the smallest chip count (at most chipCountCap) of the current denomination that still
// allows candidateSum to be reached, given what the earlier (already processed) denominations can
// achieve. Trying the smallest count first favors using fewer of this (lower-value) denomination
// and relying on higher-value denominations for the rest (since denominations are considered
// lowest-value first, in ascending order). Returns -1 if no count works.
function findMinUsableChipCount(isSumAchievable, denominationIndex, chipValue, chipCountCap, candidateSum)
{
    for(let chipCount = 0; chipCount <= chipCountCap; chipCount++)
    {
        let valueFromThisDenomination = chipCount * chipValue;

        if(valueFromThisDenomination > candidateSum)
        {
            break;
        }

        let remainingSumForEarlierDenominations = candidateSum - valueFromThisDenomination;

        if(isSumAchievable[denominationIndex - 1][remainingSumForEarlierDenominations] === true)
        {
            return chipCount;
        }
    }

    return -1;
}

// Walks the denominations backwards (from the last-processed to the first-processed, as required
// by the DP reconstruction), reading off how many chips of each were used to reach
// bestAchievableScaledSum, and records them in ascending-by-value order (matching denominationValues).
function applyChipCounts(denominationValues, scaledValues, chipCountUsedForSum, bestAchievableScaledSum)
{
    let chipCounts = new Array(denominationValues.length).fill(0);
    let valueLeftToAllocate = bestAchievableScaledSum;

    for(let denominationIndex = denominationValues.length; denominationIndex >= 1; denominationIndex--)
    {
        let chipCount = chipCountUsedForSum[denominationIndex][valueLeftToAllocate];
        let scaledChipValue = scaledValues[denominationIndex - 1];

        chipCounts[denominationIndex - 1] = chipCount;

        valueLeftToAllocate -= (chipCount * scaledChipValue);
    }

    return chipCounts;
}

// Adds the resolved player chips in the same order the C# solver (ChipCalculator.Solve) emits
// them: walking the ascending-by-value sortedCaseChips forward, i.e. lowest value first.
function addPlayerChips(caseChips, playerChips, stackSize, amountPlayers, maxChipsPerValue)
{
    let solution = solveChipAllocation(caseChips, stackSize, amountPlayers, maxChipsPerValue);

    for(let index = 0; index < solution.sortedCaseChips.length; index++)
    {
        let chipCount = solution.chipCounts[index];

        if(chipCount > 0)
        {
            playerChips.push(new Chip(chipCount, solution.sortedCaseChips[index].getValue()));
        }
    }

    let remainingValue = stackSize - solution.achievedValue;

    return remainingValue;
}
