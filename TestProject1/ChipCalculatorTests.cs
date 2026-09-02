namespace DoenaSoft.PokerChips;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

[TestClass]
public sealed class ChipCalculatorTests
{
    [TestMethod]
    public void SimpleDivisibleCase_Succeeds()
    {
        // Denominations are considered highest value first: 5, then 1.
        // Using only 5-chips, the solver can only reach 0 or 5 (with cap 10 chips it could go
        // higher, but 7 isn't a multiple of 5, so nothing beyond 5 is usable towards a sum of 7).
        // When it then considers the 1-chips for the target of 7, it immediately tries the
        // largest possible count for them: 7 one-chips, leaving 0 for the 5-chips to cover.
        // Since "0 fives" is trivially valid, that first attempt already succeeds - no 5-chip
        // is used at all, and the solver settles on 7x1 without ever needing to backtrack.
        var caseChips = new List<Chip>()
        {
            new(amount: 10, value: 5),
            new(amount: 10, value: 1),
        };

        var (remainingValue, _) = RunCalculation(caseChips, maxChips: 100, amountPlayers: 1, targetValue: 7);

        Assert.AreEqual(0, remainingValue);
    }

    [TestMethod]
    public void LastChipRunsOutBeforeRemainingValueIsZero_SolverFindsExactMatch()
    {
        // Denominations are considered highest value first: 10, then 6, then 1.
        // Using only 10-chips (cap 3) towards a target of 17, only 0 or 10 are achievable
        // (2x10 = 20 already overshoots).
        // Adding 6-chips (cap 3) on top: 2x6 = 12 combined with 0x10 works (sum 12), and
        // 1x6 combined with 1x10 also works (sum 16); most other sums (e.g. 17 itself) are
        // not reachable yet using just 10s and 6s.
        // Finally, for the 1-chips (cap 2) and the real target of 17: the solver first tries
        // the largest count, 2x1, which would need the 10s+6s to cover the remaining 15 - but
        // 15 is NOT one of the sums achievable from 10s and 6s, so that attempt fails.
        // It backs off to 1x1, which needs the remaining 16 to be covered by 10s+6s - and 16 IS
        // achievable (as 1x10 + 1x6, found above). That combination checks out exactly:
        // 1x10 + 1x6 + 1x1 = 17, leaving 0 remaining.
        var caseChips = new List<Chip>()
        {
            new(amount: 3, value: 10),
            new(amount: 3, value: 6),
            new(amount: 2, value: 1),
        };

        var (remainingValue, _) = RunCalculation(caseChips, maxChips: 100, amountPlayers: 1, targetValue: 17);

        Assert.AreEqual(0, remainingValue);
    }

    [TestMethod]
    public void EarlierChipNeedsToBeSkippedForLaterChipToDivideEvenly_SolverFindsBestMatch()
    {
        // Denominations are considered highest value first: 10, then 5. With a cap of only
        // 1 chip each, using just the 10-chip can only reach 0 or 10.
        // For the target of 13, the solver first tries using the 5-chip (1x5), which would
        // need the remaining 8 to come from the 10-chip alone - but 8 isn't 0 or 10, so that
        // attempt fails. It backs off to using 0x5, which would need the full 13 to come from
        // the 10-chip - but 13 isn't 0 or 10 either, so that also fails. With no way to hit 13
        // exactly, the solver falls back to searching for the closest sum below it: 12 and 11
        // fail the same way (5-chip leaves 7 or 6, no-5-chip leaves 12 or 11 - none are 0/10),
        // until it reaches 10, where using 0x5 leaves exactly 10 for the 10-chip to cover.
        // That succeeds, so the solver settles on 1x10 (and 0x5), leaving a remainder of 3.
        var caseChips = new List<Chip>()
        {
            new(amount: 1, value: 10),
            new(amount: 1, value: 5),
        };

        var (remainingValue, _) = RunCalculation(caseChips, maxChips: 100, amountPlayers: 1, targetValue: 13);

        Assert.AreEqual(3, remainingValue);
    }

    [TestMethod]
    public void DefaultGame()
    {
        // Denominations are considered highest value first: 200, then 100, then 25.
        // Per-player caps: 100/5=20 chips of 200, 150/5=30 (capped by maxChips to 20) of 100,
        // and 150/5=30 (capped to 20) of 25 - so every denomination is capped at 20 chips.
        //
        // Using only 200-chips (cap 20), achievable sums are multiples of 200 up to 4000.
        //
        // For the real target of 5000, the solver first considers the 25-chips: the largest
        // count, 20x25=500, would need the remaining 4500 to come from 200s and 100s - and
        // that IS achievable (e.g. 20x200 + 5x100), so it succeeds immediately with 20x25,
        // leaving 4500 to be covered by 200s and 100s.
        //
        // For that 4500 slice, the solver considers the 100-chips: it first tries the largest
        // count, 20x100=2000, which would need the remaining 2500 to come from 200s alone -
        // but 2500 isn't a multiple of 200, so that attempt fails. It backs off to 19x100=1900,
        // needing the remaining 2600 from 200s alone - and 2600 IS a multiple of 200 (13x200),
        // so that succeeds, settling on 19x100.
        //
        // Finally, the remaining 2600 is covered exactly by 13 chips of 200 (13x200=2600,
        // well within the cap of 20).
        //
        // Combining all three: 20x25 + 19x100 + 13x200 = 500 + 1900 + 2600 = 5000, leaving 0
        // remaining.
        var caseChips = new List<Chip>()
        {
            new(amount: 100, value: 200),
            new(amount: 150, value: 100),
            new(amount: 150, value: 25),
        };

        var (remainingValue, playerChips) = RunCalculation(caseChips, maxChips: 20, amountPlayers: 5, targetValue: 5000);

        Assert.AreEqual(0, remainingValue);
        Assert.HasCount(3, playerChips);

        Assert.AreEqual(25, playerChips[0].Value);
        Assert.AreEqual(20, playerChips[0].Amount);


        Assert.AreEqual(100, playerChips[1].Value);
        Assert.AreEqual(19, playerChips[1].Amount);

        Assert.AreEqual(200, playerChips[2].Value);
        Assert.AreEqual(13, playerChips[2].Amount);
    }

    [TestMethod]
    public void DefaultGameFullCase()
    {
        var caseChips = new List<Chip>()
        {
            new(amount: 50, value: 1000),
            new(amount: 50, value: 500),
            new(amount: 100, value: 200),
            new(amount: 150, value: 100),
            new(amount: 150, value: 25),
        };

        var (remainingValue, playerChips) = RunCalculation(caseChips, maxChips: 20, amountPlayers: 5, targetValue: 5000);

        Assert.AreEqual(0, remainingValue);
        Assert.HasCount(3, playerChips);

        Assert.AreEqual(25, playerChips[0].Value);
        Assert.AreEqual(20, playerChips[0].Amount);


        Assert.AreEqual(100, playerChips[1].Value);
        Assert.AreEqual(19, playerChips[1].Amount);

        Assert.AreEqual(200, playerChips[2].Value);
        Assert.AreEqual(13, playerChips[2].Amount);
    }

    private static (int remainingValue, List<Chip> playerChips) RunCalculation(List<Chip> caseChips
        , int maxChips
        , int amountPlayers
        , int targetValue)
    {
        var playerChips = new List<Chip>();

        var remainingValue = PlayerChipsHelper.CreatePlayerChips(playerChips, caseChips, maxChips, amountPlayers, targetValue);

        return (remainingValue, playerChips);
    }
}
