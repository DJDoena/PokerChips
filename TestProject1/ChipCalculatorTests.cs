namespace DoenaSoft.PokerChips;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

[TestClass]
public sealed class ChipCalculatorTests
{
    [TestMethod]
    public void SimpleDivisibleCase_Succeeds()
    {
        // Denominations are considered lowest value first: 1, then 5.
        // The solver tries the smallest usable chip count of the current denomination first, so
        // for the 1-chips (cap 10) and target of 7, it first tries 0x1, then 1x1, and so on,
        // stopping as soon as the remainder can be covered by the not-yet-processed 5-chips.
        // 0x1 through 6x1 all leave a remainder that isn't 0 or a multiple of 5 (achievable from
        // 5-chips alone), but 7x1 leaves a remainder of 0, which is trivially achievable using
        // "0 fives". That succeeds, so the solver settles on 7x1 without ever needing a 5-chip.
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
        // Denominations are considered lowest value first: 1, then 6, then 10.
        // Using only 1-chips (cap 2), achievable sums are 0, 1, or 2.
        // Adding 6-chips (cap 3) on top: for each candidate sum, the solver tries the smallest
        // usable count of 6s first. E.g. for a sum of 17, 0x6 through 1x6 leave remainders (17,
        // 11) not achievable from 1s alone, but the solver keeps trying up to the cap; sums built
        // from combinations like 2x6+1(=13) or 1x6+2(=8) become achievable at this stage too.
        // Finally, for the 10-chips (cap 3) and the real target of 17: the solver tries the
        // smallest count first, 0x10, which would need the full 17 to come from 1s+6s alone - not
        // achievable (max reachable there is 2x6+2x1=14). It then tries 1x10, needing the
        // remaining 7 from 1s+6s - and 7 IS achievable (1x6+1x1). That succeeds, so the solver
        // settles on 1x10 + 1x6 + 1x1 = 17, leaving 0 remaining.
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
        // Denominations are considered lowest value first: 5, then 10. With a cap of only
        // 1 chip each, using just the 5-chip can only reach 0 or 5.
        // For the target of 13, the solver first tries using 0x10, which would need the full 13
        // to come from the 5-chip alone - but 13 isn't 0 or 5, so that attempt fails. It then
        // tries 1x10, which would need the remaining 3 to come from the 5-chip - but 3 isn't 0 or
        // 5 either, so that also fails. With no way to hit 13 exactly, the solver falls back to
        // searching for the closest sum below it: 12 and 11 fail the same way (0x10 leaves 12 or
        // 11, 1x10 leaves 2 or 1 - none are 0/5), until it reaches 10, where using 1x10 leaves
        // exactly 0 for the 5-chip to cover (trivially achievable). That succeeds, so the solver
        // settles on 1x10 (and 0x5), leaving a remainder of 3.
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
        // Denominations are considered lowest value first: 25, then 100, then 200.
        // Per-player caps: 150/5=30 (capped by maxChips to 20) of 25, 150/5=30 (capped to 20) of
        // 100, and 100/5=20 of 200 - so every denomination is capped at 20 chips.
        //
        // For each candidate sum, the solver tries the smallest usable chip count of the current
        // denomination first, i.e. it prefers leaving as much as possible to be covered by the
        // (not yet processed) higher-value denominations, and only reaches for more of the current
        // denomination if smaller counts can't make the remainder achievable from what came before.
        //
        // Using only 25-chips (cap 20), achievable sums are multiples of 25 up to 500.
        //
        // Adding 100-chips (cap 20) on top: for a sum like 5000, the smallest counts of 100s are
        // tried first, e.g. 0x100 would need the remaining 5000 to come from 25s alone - not a
        // multiple of 25 within the cap, so that (and several small counts after it) fail, until
        // eventually enough 100s are used to make the remainder reachable from 25s.
        //
        // Finally, the 200-chips (cap 20) are added: for the real target of 5000, the smallest
        // counts of 200 are tried first; most leave a remainder not reachable from 25s/100s alone,
        // until reaching 13x200=2600, which leaves exactly 2400 - achievable via 19x100+20x25
        // (1900+500=2400).
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
        // Same target and per-player caps as DefaultGame, but with two extra, higher-value
        // denominations (1000 and 500) added to the case. Denominations are considered lowest
        // value first: 25, then 100, then 200, then 500, then 1000.
        // Per-player caps: 150/5=30 (capped by maxChips to 20) of 25, 150/5=30 (capped to 20) of
        // 100, 100/5=20 of 200, 50/5=10 of 500, and 50/5=10 of 1000.
        //
        // As in DefaultGame, processing just the 25s, 100s, and 200s already finds an exact match
        // for the target of 5000 (20x25 + 19x100 + 13x200 = 5000), so the isSumAchievable table
        // already marks 5000 as reachable using only those first three denominations.
        //
        // When the solver then considers the 500-chips, it tries the smallest usable count first
        // (0x500): this requires the remaining 5000 to be covered by the earlier denominations
        // (25s/100s/200s) alone - which, as just noted, IS achievable. So the very first attempt
        // (0x500) already succeeds, and no 500-chip is used.
        //
        // The same happens for the 1000-chips: the solver again tries the smallest usable count
        // first (0x1000), which requires the remaining 5000 to be covered by 25s/100s/200s/500s -
        // still achievable via the same 20x25 + 19x100 + 13x200 combination - so 0x1000 succeeds
        // immediately as well.
        //
        // The final result is therefore identical to DefaultGame: 20x25 + 19x100 + 13x200 = 5000,
        // leaving 0 remaining, with neither the 500 nor the 1000 denomination used at all.
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

    [TestMethod]
    public void DefaultGameFullCase_ShuffledOrder()
    {
        // Same case chips (amount/value pairs) as DefaultGameFullCase, but fed into the solver in
        // a shuffled order instead of sorted by value. The solver internally re-sorts the buffered
        // denominations by value (see ChipCalculator.GetDenominationOrderAscendingByValue) before
        // running the DP, so the order the case chips are supplied in should not affect the result.
        var caseChips = new List<Chip>()
        {
            new(amount: 150, value: 100),
            new(amount: 50, value: 1000),
            new(amount: 150, value: 25),
            new(amount: 100, value: 200),
            new(amount: 50, value: 500),
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
    public void ZeroValueChip_IsIgnoredBySolver()
    {
        // A 0-value chip (as the UI allows for unused case-chip color slots) can never
        // contribute to the target sum, so ChipCalculator.AddPlayerChip filters it out before it
        // reaches the solver. This is the same DefaultGame scenario, but with an extra 0-value
        // chip added to the case; the result must be identical to DefaultGame, with the 0-value
        // chip neither causing an error nor appearing in the output.
        var caseChips = new List<Chip>()
        {
            new(amount: 100, value: 200),
            new(amount: 150, value: 100),
            new(amount: 150, value: 25),
            new(amount: 0, value: 0),
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
