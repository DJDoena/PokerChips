// Frontend / UI logic: DOM manipulation, table building, case presets, localization, and the
// "Calculate" button handler. Chip allocation itself lives in PokerChipsSolver.js.
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

// All user-facing texts, keyed by language. Add a language by adding a new top-level key here.
const Translations =
{
    de:
    {
        insufficientChips: "Die Anzahl der Chips mal den Wert der Chips ist nicht ausreichend für die Anzahl der Spieler!",
        amountOfChipsPerPlayer: "Anzahl Chips je Spieler:",
        chipValue: "Chip Wert:"
    },
    en:
    {
        insufficientChips: "Number of chips times value of chips is insufficient for the number of players!",
        amountOfChipsPerPlayer: "Amount of chips per player:",
        chipValue: "Chip value:"
    }
};

function getTranslations(isGerman)
{
    return isGerman === true ? Translations.de : Translations.en;
}

function calculate(isGerman)
{
    let translations = getTranslations(isGerman);

    initPlayerChipsOutput();
    let caseChips = createCaseChips();
    let playerChips = [];
    let stackSize = parseInt(getById("stackSize").value);
    let amountPlayers = parseInt(getById("amountPlayers").value);
    let maxChips = parseInt(getById("maxChips").value);
    let remainingValue = addPlayerChips(caseChips, playerChips, stackSize, amountPlayers, maxChips);

    if(remainingValue !== 0)
    {
        alert(translations.insufficientChips);

        return;
    }

    let rowNode = getById(PokerTable).insertRow(-1);
    setId(rowNode, PlayerChipRowPrefix + "H");

    let amountCellNode = rowNode.insertCell(-1);
    amountCellNode.setAttribute("style", "text-align: center; font-weight: bold;");
    amountCellNode.innerText = translations.amountOfChipsPerPlayer;

    let valueCellNode = rowNode.insertCell(-1);
    valueCellNode.setAttribute("style", "text-align: center; font-weight: bold;");
    valueCellNode.innerText = translations.chipValue;

    addPlayerChipsOutput(playerChips);
}
