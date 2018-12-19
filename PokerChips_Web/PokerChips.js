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

class AddPlayerChipResult
{
    constructor(isFinished, remainingValue)
    {
        this._isFinished = isFinished;
        this._remainingValue = parseInt(remainingValue);
    }

    getIsFinished()
    {
        return this._isFinished;
    }

    getRemainingValue()
    {
        return this._remainingValue;
    }
}

const NumberOfCaseChipColors = 5;

const AmountOption0 = 0;
const AmountOption50 = 50;
const AmountOption100 = 100;
const AmountOption150 = 150;
const AmountOption200 = 200;
const AmountOption250 = 250;
const AmountOption300 = 300;

const ValueOption0 = 0;
const ValueOption25 = 25;
const ValueOption50 = 50;
const ValueOption100 = 100;
const ValueOption200 = 200;
const ValueOption500 = 500;
const ValueOption1000 = 1000;

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
}

function addValueOptions(selectNode)
{
    addSelectOption(selectNode, ValueOption0);
    addSelectOption(selectNode, ValueOption25);
    addSelectOption(selectNode, ValueOption50);
    addSelectOption(selectNode, ValueOption100);
    addSelectOption(selectNode, ValueOption200);
    addSelectOption(selectNode, ValueOption500);
    addSelectOption(selectNode, ValueOption1000);
}

function addSelectCellNode(rowNode, id)
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
    let rowNode = getById("caseTable").insertRow(-1);
    let amountSelectNode = addSelectCellNode(rowNode, "caseChipAmount_" + caseChipIndex);
    addAmountOptions(amountSelectNode);
    let valueSelectNode = addSelectCellNode(rowNode, "caseChipValue_" + caseChipIndex);
    addValueOptions(valueSelectNode);
}

function setCaseDefaults(caseChipIndex, amount, value)
{
    if(caseChipIndex >= NumberOfCaseChipColors)
    {
        return;
    }

    getById("caseChipAmount_" + caseChipIndex).value = amount;
    getById("caseChipValue_" + caseChipIndex).value = value;
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
    let rowNode = getById("playerChipRow_" + playerChipIndex);

    if(rowNode !== null)
    {
        getById("caseTable").deleteRow(rowNode.rowIndex);
    }
}

function initPlayerChipsOutput()
{
    for(let playerChipIndex = 0; playerChipIndex < NumberOfCaseChipColors; playerChipIndex++)
    {
        initPlayerChipOutput(playerChipIndex);
    }
}

function createCaseChip(caseChipIndex)
{
    let amount = parseInt(getById("caseChipAmount_" + caseChipIndex).value);
    let value = parseInt(getById("caseChipValue_" + caseChipIndex).value);
    
    return new Chip(amount, value);
}

function createCaseChips()
{
    let caseChips = [];

    for(let caseChipIndex = 0; caseChipIndex < NumberOfCaseChipColors; caseChipIndex++)
    {
        caseChips.push(createCaseChip(caseChipIndex));
    }

    return caseChips;
}

function sortCaseChips(caseChips)
{
    let resorted = false;

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

function chipsExceedRemainingValue(chipAmount, chipValue, remainingValue)
{
    let result = (chipAmount * chipValue) >= remainingValue;

    return result;
}

function getAmount(caseChipAmount, chipValue, remainingValue)
{
    let amountPlayers = parseInt(getById("amountPlayers").value);
    let chipAmount = Math.floor(caseChipAmount / amountPlayers);;
    let maxChips = parseInt(getById("maxChips").value);

    if(chipAmount > maxChips)
    {
        chipAmount = maxChips;
    }

    if(chipsExceedRemainingValue(chipAmount, chipValue, remainingValue) === true)
    {
        chipAmount = Math.floor(remainingValue / chipValue);
    }

    return chipAmount;
}

function isDivisibleWithoutRest(chipValue, remainingValue)
{
    let result = (remainingValue % chipValue) === 0;

    return result;
}

function isLastChip(amount, chipValue, nextCaseChip, remainingValue)
{
    let result = (nextCaseChip === null) || ((chipsExceedRemainingValue(amount, chipValue, remainingValue) === true)  &&  (isDivisibleWithoutRest(chipValue, remainingValue) === true))
    
    return result;
}

function addPlayerChip(chipAmount, chipValue, playerChips, remainingValue)
{
    playerChips.push(new Chip(chipAmount, chipValue));
    remainingValue -= (chipAmount * chipValue);
    
    return new AddPlayerChipResult(remainingValue === 0, remainingValue); 
}

function tryAddPlayerChipWithConvenientValueForNextChip(currentChipAmount, currentChipValue, nextChipValue, playerChips, remainingValue)
{
    for(; currentChipAmount > 0; currentChipAmount--)
    {
        let potentialRemainingValue = remainingValue - (currentChipAmount * currentChipValue);

        if(isDivisibleWithoutRest(nextChipValue, potentialRemainingValue))
        {
            let result = addPlayerChip(currentChipAmount, currentChipValue, playerChips, remainingValue);

            return result;
        }
    }

    return new AddPlayerChipResult(false, remainingValue);
}

function tryAddPlayerChip(currentCaseChip, nextCaseChip, playerChips, remainingValue)
{
    let chipValue = currentCaseChip.getValue();                
    let amount = getAmount(currentCaseChip.getAmount(), chipValue, remainingValue);
    
    if(isLastChip(amount, chipValue, nextCaseChip, remainingValue) === true)
    {
        let result = addPlayerChip(amount, chipValue, playerChips, remainingValue);

        return result;
    }

    let result = tryAddPlayerChipWithConvenientValueForNextChip(amount, chipValue, nextCaseChip.getValue(), playerChips, remainingValue);

    return result;
}

function addPlayerChips(caseChips, playerChips, remainingValue)
{
    for(let caseChipIndex = 0; caseChipIndex < caseChips.length; caseChipIndex++)
    {
        let currentCaseChip = caseChips[caseChipIndex];
        let nextCaseChip = null;

        if(caseChipIndex < caseChips.length -1)
        {
            nextCaseChip = caseChips[caseChipIndex + 1];
        }

        let result = tryAddPlayerChip(currentCaseChip, nextCaseChip, playerChips, remainingValue);
        remainingValue = result.getRemainingValue();

        if(result.getIsFinished() === true)
        {
            break;
        }
    }

    return remainingValue;
}

function addPlayerChipOutput(playerChipIndex, playerChip)
{
    let rowNode = getById("caseTable").insertRow(-1);
    setId(rowNode, "playerChipRow_" + playerChipIndex);
    let amountCellNode = rowNode.insertCell(-1);
    setId(amountCellNode, "playerChipAmountCell_" + playerChipIndex);
    amountCellNode.setAttribute("style", "text-align: center;");
    amountCellNode.innerText = playerChip.getAmount();
    let valueCellNode = rowNode.insertCell(-1);
    setId(valueCellNode, "playerChipValueCell_" + playerChipIndex);
    valueCellNode.setAttribute("style", "text-align: center;");
    valueCellNode.innerText = playerChip.getValue();
}

function addPlayerChipsOutput(playerChips)
{
    for(let playerChipIndex = 0; playerChipIndex < playerChips.length; playerChipIndex++)
    {
        let playerChip = playerChips[playerChipIndex];

        addPlayerChipOutput(playerChipIndex, playerChip);
    }
}

function calulate()
{
    initPlayerChipsOutput();
    let caseChips = createCaseChips();
    sortCaseChips(caseChips);
    let playerChips = [];
    let remainingValue = parseInt(getById("stackSize").value);
    remainingValue = addPlayerChips(caseChips, playerChips, remainingValue);

    if (remainingValue !== 0)
    {
        alert("Number of chips times value of chips is insufficient for these players!");

        return;
    }

    addPlayerChipsOutput(playerChips);
}