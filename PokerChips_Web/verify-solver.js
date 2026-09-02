// Harness that runs PokerChips.js's solver against the same inputs used by
// TestProject1/ChipCalculatorTests.cs and prints a pass/fail comparison.
//
// Run with: node PokerChips_Web/verify-solver.js
const fs = require("fs");
const path = require("path");
const vm = require("vm");

const scriptPath = path.join(__dirname, "PokerChips.js");
const source = fs.readFileSync(scriptPath, "utf8");

// Minimal DOM mock: getMaxAmount() reads getById("amountPlayers").value and
// getById("maxChips").value, so provide those two elements.
let domValues = {};

const documentMock = {
    getElementById(id)
    {
        if(domValues[id] === undefined)
        {
            throw new Error(`Unexpected getElementById("${id}")`);
        }

        return { value: domValues[id] };
    }
};

const sandbox = { document: documentMock, console: console };
vm.createContext(sandbox);
vm.runInContext(source, sandbox, { filename: "PokerChips.js" });

// Top-level `class`/`function` declarations live in the context's lexical scope,
// not as properties of the sandbox object, so fetch references via another eval.
const ChipCtor = vm.runInContext("Chip", sandbox);
const addPlayerChipsFn = vm.runInContext("addPlayerChips", sandbox);

function runCase(name, caseChipDefs, maxChips, amountPlayers, startingValue, expectedRemaining, expectedPlayerChips)
{
    domValues = { amountPlayers: amountPlayers, maxChips: maxChips };

    const caseChips = caseChipDefs.map(([amount, value]) => new ChipCtor(amount, value));
    const playerChips = [];

    const remainingValue = addPlayerChipsFn(caseChips, playerChips, startingValue);

    let pass = remainingValue === expectedRemaining;

    if(expectedPlayerChips !== undefined)
    {
        const actual = playerChips.map(c => [c.getAmount(), c.getValue()]);
        const matches = actual.length === expectedPlayerChips.length
            && actual.every(([amount, value], index) => amount === expectedPlayerChips[index][0] && value === expectedPlayerChips[index][1]);

        pass = pass && matches;
    }

    console.log(`${pass ? "PASS" : "FAIL"} - ${name}: expected remainingValue=${expectedRemaining}, got ${remainingValue}`);
    console.log(`       playerChips: ${playerChips.map(c => `${c.getAmount()}x${c.getValue()}`).join(", ") || "(none)"}`);

    if(expectedPlayerChips !== undefined)
    {
        console.log(`       expected:    ${expectedPlayerChips.map(([amount, value]) => `${amount}x${value}`).join(", ") || "(none)"}`);
    }

    return pass;
}

let allPassed = true;

// Mirrors ChipCalculatorTests.SimpleDivisibleCase_Succeeds
allPassed = runCase("SimpleDivisibleCase_Succeeds", [[10, 5], [10, 1]], 100, 1, 7, 0) && allPassed;

// Mirrors ChipCalculatorTests.LastChipRunsOutBeforeRemainingValueIsZero_SolverFindsExactMatch
allPassed = runCase("LastChipRunsOutBeforeRemainingValueIsZero_SolverFindsExactMatch", [[3, 10], [3, 6], [2, 1]], 100, 1, 17, 0) && allPassed;

// Mirrors ChipCalculatorTests.EarlierChipNeedsToBeSkippedForLaterChipToDivideEvenly_SolverFindsBestMatch
allPassed = runCase("EarlierChipNeedsToBeSkippedForLaterChipToDivideEvenly_SolverFindsBestMatch", [[1, 10], [1, 5]], 100, 1, 13, 3) && allPassed;

// Mirrors ChipCalculatorTests.DefaultGame
allPassed = runCase("DefaultGame", [[100, 200], [150, 100], [150, 25]], 20, 5, 5000, 0, [[20, 25], [19, 100], [13, 200]]) && allPassed;

console.log(allPassed ? "\nAll cases passed." : "\nSome cases FAILED.");

process.exitCode = allPassed ? 0 : 1;
