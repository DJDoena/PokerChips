namespace TestProject1;

using DoenaSoft.PokerChips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

[TestClass]
public sealed class ChipCalculatorTests
{
    // Mirrors MainForm.CreatePlayerChips: feeds the ordered case chips into the
    // calculator one by one and stops early if a step reports "finished".
    private static int RunCalculation(List<Chip> caseChips, int maxChips, int amountPlayers, int startingValue, out List<Chip> playerChips)
    {
        playerChips = [];

        var remainingValue = startingValue;

        var chipCalculator = new ChipCalculator(playerChips, maxChips, amountPlayers);

        for (var index = 0; index < caseChips.Count; index++)
        {
            var currentCaseChip = caseChips[index];

            var nextCaseChip = (index < caseChips.Count - 1) ? caseChips[index + 1] : null;

            if (chipCalculator.AddPlayerChip(currentCaseChip, nextCaseChip, ref remainingValue))
            {
                break;
            }
        }

        return remainingValue;
    }

    [TestMethod]
    public void SimpleDivisibleCase_Succeeds()
    {
        var caseChips = new List<Chip>
        {
            new(amount: 10, value: 5),
            new(amount: 10, value: 1),
        };

        var remainingValue = RunCalculation(caseChips, maxChips: 100, amountPlayers: 1, startingValue: 7, out var playerChips);

        Assert.AreEqual(0, remainingValue);
    }

    [TestMethod]
    public void LastChipRunsOutBeforeRemainingValueIsZero_CurrentlyReportsFalseFinished()
    {
        // 10 + 6 + 1 = 17 is mathematically achievable (1x10, 1x6, 1x1),
        // but the greedy algorithm never tries that combination and,
        // because it treats the last denomination as always "final",
        // it reports success (AddPlayerChip returns true) even though
        // 3 chips worth of value are left unaccounted for.
        // This test pins the CURRENT (buggy) behavior; it should be
        // updated to expect 0 once the solver replaces the greedy logic.
        var caseChips = new List<Chip>
        {
            new(amount: 3, value: 10),
            new(amount: 3, value: 6),
            new(amount: 2, value: 1),
        };

        var remainingValue = RunCalculation(caseChips, maxChips: 100, amountPlayers: 1, startingValue: 17, out var playerChips);

        // TODO: once the solver is implemented, this should be 0.
        Assert.AreEqual(3, remainingValue);
    }

    [TestMethod]
    public void EarlierChipNeedsToBeSkippedForLaterChipToDivideEvenly_CurrentlyFails()
    {
        // 10 is skipped entirely by the greedy algorithm because it can't
        // find a way to make the *next* denomination (6) divide the
        // remainder evenly, even though skipping straight to 6 and 1
        // would have worked out mathematically for the whole set.
        // This test pins the CURRENT (buggy) behavior; it should be
        // updated to expect 0 once the solver replaces the greedy logic.
        var caseChips = new List<Chip>
        {
            new(amount: 1, value: 10),
            new(amount: 1, value: 5),
        };

        var remainingValue = RunCalculation(caseChips, maxChips: 100, amountPlayers: 1, startingValue: 13, out var playerChips);

        // TODO: once the solver is implemented, this should be 0.
        Assert.AreEqual(8, remainingValue);
    }
}
