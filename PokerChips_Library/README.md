# Chip Calculator Solver

`ChipCalculator` (see [ChipCalculator.cs](ChipCalculator.cs)) determines how many chips of each
denomination to hand out per player, given a case of available chips (each with an amount and a
value) and a target stack value per player. It is used by
[PlayerChipsHelper.CreatePlayerChips](PlayerChipsHelper.cs).

## Problem being solved

This is a classic **bounded knapsack** problem: each denomination (chip value) may be used between
0 and its own cap number of times, and we want the achievable sum closest to (but not exceeding)
the target value.

- The **cap** for a denomination is `min(caseChipAmount / amountPlayers, maxChipsPerValue)` (see
  `GetMaxAmount`) - i.e. how many chips of that value are actually available per player, capped by
  a configured maximum.
- Chips with a value of 0 (used by the UI for unused case-chip color slots) can never contribute to
  the target sum, so they are filtered out before reaching the solver (see `AddPlayerChip`).

## Algorithm overview

1. **Sort ascending by value.** The buffered denominations are sorted from lowest to highest value
   (`GetDenominationsSortedAscendingByValue`). Processing lowest-value first, combined with the
   tie-break below, means the solver prefers using more of the smaller denominations and only
   reaches for larger ones when necessary - e.g. it won't hand out a 1000-value chip if the target
   can already be hit exactly using only smaller chips.

2. **Shrink the table using the GCD.** Every achievable sum is necessarily a multiple of the
   greatest common divisor (GCD) of all denomination values combined. Dividing all denomination
   values and the target by that GCD shrinks the dynamic-programming table without changing which
   sums are achievable, just their scale (see `GetDenominationGcd`/`Solve`).

   Note that this is the GCD of *all* denominations combined, not of any single denomination's
   value. For example, with denominations 25/100/200/500/1000 (GCD 25), a scaled sum of 199
   (unscaled: 4975) is a multiple of 25 but not of 100/200/500/1000 individually - it can still be
   achievable via a mix of denominations (mostly 25s). Being a multiple of the GCD is a
   **necessary** condition for achievability, not a **sufficient** one - the DP table still has to
   be checked cell-by-cell, since not every multiple of the GCD is actually reachable (e.g. if caps
   are too restrictive).

3. **Build the bounded-knapsack DP table** (`BuildBoundedKnapsackTable`). Two parallel tables are
   built, each with one row per denomination (plus a base row) and one column per achievable sum
   (in scaled units, from 0 to the scaled target):
   - `isSumAchievable[denominationIndex, sum]`: whether `sum` can be built exactly using only the
	 first `denominationIndex` denominations, each within its own cap.
   - `chipCountUsedForSum[denominationIndex, sum]`: how many chips of the denomination at
	 `denominationIndex - 1` were used to achieve `sum`, so the choice can be reconstructed
	 afterwards.

   Both tables are allocated as full rectangular arrays, so a `chipCountUsedForSum` entry is only
   meaningful where the corresponding `isSumAchievable` entry is `true`; unreachable cells simply
   retain their default value (`false`/`0`) rather than encoding "0 chips achieves this sum".

   For each denomination and each candidate sum, `FindMinUsableChipCount` tries the **smallest**
   usable chip count first (0 upward), stopping as soon as the remainder is achievable using the
   earlier (already-processed, lower-value) denominations. This is the tie-break that favors
   using fewer of the current (lower-value) denomination and relying on higher-value denominations
   for the rest.

4. **Find the best achievable sum.** `FindBestAchievableSum` searches downward from the (scaled)
   target for the first sum marked achievable using all denominations - preferring an exact match,
   but falling back to the closest sum below it if an exact match isn't possible.

5. **Reconstruct the chip counts.** `ApplyChipCounts` walks the DP table backwards (from the last
   denomination to the first, as required by the reconstruction) to read off how many chips of each
   denomination were used, then emits them as actual player chips via `AddPlayerChip`, in ascending
   order by value.

## Cross-language parity

[PokerChips_Web/PokerChipsSolver.js](../PokerChips_Web/PokerChipsSolver.js) is a pure-JavaScript
port of this exact algorithm (no DOM dependency), used by the web version of the app and verified
independently via [PokerChips_Web/verify-solver.js](../PokerChips_Web/verify-solver.js). Both
implementations are expected to produce identical results for the same inputs; see
[TestProject1/ChipCalculatorTests.cs](../TestProject1/ChipCalculatorTests.cs) for the C# test suite
covering both.
