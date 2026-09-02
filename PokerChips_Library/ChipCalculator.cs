namespace DoenaSoft.PokerChips;

public sealed class ChipCalculator
{
    private readonly List<Chip> _playerChips;

    private readonly int _maxChipsPerValue;

    private readonly int _amountPlayers;

    private readonly List<int> _denominationValues = [];

    private readonly List<int> _denominationCaps = [];

    internal ChipCalculator(List<Chip> playerChips
        , int maxChipsPerValue
        , int amountPlayers)
    {
        _playerChips = playerChips;
        _maxChipsPerValue = maxChipsPerValue;
        _amountPlayers = amountPlayers;
    }

    /// <summary>
    /// Buffers the denomination (value and max usable count) of <paramref name="currentCaseChip"/>,
    /// unless its value is 0 (a 0-value chip can never contribute to the target sum, so it is
    /// filtered out here instead of being fed into the solver).
    /// Once the last case chip has been buffered (<paramref name="nextCaseChip"/> is null), all
    /// buffered denominations are handed to the solver to compute the actual player chips at once.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="currentCaseChip"/> has an amount greater than 0 but a value of 0,
    /// since that combination cannot be a legitimate case chip (only the UI's unused, empty
    /// color slots are expected to have a value of 0, and those always have an amount of 0 too).
    /// </exception>
    internal (bool isDone, int remainingValue) AddPlayerChip(Chip currentCaseChip
        , Chip nextCaseChip
        , int remainingValue)
    {
        var chipValue = currentCaseChip.Value;

        if (chipValue == 0)
        {
            if (currentCaseChip.Amount > 0)
            {
                throw new ArgumentException("A chip with an amount greater than 0 must not have a value of 0.", nameof(currentCaseChip));
            }
        }
        else
        {
            var chipCountCap = this.GetMaxAmount(currentCaseChip.Amount);

            _denominationValues.Add(chipValue);
            _denominationCaps.Add(chipCountCap);
        }

        if (nextCaseChip != null)
        {
            return (false, remainingValue);
        }

        var solvedRemainingValue = this.Solve(remainingValue);

        return (true, solvedRemainingValue);
    }

    /// <summary>
    /// Determines the maximum number of chips of a denomination that may be handed out per player,
    /// bounded both by how many are available per player and by the configured maximum chip count.
    /// </summary>
    private int GetMaxAmount(int caseChipAmount)
    {
        var chipAmount = caseChipAmount / _amountPlayers;

        if (chipAmount > _maxChipsPerValue)
        {
            chipAmount = _maxChipsPerValue;
        }

        return chipAmount;
    }

    /// <summary>
    /// Solves for the combination of chip counts (one count per denomination gathered so far via
    /// <see cref="AddPlayerChip"/>) that sums up as closely as possible to <paramref name="remainingValue"/>,
    /// without exceeding it, while respecting each denomination's cap.
    /// This is a classic "bounded knapsack" problem: each denomination (coin/chip value) may be used
    /// between 0 and its cap number of times, and we want the achievable sum closest to the target.
    /// </summary>
    private int Solve(int remainingValue)
    {
        var targetValue = remainingValue;

        var (sortedValues, sortedCaps) = this.GetDenominationsSortedAscendingByValue();

        // Every achievable sum is necessarily a multiple of the GCD of all denomination values, so
        // dividing the values (and the target) by that GCD shrinks the DP table without changing
        // which sums are achievable, just their scale.
        // Note that this is a multiple of the GCD of ALL denominations combined, not of any single
        // denomination's value. E.g. with denominations 25/100/200/500/1000 (GCD 25), a sum like
        // 4975 (scaled: 199) is a multiple of 25 but not of 100/200/500/1000 individually - it can
        // still be achievable via a mix of denominations (mostly 25s). Being a multiple of the GCD
        // is a NECESSARY condition for achievability, not a SUFFICIENT one, so isSumAchievable still
        // has to be checked per-cell; not every multiple of the GCD is actually reachable (e.g. if
        // the caps are too restrictive), but everything reachable will be a multiple of it.
        var denominationGcd = GetDenominationGcd(sortedValues);

        var scaledTargetValue = targetValue / denominationGcd;

        var scaledValues = sortedValues.Select(value => value / denominationGcd).ToArray();

        var (isSumAchievable, chipCountUsedForSum) = BuildBoundedKnapsackTable(scaledValues, sortedCaps, scaledTargetValue);

        var bestAchievableScaledSum = FindBestAchievableSum(isSumAchievable, scaledValues.Length, scaledTargetValue);

        if (bestAchievableScaledSum < 0)
        {
            // No combination of chips (not even zero of everything) matched, which should not
            // normally happen since a sum of 0 is always achievable.
            return remainingValue;
        }

        return this.ApplyChipCounts(sortedValues, scaledValues, chipCountUsedForSum, bestAchievableScaledSum, remainingValue);
    }

    /// <summary>
    /// Returns the greatest common divisor shared by all denomination values (or 1 if there are no
    /// denominations), used to shrink the DP table's column count.
    /// </summary>
    private static int GetDenominationGcd(int[] sortedValues)
    {
        if (sortedValues.Length == 0)
        {
            return 1;
        }

        var result = sortedValues[0];

        for (var index = 1; index < sortedValues.Length; index++)
        {
            result = Gcd(result, sortedValues[index]);
        }

        return result;
    }

    /// <summary>
    /// Returns the greatest common divisor (GCD) of two numbers, i.e. the largest number that
    /// divides both <paramref name="left"/> and <paramref name="right"/> without a remainder,
    /// computed via the Euclidean algorithm.
    /// </summary>
    private static int Gcd(int left, int right)
    {
        while (right != 0)
        {
            (left, right) = (right, left % right);
        }

        return left;
    }

    /// <summary>
    /// Returns the buffered denomination values and caps sorted ascending by value (as parallel
    /// arrays). The chip-count-minimizing tie-break in <see cref="FindMinUsableChipCount"/> only
    /// favors fewer, higher-value chips being needed (i.e. prefers using smaller chips) if the
    /// lowest-value denominations are considered first, so this is independent of the order the
    /// case chips were originally fed in via <see cref="AddPlayerChip"/> (e.g. MainForm feeds them
    /// sorted ascending by value).
    /// </summary>
    private (int[] values, int[] caps) GetDenominationsSortedAscendingByValue()
    {
        var sortedValues = _denominationValues.ToArray();

        var sortedCaps = _denominationCaps.ToArray();

        Array.Sort(sortedValues, sortedCaps);

        return (sortedValues, sortedCaps);
    }

    /// <summary>
    /// Runs the bounded-knapsack dynamic-programming pass over the denominations (sorted ascending
    /// by value), producing two tables:
    /// - isSumAchievable[denominationIndex, sum]: whether "sum" can be built exactly using only the
    ///   first "denominationIndex" denominations (each within its own cap).
    /// - chipCountUsedForSum[denominationIndex, sum]: how many chips of the denomination at
    ///   (denominationIndex - 1) were used to achieve "sum", so the choice can be reconstructed
    ///   afterwards by <see cref="ApplyChipCounts"/>.
    /// A chipCountUsedForSum[denominationIndex, sum] entry is only meaningful when
    /// isSumAchievable[denominationIndex, sum] is true; both arrays are allocated as full
    /// rectangular arrays, so unreachable cells simply retain their default value (false/0) rather
    /// than encoding "0 chips achieves this sum".
    /// </summary>
    private static (bool[,] isSumAchievable, int[,] chipCountUsedForSum) BuildBoundedKnapsackTable(int[] sortedValues
        , int[] sortedCaps
        , int targetValue)
    {
        var denominationCount = sortedValues.Length;

        var isSumAchievable = new bool[denominationCount + 1, targetValue + 1];

        var chipCountUsedForSum = new int[denominationCount + 1, targetValue + 1];

        // Base case: a sum of 0 is always achievable using zero denominations.
        isSumAchievable[0, 0] = true;

        for (var denominationIndex = 1; denominationIndex <= denominationCount; denominationIndex++)
        {
            var chipValue = sortedValues[denominationIndex - 1];

            var chipCountCap = sortedCaps[denominationIndex - 1];

            FillAchievableSumsForDenomination(isSumAchievable, chipCountUsedForSum, denominationIndex, chipValue, chipCountCap, targetValue);
        }

        return (isSumAchievable, chipCountUsedForSum);
    }

    /// <summary>
    /// Fills in isSumAchievable/chipCountUsedForSum at row "denominationIndex" for every candidate
    /// sum from 0 to targetValue, given the denomination's value and chip-count cap.
    /// </summary>
    private static void FillAchievableSumsForDenomination(bool[,] isSumAchievable
        , int[,] chipCountUsedForSum
        , int denominationIndex
        , int chipValue
        , int chipCountCap
        , int targetValue)
    {
        for (var candidateSum = 0; candidateSum <= targetValue; candidateSum++)
        {
            var chipCount = FindMinUsableChipCount(isSumAchievable, denominationIndex, chipValue, chipCountCap, candidateSum);

            if (chipCount < 0)
            {
                continue;
            }

            isSumAchievable[denominationIndex, candidateSum] = true;

            chipCountUsedForSum[denominationIndex, candidateSum] = chipCount;
        }
    }

    /// <summary>
    /// Finds the smallest chip count (at most chipCountCap) of the current denomination that still
    /// allows candidateSum to be reached, given what the earlier (already processed) denominations
    /// can achieve. Trying the smallest count first favors using fewer of this (lower-value)
    /// denomination and relying on higher-value denominations for the rest (since denominations are
    /// considered lowest-value first, in ascending order). Returns -1 if no count works.
    /// </summary>
    private static int FindMinUsableChipCount(bool[,] isSumAchievable
        , int denominationIndex
        , int chipValue
        , int chipCountCap
        , int candidateSum)
    {
        for (var chipCount = 0; chipCount <= chipCountCap; chipCount++)
        {
            var valueFromThisDenomination = chipCount * chipValue;

            if (valueFromThisDenomination > candidateSum)
            {
                break;
            }

            var remainingSumForEarlierDenominations = candidateSum - valueFromThisDenomination;

            if (isSumAchievable[denominationIndex - 1, remainingSumForEarlierDenominations])
            {
                return chipCount;
            }
        }

        return -1;
    }

    /// <summary>
    /// Walks the denominations backwards (from the last-processed to the first-processed, as
    /// required by the DP reconstruction) to read off how many chips of each denomination were used
    /// to reach bestAchievableSum, then materializes them as actual player chips in ascending order
    /// by value (matching sortedValues).
    /// </summary>
    private int ApplyChipCounts(int[] sortedValues
        , int[] scaledValues
        , int[,] chipCountUsedForSum
        , int bestAchievableScaledSum
        , int remainingValue)
    {
        var chipCounts = new int[sortedValues.Length];

        var valueLeftToAllocate = bestAchievableScaledSum;

        for (var denominationIndex = sortedValues.Length; denominationIndex >= 1; denominationIndex--)
        {
            var chipCount = chipCountUsedForSum[denominationIndex, valueLeftToAllocate];

            var scaledChipValue = scaledValues[denominationIndex - 1];

            chipCounts[denominationIndex - 1] = chipCount;

            valueLeftToAllocate -= (chipCount * scaledChipValue);
        }

        for (var index = 0; index < sortedValues.Length; index++)
        {
            var chipCount = chipCounts[index];

            if (chipCount > 0)
            {
                remainingValue = this.AddPlayerChip(chipCount, sortedValues[index], remainingValue);
            }
        }

        return remainingValue;
    }

    /// <summary>
    /// Finds the largest sum (at most <paramref name="targetValue"/>) that is achievable using all
    /// gathered denominations, preferring an exact match to <paramref name="targetValue"/> when possible.
    /// </summary>
    private static int FindBestAchievableSum(bool[,] isSumAchievable
        , int denominationCount
        , int targetValue)
    {
        for (var candidateSum = targetValue; candidateSum >= 0; candidateSum--)
        {
            if (isSumAchievable[denominationCount, candidateSum])
            {
                return candidateSum;
            }
        }

        return -1;
    }

    private int AddPlayerChip(int chipAmount
        , int chipValue
        , int remainingValue)
    {
        _playerChips.Add(new Chip(chipAmount, chipValue));

        return remainingValue - (chipAmount * chipValue);
    }
}