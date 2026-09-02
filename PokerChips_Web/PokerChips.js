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

const PokerTable = "pokerTable";
const PlayerChipRowPrefix = "playerChipRow_";
const CaseChipAmountPrefix = "caseChipAmount_";
const CaseChipValuePrefix = "caseChipValue_";

const NumberOfCaseChipColors = 5;

const AmountOption0 = 0;
const AmountOption50 = 50;
const AmountOption100 = 100;
const AmountOption150 = 150;
const AmountOption200 = 200;
const AmountOption250 = 250;
const AmountOption300 = 300;
const AmountOption500 = 500;
const AmountOption1000 = 1000;

const ValueOption0 = 0;
const ValueOption10 = 10;
const ValueOption25 = 25;
const ValueOption50 = 50;
const ValueOption100 = 100;
const ValueOption200 = 200;
const ValueOption250 = 250;
const ValueOption500 = 500;
const ValueOption1000 = 1000;
const ValueOption2000 = 2000;
const ValueOption2500 = 2500;
const ValueOption5000 = 5000;
const ValueOption10000 = 10000;
const ValueOption20000 = 20000;
const ValueOption25000 = 25000;
const ValueOption50000 = 50000;
const ValueOption100000 = 100000;

function getById(id)
{
    let node = document.getElementById(id);

    return node;
}

function setId(node, id)
{
    node.setAttribute("id", id);
}

function addSelectOption(selectNode, value)
{
    let optionNode = document.createElement("OPTION");
    optionNode.innerText = value;
    selectNode.add(optionNode);
}

function addAmountOptions(selectNode)
{
    addSelectOption(selectNode, AmountOption0);
    addSelectOption(selectNode, AmountOption50);
    addSelectOption(selectNode, AmountOption100);
    addSelectOption(selectNode, AmountOption150);
    addSelectOption(selectNode, AmountOption200);
    addSelectOption(selectNode, AmountOption250);
    addSelectOption(selectNode, AmountOption300);
    addSelectOption(selectNode, AmountOption500);
    addSelectOption(selectNode, AmountOption1000);
}

function addValueOptions(selectNode)
{
    addSelectOption(selectNode, ValueOption0);
    addSelectOption(selectNode, ValueOption10);
    addSelectOption(selectNode, ValueOption25);
    addSelectOption(selectNode, ValueOption50);
    addSelectOption(selectNode, ValueOption100);
    addSelectOption(selectNode, ValueOption200);
    addSelectOption(selectNode, ValueOption250);
    addSelectOption(selectNode, ValueOption500);
    addSelectOption(selectNode, ValueOption1000);
    addSelectOption(selectNode, ValueOption2000);
    addSelectOption(selectNode, ValueOption2500);
    addSelectOption(selectNode, ValueOption5000);
    addSelectOption(selectNode, ValueOption10000);
    addSelectOption(selectNode, ValueOption20000);
    addSelectOption(selectNode, ValueOption25000);
    addSelectOption(selectNode, ValueOption50000);
    addSelectOption(selectNode, ValueOption100000);
}

function addSelectCell(rowNode, id)
{
    let cellNode = rowNode.insertCell(-1);
    cellNode.setAttribute("style", "text-align: center;");

    let selectNode = document.createElement("SELECT");
    setId(selectNode, id);

    cellNode.appendChild(selectNode);

    return selectNode;
}

function addCaseRow(caseChipIndex)
{
    let rowNode = getById(PokerTable).insertRow(-1);

    let amountSelectNode = addSelectCell(rowNode, CaseChipAmountPrefix + caseChipIndex);
    addAmountOptions(amountSelectNode);

    let valueSelectNode = addSelectCell(rowNode, CaseChipValuePrefix + caseChipIndex);
    addValueOptions(valueSelectNode);
}

function setCaseDefaults(caseChipIndex, amount, value)
{
    if(caseChipIndex >= NumberOfCaseChipColors)
    {
        return;
    }

    getById(CaseChipAmountPrefix + caseChipIndex).value = amount;
    getById(CaseChipValuePrefix + caseChipIndex).value = value;
}

function setOneNormal500Case()
{
    setCaseDefaults(0, AmountOption150, ValueOption25);
    setCaseDefaults(1, AmountOption150, ValueOption100);
    setCaseDefaults(2, AmountOption100, ValueOption200);
    setCaseDefaults(3, AmountOption0, ValueOption0);
    setCaseDefaults(4, AmountOption0, ValueOption0);
}

function setOneFull500Case()
{
    setCaseDefaults(0, AmountOption150, ValueOption25);
    setCaseDefaults(1, AmountOption150, ValueOption100);
    setCaseDefaults(2, AmountOption100, ValueOption200);
    setCaseDefaults(3, AmountOption50, ValueOption500);
    setCaseDefaults(4, AmountOption50, ValueOption1000);
}

function setTwo500Case()
{
    setCaseDefaults(0, AmountOption300, ValueOption25);
    setCaseDefaults(1, AmountOption300, ValueOption100);
    setCaseDefaults(2, AmountOption200, ValueOption200);
    setCaseDefaults(3, AmountOption100, ValueOption500);
    setCaseDefaults(4, AmountOption100, ValueOption1000);
}

function initPlayerChipOutput(playerChipIndex)
{
    let rowNode = getById(PlayerChipRowPrefix + playerChipIndex);

    if(rowNode !== null)
    {
        getById(PokerTable).deleteRow(rowNode.rowIndex);
    }
}

function initPlayerChipsOutput()
{
    initPlayerChipOutput("H");

    for(let playerChipIndex = 0; playerChipIndex < NumberOfCaseChipColors; playerChipIndex++)
    {
        initPlayerChipOutput(playerChipIndex);
    }
}

function createCaseChip(caseChipIndex)
{
    let amount = parseInt(getById(CaseChipAmountPrefix + caseChipIndex).value);
    let value = parseInt(getById(CaseChipValuePrefix + caseChipIndex).value);
    
    return new Chip(amount, value);
}

function sortCaseChips(caseChips)
{
    let resorted = false;

    //good ol'fashioned bubble sort
    do
    {
        resorted = false;

        for(let caseChipIndex = 0; caseChipIndex < caseChips.length - 2; caseChipIndex++)
        {
            if(caseChips[caseChipIndex].getValue() > caseChips[caseChipIndex + 1].getValue())
            {
                let temp = caseChips[caseChipIndex];
                caseChips[caseChipIndex] = caseChips[caseChipIndex + 1];
                caseChips[caseChipIndex + 1] = temp;

                resorted = true;
            }
        }
    }while (resorted === true);
}

function createCaseChips()
{
    let caseChips = [];

    for(let caseChipIndex = 0; caseChipIndex < NumberOfCaseChipColors; caseChipIndex++)
    {
        caseChips.push(createCaseChip(caseChipIndex));
    }

    sortCaseChips(caseChips);

    return caseChips;
}

function getMaxAmount(caseChipAmount)
{
    let amountPlayers = parseInt(getById("amountPlayers").value);
    let chipAmount = Math.floor(caseChipAmount / amountPlayers);
    let maxChips = parseInt(getById("maxChips").value);

    if(chipAmount > maxChips)
    {
        chipAmount = maxChips;
    }

    return chipAmount;
}

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
function solveChipAllocation(caseChips, targetValue)
{
    let denominationCount = caseChips.length;
    let denominationValues = caseChips.map(caseChip => caseChip.getValue());
    let denominationCaps = caseChips.map(caseChip => getMaxAmount(caseChip.getAmount()));

    // The chip-count-maximizing tie-break below only favors fewer, higher-value chips overall if
    // the highest-value denominations are considered first. Build a descending-by-value index
    // order here, independent of the order the case chips were passed in.
    let denominationOrder = [];

    for(let index = 0; index < denominationCount; index++)
    {
        denominationOrder.push(index);
    }

    denominationOrder.sort((left, right) => denominationValues[right] - denominationValues[left]);

    // isSumAchievable[denominationIndex][sum] is true if "sum" can be built exactly using only the
    // first "denominationIndex" denominations (in descending-value order), each within its own cap.
    let isSumAchievable = [];

    // chipCountUsedForSum[denominationIndex][sum] stores how many chips of the denomination at
    // (denominationIndex - 1) were used to achieve "sum", so the choice can be reconstructed afterwards.
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

        for(let candidateSum = 0; candidateSum <= targetValue; candidateSum++)
        {
            // Try using as many chips of this denomination as possible first, so that when a valid
            // combination is found it favors fewer, higher-value chips overall (since higher-value
            // denominations are considered first, in descending order).
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
                    isSumAchievable[denominationIndex][candidateSum] = true;
                    chipCountUsedForSum[denominationIndex][candidateSum] = chipCount;

                    break;
                }
            }
        }
    }

    let bestAchievableSum = findBestAchievableSum(isSumAchievable, denominationCount, targetValue);
    let chipCounts = new Array(denominationCount).fill(0);

    if(bestAchievableSum < 0)
    {
        // No combination of chips (not even zero of everything) matched, which should not normally
        // happen since a sum of 0 is always achievable.
        return { chipCounts: chipCounts, achievedValue: 0 };
    }

    // Walk the denominations backwards (i.e. from lowest to highest value, the reverse of the
    // descending order used above), reading off how many chips of each were used to reach
    // bestAchievableSum, and record them against their original denomination index.
    let valueLeftToAllocate = bestAchievableSum;

    for(let denominationIndex = denominationCount; denominationIndex >= 1; denominationIndex--)
    {
        let chipCount = chipCountUsedForSum[denominationIndex][valueLeftToAllocate];
        let originalIndex = denominationOrder[denominationIndex - 1];
        let chipValue = denominationValues[originalIndex];

        chipCounts[originalIndex] = chipCount;

        valueLeftToAllocate -= (chipCount * chipValue);
    }

    return { chipCounts: chipCounts, achievedValue: bestAchievableSum, denominationOrder: denominationOrder };
}

// Adds the resolved player chips in the same order the C# solver (ChipCalculator.Solve) emits
// them: walking the descending-by-value denominationOrder backwards, i.e. lowest value first.
function addPlayerChips(caseChips, playerChips, stackSize)
{
    let solution = solveChipAllocation(caseChips, stackSize);

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

function addPlayerChipOutput(playerChipIndex, playerChip)
{
    let rowNode = getById(PokerTable).insertRow(-1);
    setId(rowNode, PlayerChipRowPrefix + playerChipIndex);

    let amountCellNode = rowNode.insertCell(-1);
    amountCellNode.setAttribute("style", "text-align: center;");
    amountCellNode.innerText = playerChip.getAmount();

    let valueCellNode = rowNode.insertCell(-1);
    valueCellNode.setAttribute("style", "text-align: center;");
    valueCellNode.innerText = playerChip.getValue();
}

function addPlayerChipsOutput(playerChips)
{
    for(let playerChipIndex = 0; playerChipIndex < playerChips.length; playerChipIndex++)
    {
        addPlayerChipOutput(playerChipIndex, playerChips[playerChipIndex]);
    }
}

function calulate(isGerman)
{
    initPlayerChipsOutput();
    let caseChips = createCaseChips();    
    let playerChips = [];
    let stackSize = parseInt(getById("stackSize").value);
    let remainingValue = addPlayerChips(caseChips, playerChips, stackSize);

    if(remainingValue !== 0)
    {
        if(isGerman === true)
        {
            alert("Die Anzahl der Chips mal den Wert der Chips ist nicht ausreichend für die Anzahl der Spieler!");
        }
        else
        {
            alert("Number of chips times value of chips is insufficient for the number of players!");
        }

        return;
    }

    let rowNode = getById(PokerTable).insertRow(-1);
    setId(rowNode, PlayerChipRowPrefix + "H");

    let amountCellNode = rowNode.insertCell(-1);
    amountCellNode.setAttribute("style", "text-align: center; font-weight: bold;");

    let valueCellNode = rowNode.insertCell(-1);
    valueCellNode.setAttribute("style", "text-align: center; font-weight: bold;");

    if(isGerman === true)
    {
        amountCellNode.innerText = "Anzahl Chips je Spieler:";
        valueCellNode.innerText = "Chip Wert:";
    }
    else
    {
        amountCellNode.innerText = "Amount of chips per player:";
        valueCellNode.innerText = "Chip value:";
    }   

    addPlayerChipsOutput(playerChips);
}