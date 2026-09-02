namespace DoenaSoft.PokerChips;

internal sealed class ChipCalculator
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
    /// Buffers the denomination (value and max usable count) of <paramref name="currentCaseChip"/>.
    /// Once the last case chip has been buffered (<paramref name="nextCaseChip"/> is null), all
    /// buffered denominations are handed to the solver to compute the actual player chips at once.
    /// </summary>
    internal bool AddPlayerChip(Chip currentCaseChip
        , Chip nextCaseChip
        , ref int remainingValue)
    {
        var chipValue = currentCaseChip.Value;

        var chipCountCap = this.GetMaxAmount(currentCaseChip.Amount);

        _denominationValues.Add(chipValue);
        _denominationCaps.Add(chipCountCap);

        if (nextCaseChip != null)
        {
            return false;
        }

        this.Solve(ref remainingValue);

        return true;
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
    private void Solve(ref int remainingValue)
    {
        var targetValue = remainingValue;

        var denominationOrder = this.GetDenominationOrderDescendingByValue();

        var (isSumAchievable, chipCountUsedForSum) = this.BuildBoundedKnapsackTable(denominationOrder, targetValue);

        var bestAchievableSum = FindBestAchievableSum(isSumAchievable, denominationOrder.Length, targetValue);

        if (bestAchievableSum < 0)
        {
            // No combination of chips (not even zero of everything) matched, which should not
            // normally happen since a sum of 0 is always achievable.
            return;
        }

        this.ApplyChipCounts(denominationOrder, chipCountUsedForSum, bestAchievableSum, ref remainingValue);
    }

    /// <summary>
    /// Builds an index permutation over the buffered denominations, ordered from highest to lowest
    /// value. The chip-count-maximizing tie-break in <see cref="FindMaxUsableChipCount"/> only favors
    /// fewer, higher-value chips overall if the highest-value denominations are considered first, so
    /// this is independent of the order the case chips were originally fed in via
    /// <see cref="AddPlayerChip"/> (e.g. MainForm feeds them sorted ascending by value).
    /// </summary>
    private int[] GetDenominationOrderDescendingByValue()
    {
        var denominationCount = _denominationValues.Count;

        var denominationOrder = new int[denominationCount];

        for (var index = 0; index < denominationCount; index++)
        {
            denominationOrder[index] = index;
        }

        Array.Sort(denominationOrder, (left, right) => _denominationValues[right].CompareTo(_denominationValues[left]));

        return denominationOrder;
    }

    /// <summary>
    /// Runs the bounded-knapsack dynamic-programming pass over the denominations (in the given
    /// order), producing two tables:
    /// - isSumAchievable[denominationIndex, sum]: whether "sum" can be built exactly using only the
    ///   first "denominationIndex" denominations (each within its own cap).
    /// - chipCountUsedForSum[denominationIndex, sum]: how many chips of the denomination at
    ///   (denominationIndex - 1) were used to achieve "sum", so the choice can be reconstructed
    ///   afterwards by <see cref="ApplyChipCounts"/>.
    /// </summary>
    private (bool[,] IsSumAchievable, int[,] ChipCountUsedForSum) BuildBoundedKnapsackTable(int[] denominationOrder
        , int targetValue)
    {
        var denominationCount = denominationOrder.Length;

        var isSumAchievable = new bool[denominationCount + 1, targetValue + 1];

        var chipCountUsedForSum = new int[denominationCount + 1, targetValue + 1];

        // Base case: a sum of 0 is always achievable using zero denominations.
        isSumAchievable[0, 0] = true;

        for (var denominationIndex = 1; denominationIndex <= denominationCount; denominationIndex++)
        {
            var originalIndex = denominationOrder[denominationIndex - 1];

            var chipValue = _denominationValues[originalIndex];

            var chipCountCap = _denominationCaps[originalIndex];

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
            var chipCount = FindMaxUsableChipCount(isSumAchievable, denominationIndex, chipValue, chipCountCap, candidateSum);

            if (chipCount < 0)
            {
                continue;
            }

            isSumAchievable[denominationIndex, candidateSum] = true;

            chipCountUsedForSum[denominationIndex, candidateSum] = chipCount;
        }
    }

    /// <summary>
    /// Finds the largest chip count (at most chipCountCap) of the current denomination that still
    /// allows candidateSum to be reached, given what the earlier (already processed) denominations
    /// can achieve. Trying the largest count first favors fewer, higher-value chips overall (since
    /// higher-value denominations are considered first, in descending order). Returns -1 if no
    /// count works.
    /// </summary>
    private static int FindMaxUsableChipCount(bool[,] isSumAchievable
        , int denominationIndex
        , int chipValue
        , int chipCountCap
        , int candidateSum)
    {
        for (var chipCount = chipCountCap; chipCount >= 0; chipCount--)
        {
            var valueFromThisDenomination = chipCount * chipValue;

            if (valueFromThisDenomination > candidateSum)
            {
                continue;
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
    /// Walks the denominations backwards (i.e. from lowest to highest value, the reverse of the
    /// descending order used to build the table), reading off how many chips of each were used to
    /// reach bestAchievableSum, and materializes them as actual player chips.
    /// </summary>
    private void ApplyChipCounts(int[] denominationOrder
        , int[,] chipCountUsedForSum
        , int bestAchievableSum
        , ref int remainingValue)
    {
        var valueLeftToAllocate = bestAchievableSum;

        for (var denominationIndex = denominationOrder.Length; denominationIndex >= 1; denominationIndex--)
        {
            var chipCount = chipCountUsedForSum[denominationIndex, valueLeftToAllocate];

            var originalIndex = denominationOrder[denominationIndex - 1];

            var chipValue = _denominationValues[originalIndex];

            if (chipCount > 0)
            {
                this.AddPlayerChip(chipCount, chipValue, ref remainingValue);
            }

            valueLeftToAllocate -= (chipCount * chipValue);
        }
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

    private void AddPlayerChip(int chipAmount
        , int chipValue
        , ref int remainingValue)
    {
        _playerChips.Add(new Chip(chipAmount, chipValue));

        remainingValue -= (chipAmount * chipValue);
    }
}