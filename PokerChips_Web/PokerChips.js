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
    constructor(isFinished, remainingValue, nextPlayerChipIndex)
    {
        this._isFinished = isFinished;
        this._remainingValue = parseInt(remainingValue);
        this._nextPlayerChipIndex = parseInt(nextPlayerChipIndex);
    }

    getIsFinished()
    {
        return this._isFinished;
    }

    getRemainingValue()
    {
        return this._remainingValue;
    }

    getNextPlayerChipIndex()
    {
        return this._nextPlayerChipIndex;
    }
}

function getById(id)
{
    var node = document.getElementById(id);

    return node;
}

function setId(node, id)
{
    node.setAttribute("id", id);
}

function addSelectOption(selectNode, value)
{
    var optionNode = document.createElement("OPTION");
    optionNode.innerText = value;
    selectNode.add(optionNode);
}

function addAmountOptions(amountSelectNode)
{
    addSelectOption(amountSelectNode, 0);
    addSelectOption(amountSelectNode, 50);
    addSelectOption(amountSelectNode, 100);
    addSelectOption(amountSelectNode, 150);
    addSelectOption(amountSelectNode, 200);
    addSelectOption(amountSelectNode, 250);
    addSelectOption(amountSelectNode, 300);
}

function addValueOptions(valueSelectNode)
{
    addSelectOption(valueSelectNode, 0);
    addSelectOption(valueSelectNode, 25);
    addSelectOption(valueSelectNode, 50);
    addSelectOption(valueSelectNode, 100);
    addSelectOption(valueSelectNode, 200);
    addSelectOption(valueSelectNode, 500);
    addSelectOption(valueSelectNode, 1000);
}

function addSelectCellNode(rowNode, id)
{
    var cellNode = rowNode.insertCell(-1);
    cellNode.setAttribute("style", "text-align: center;");
    var selectNode = document.createElement("SELECT");
    setId(selectNode, id);
    cellNode.appendChild(selectNode);

    return selectNode;
}

function addCaseRow(caseIndex)
{
    var rowNode = getById("caseTable").insertRow(-1);
    var amountSelectNode = addSelectCellNode(rowNode, "caseChipAmount_" + caseIndex);
    addAmountOptions(amountSelectNode);
    var valueSelectNode = addSelectCellNode(rowNode, "caseChipValue_" + caseIndex);
    addValueOptions(valueSelectNode);
}

function setCaseDefaults(caseIndex, amount, value)
{
    getById("caseChipAmount_" + caseIndex).value = amount;
    getById("caseChipValue_" + caseIndex).value = value;
}

function setOneNormal500Case()
{
    setCaseDefaults(0, 150, 25);
    setCaseDefaults(1, 150, 100);
    setCaseDefaults(2, 100, 200);
    setCaseDefaults(3, 0, 0);
    setCaseDefaults(4, 0, 0);
}

function setOneFull500Case()
{
    setCaseDefaults(0, 150, 25);
    setCaseDefaults(1, 150, 100);
    setCaseDefaults(2, 100, 200);
    setCaseDefaults(3, 50, 500);
    setCaseDefaults(4, 50, 1000);
}

function setTwo500Case()
{
    setCaseDefaults(0, 300, 25);
    setCaseDefaults(1, 300, 100);
    setCaseDefaults(2, 200, 200);
    setCaseDefaults(3, 100, 500);
    setCaseDefaults(4, 100, 1000);
}

function initPlayerChipOutput(playerChipIndex)
{
    var rowNode = getById("playerChipRow_" + playerChipIndex);

    if(rowNode !== null)
    {
        getById("caseTable").deleteRow(rowNode.rowIndex);
    }
}

function initPlayerChipsOutput()
{
    for(var playerChipIndex = 0; playerChipIndex < 6; playerChipIndex++)
    {
        initPlayerChipOutput(playerChipIndex);
    }
}

function createCaseChip(caseIndex)
{
    var amount = parseInt(getById("caseChipAmount_" + caseIndex).value);
    var value = parseInt(getById("caseChipValue_" + caseIndex).value);
    
    return new Chip(amount, value);
}

function sortCaseChips(caseChips)
{
    var resorted;

    do
    {
        resorted = false;

        for(var caseIndex = 0; caseIndex < caseChips.length - 2; caseIndex++)
        {
            if(caseChips[caseIndex].getValue() > caseChips[caseIndex + 1].getValue())
            {
                var temp = caseChips[caseIndex];
                caseChips[caseIndex] = caseChips[caseIndex + 1];
                caseChips[caseIndex + 1] = temp;
                resorted = true;
            }
        }
    }while (resorted === true);
}

function chipsExceedRemainingValue(chipAmount, chipValue, remainingValue)
{
    var result = (chipAmount * chipValue) >= remainingValue;

    return result;
}

function getAmount(caseChipAmount, chipValue, remainingValue)
{
    var amountPlayers = parseInt(getById("amountPlayers").value);
    var chipAmount = Math.floor(caseChipAmount / amountPlayers);;
    var maxChips = parseInt(getById("maxChips").value);

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
    var result = (remainingValue % chipValue) === 0;

    return result;
}

function isLastChip(amount, chipValue, nextCaseChip, remainingValue)
{
    var result = (nextCaseChip === null) || ((chipsExceedRemainingValue(amount, chipValue, remainingValue) === true)  &&  (isDivisibleWithoutRest(chipValue, remainingValue) === true))
    
    return result;
}

function addPlayerChip(chipAmount, chipValue, playerChips, playerChipIndex, remainingValue)
{
    playerChips[playerChipIndex] = new Chip(chipAmount, chipValue);
    remainingValue = remainingValue - (chipAmount * chipValue);
    
    return new AddPlayerChipResult(remainingValue === 0, remainingValue, playerChipIndex + 1); 
}

function tryAddPlayerChipWithConvenientValueForNextChip(currentChipAmount, currentChipValue, nextChipValue, playerChips, playerChipIndex, remainingValue)
{
    for(; currentChipAmount > 0; currentChipAmount--)
    {
        var potentialRemainingValue = remainingValue - (currentChipAmount * currentChipValue);

        if(isDivisibleWithoutRest(nextChipValue, potentialRemainingValue))
        {
            var result = addPlayerChip(currentChipAmount, currentChipValue, playerChips, playerChipIndex, remainingValue);

            return result;
        }
    }

    return new AddPlayerChipResult(false, remainingValue, playerChipIndex);
}

function tryAddPlayerChip(currentCaseChip, nextCaseChip, playerChips, playerChipIndex, remainingValue)
{
    var chipValue = currentCaseChip.getValue();                
    var amount = getAmount(currentCaseChip.getAmount(), chipValue, remainingValue);
    
    if(isLastChip(amount, chipValue, nextCaseChip, remainingValue) === true)
    {
        var result = addPlayerChip(amount, chipValue, playerChips, playerChipIndex, remainingValue);

        return result;
    }

    var result = tryAddPlayerChipWithConvenientValueForNextChip(amount, chipValue, nextCaseChip.getValue(), playerChips, playerChipIndex, remainingValue);

    return result;
}

function addPlayerChips(caseChips, playerChips, remainingValue)
{
    var playerChipIndex = 0;

    for(var caseChipIndex = 0; caseChipIndex < caseChips.length; caseChipIndex++)
    {
        var currentCaseChip = caseChips[caseChipIndex];
        var nextCaseChip = null;

        if(caseChipIndex < caseChips.length -1)
        {
            nextCaseChip = caseChips[caseChipIndex + 1];
        }

        var result = tryAddPlayerChip(currentCaseChip, nextCaseChip, playerChips, playerChipIndex, remainingValue);
        remainingValue = result.getRemainingValue();
        playerChipIndex = result.getNextPlayerChipIndex();

        if(result.getIsFinished() === true)
        {
            break;
        }
    }

    return remainingValue;
}

function addPlayerChipOutput(playerChipIndex, playerChip)
{
    var rowNode = getById("caseTable").insertRow(-1);
    setId(rowNode, "playerChipRow_" + playerChipIndex);
    var amountCellNode = rowNode.insertCell(-1);
    setId(amountCellNode, "playerChipAmountCell_" + playerChipIndex);
    amountCellNode.setAttribute("style", "text-align: center;");
    amountCellNode.innerText = playerChip.getAmount();
    var valueCellNode = rowNode.insertCell(-1);
    setId(valueCellNode, "playerChipValueCell_" + playerChipIndex);
    valueCellNode.setAttribute("style", "text-align: center;");
    valueCellNode.innerText = playerChip.getValue();
}

function addPlayerChipsOutput(playerChips)
{
    for(var playerChipIndex = 0; playerChipIndex < playerChips.length; playerChipIndex++)
    {
        var playerChip = playerChips[playerChipIndex];

        if(playerChip === null)
        {
            continue;
        }

        addPlayerChipOutput(playerChipIndex, playerChip);
    }
}

function calulate()
{
    initPlayerChipsOutput();
    var caseChips = [ createCaseChip(0), createCaseChip(1), createCaseChip(2), createCaseChip(3), createCaseChip(4) ];
    sortCaseChips(caseChips);
    var playerChips = [ null, null, null, null, null ];
    var remainingValue = parseInt(getById("stackSize").value);
    remainingValue = addPlayerChips(caseChips, playerChips, remainingValue);

    if (remainingValue !== 0)
    {
        alert("Number of chips + value of chips is insufficient for these players!");

        return;
    }

    addPlayerChipsOutput(playerChips);
}