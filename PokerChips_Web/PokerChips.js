// Frontend / UI logic: DOM manipulation, table building, case presets, localization, and the
// "Calculate" button handler. Chip allocation itself lives in PokerChipsSolver.js.
const PokerTable = "pokerTable";
const PlayerChipRowPrefix = "playerChipRow_";
const CaseChipAmountPrefix = "caseChipAmount_";
const CaseChipValuePrefix = "caseChipValue_";
const CalculateRowId = "calculateRow";

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

// Shorthand for document.getElementById.
function getById(id)
{
    let node = document.getElementById(id);

    return node;
}

// Shorthand for setting a node's id attribute.
function setId(node, id)
{
    node.setAttribute("id", id);
}

// Appends a single <option> (with its text set to value) to the given <select> node.
function addSelectOption(selectNode, value)
{
    let optionNode = document.createElement("OPTION");
    optionNode.innerText = value;
    selectNode.add(optionNode);
}

// Populates an amount <select> with the fixed set of selectable chip amounts.
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

// Populates a value <select> with the fixed set of selectable chip denominations.
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

// Appends a new table cell containing an (initially empty) <select> node with the given id,
// returning the <select> node so its options can be populated by the caller.
function addSelectCell(rowNode, id)
{
    let cellNode = rowNode.insertCell(-1);
    cellNode.setAttribute("style", "text-align: center;");

    let selectNode = document.createElement("SELECT");
    setId(selectNode, id);

    cellNode.appendChild(selectNode);

    return selectNode;
}

// Adds one case-chip input row (amount <select> + value <select>) to the poker table, identified
// by caseChipIndex, used once per case-chip color slot at page setup.
// Inserted directly above the "Calculate" button row (identified by CalculateRowId) instead of
// being appended at the end of the table, since the setup script that calls this runs after the
// whole table (including the button row) has already been parsed - <script> is not a valid child
// of <table>/<tr>, so it can no longer live between the header row and the button row in the HTML.
function addCaseRow(caseChipIndex)
{
    let calculateRowNode = getById(CalculateRowId);

    let rowNode = getById(PokerTable).insertRow(calculateRowNode.rowIndex);

    let amountSelectNode = addSelectCell(rowNode, CaseChipAmountPrefix + caseChipIndex);
    addAmountOptions(amountSelectNode);

    let valueSelectNode = addSelectCell(rowNode, CaseChipValuePrefix + caseChipIndex);
    addValueOptions(valueSelectNode);
}

// Sets the amount/value selection for one case-chip row, used by the preset-case functions below.
// Silently does nothing if caseChipIndex is out of range (more color slots requested than exist).
function setCaseDefaults(caseChipIndex, amount, value)
{
    if(caseChipIndex >= NumberOfCaseChipColors)
    {
        return;
    }

    getById(CaseChipAmountPrefix + caseChipIndex).value = amount;
    getById(CaseChipValuePrefix + caseChipIndex).value = value;
}

// Preset: fills the case rows with a single normal 500-chip case (25/100/200 only, no 500s/1000s).
function setOneNormal500Case()
{
    setCaseDefaults(0, AmountOption150, ValueOption25);
    setCaseDefaults(1, AmountOption150, ValueOption100);
    setCaseDefaults(2, AmountOption100, ValueOption200);
    setCaseDefaults(3, AmountOption0, ValueOption0);
    setCaseDefaults(4, AmountOption0, ValueOption0);
}

// Preset: fills the case rows with a single full 500-chip case (adds 500s and 1000s on top of
// setOneNormal500Case).
function setOneFull500Case()
{
    setCaseDefaults(0, AmountOption150, ValueOption25);
    setCaseDefaults(1, AmountOption150, ValueOption100);
    setCaseDefaults(2, AmountOption100, ValueOption200);
    setCaseDefaults(3, AmountOption50, ValueOption500);
    setCaseDefaults(4, AmountOption50, ValueOption1000);
}

// Preset: fills the case rows with two full 500-chip cases combined (double the amounts of
// setOneFull500Case).
function setTwo500Case()
{
    setCaseDefaults(0, AmountOption300, ValueOption25);
    setCaseDefaults(1, AmountOption300, ValueOption100);
    setCaseDefaults(2, AmountOption200, ValueOption200);
    setCaseDefaults(3, AmountOption100, ValueOption500);
    setCaseDefaults(4, AmountOption100, ValueOption1000);
}

// Removes a single previously-added player-chip output row (identified by playerChipIndex, or "H"
// for the header row), if it exists, so a fresh "Calculate" run doesn't append duplicate rows.
function initPlayerChipOutput(playerChipIndex)
{
    let rowNode = getById(PlayerChipRowPrefix + playerChipIndex);

    if(rowNode !== null)
    {
        getById(PokerTable).deleteRow(rowNode.rowIndex);
    }
}

// Clears all player-chip output rows (header + one per case-chip color slot) left over from a
// previous "Calculate" run, in preparation for a new one.
function initPlayerChipsOutput()
{
    initPlayerChipOutput("H");

    for(let playerChipIndex = 0; playerChipIndex < NumberOfCaseChipColors; playerChipIndex++)
    {
        initPlayerChipOutput(playerChipIndex);
    }

    initPlayerChipOutput("GrandTotal");
}

// Reads the current amount/value selection for one case-chip row and builds the corresponding Chip.
function createCaseChip(caseChipIndex)
{
    let amount = parseInt(getById(CaseChipAmountPrefix + caseChipIndex).value);
    let value = parseInt(getById(CaseChipValuePrefix + caseChipIndex).value);

    return new Chip(amount, value);
}

// Sorts caseChips ascending by value in place (simple bubble sort), matching the order the
// solver's own denomination sort expects, so the resulting output rows are shown lowest-value first.
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

// A chip with an amount greater than 0 but a value of 0 is not a legitimate case chip (mirrors
// the guard in ChipCalculator.AddPlayerChip / solveChipAllocation), so the UI rejects it before
// it ever reaches the solver. Must be checked against the case chips in their original (UI) row
// order, before they get resorted by value, so the reported row number matches what the user sees.
// Returns the 1-based row number of the first offending case chip, or -1 if every case chip is valid.
function findRowWithAmountButNoValue(unsortedCaseChips)
{
    for(let caseChipIndex = 0; caseChipIndex < unsortedCaseChips.length; caseChipIndex++)
    {
        if(unsortedCaseChips[caseChipIndex].getAmount() > 0 && unsortedCaseChips[caseChipIndex].getValue() === 0)
        {
            return caseChipIndex + 1;
        }
    }

    return -1;
}

// Reads all case-chip rows into Chip objects and validates them. If any row has an amount but no
// value, sorting/solving is skipped and the offending row number is returned via invalidRow;
// otherwise the chips are sorted ascending by value (see sortCaseChips) and returned in caseChips.
function createCaseChips()
{
    let caseChips = [];

    for(let caseChipIndex = 0; caseChipIndex < NumberOfCaseChipColors; caseChipIndex++)
    {
        caseChips.push(createCaseChip(caseChipIndex));
    }

    let invalidRow = findRowWithAmountButNoValue(caseChips);

    if(invalidRow !== -1)
    {
        return { caseChips: null, invalidRow: invalidRow };
    }

    sortCaseChips(caseChips);

    return { caseChips: caseChips, invalidRow: -1 };
}

// Appends one output row showing the resolved amount/value/total value of a single player chip
// denomination.
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

    let totalCellNode = rowNode.insertCell(-1);
    totalCellNode.setAttribute("style", "text-align: center;");
    totalCellNode.innerText = playerChip.getAmount() * playerChip.getValue();
}

// Appends one output row per resolved player chip denomination, followed by a bold grand-total
// row showing the combined total value of all player chips (mirrors ResultForm.AddGrandTotalLabel).
function addPlayerChipsOutput(playerChips)
{
    let grandTotal = 0;

    for(let playerChipIndex = 0; playerChipIndex < playerChips.length; playerChipIndex++)
    {
        addPlayerChipOutput(playerChipIndex, playerChips[playerChipIndex]);

        grandTotal += playerChips[playerChipIndex].getAmount() * playerChips[playerChipIndex].getValue();
    }

    let rowNode = getById(PokerTable).insertRow(-1);
    setId(rowNode, PlayerChipRowPrefix + "GrandTotal");

    rowNode.insertCell(-1);
    rowNode.insertCell(-1);

    let grandTotalCellNode = rowNode.insertCell(-1);
    grandTotalCellNode.setAttribute("style", "text-align: center; font-weight: bold;");
    grandTotalCellNode.innerText = grandTotal;
}

// All user-facing texts, keyed by language. Add a language by adding a new top-level key here.
const Translations =
{
    de:
    {
        insufficientChips: "Die Anzahl der Chips mal den Wert der Chips ist nicht ausreichend für die Anzahl der Spieler!",
        amountOfChipsPerPlayer: "Anzahl Chips je Spieler:",
        chipValue: "Chip Wert:",
        totalValue: "Gesamtwert:",
        amountSetButValueIsZero: "In Zeile {0} wurde eine Anzahl ohne einen Chip-Wert ausgewählt!"
    },
    en:
    {
        insufficientChips: "Number of chips times value of chips is insufficient for the number of players!",
        amountOfChipsPerPlayer: "Amount of chips per player:",
        chipValue: "Chip value:",
        totalValue: "Total Value:",
        amountSetButValueIsZero: "In row {0} an amount was selected without a chip value!"
    }
};

// "Calculate" button handler: validates and reads the case chips, clears previous output, runs the
// solver (see addPlayerChips in PokerChipsSolver.js), and renders either an error alert (invalid
// input row, or insufficient chips for the requested stack size) or the resolved player chips.
function calculate(languageCode)
{
    let translations = Translations[languageCode] || Translations.en;

    initPlayerChipsOutput();
    let caseChipsResult = createCaseChips();

    if(caseChipsResult.invalidRow !== -1)
    {
        alert(translations.amountSetButValueIsZero.replace("{0}", caseChipsResult.invalidRow));

        return;
    }

    let caseChips = caseChipsResult.caseChips;
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

    let totalCellNode = rowNode.insertCell(-1);
    totalCellNode.setAttribute("style", "text-align: center; font-weight: bold;");
    totalCellNode.innerText = translations.totalValue;

    addPlayerChipsOutput(playerChips);
}
