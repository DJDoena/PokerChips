namespace DoenaSoft.PokerChips;

internal sealed class ChipCalculator
{
    private readonly List<Chip> _playerChips;

    private readonly int _maxChips;

    private readonly int _amountPlayers;

    private readonly List<int> _denominationValues = new();

    private readonly List<int> _denominationCaps = new();

    internal ChipCalculator(List<Chip> playerChips, int maxChips, int amountPlayers)
    {
        _playerChips = playerChips;
        _maxChips = maxChips;
        _amountPlayers = amountPlayers;
    }

    /// <summary>
    /// Buffers the denomination (value and max usable count) of <paramref name="currentCaseChip"/>.
    /// Once the last case chip has been buffered (<paramref name="nextCaseChip"/> is null), all
    /// buffered denominations are handed to the solver to compute the actual player chips at once.
    /// </summary>
    internal bool AddPlayerChip(Chip currentCaseChip, Chip nextCaseChip, ref int remainingValue)
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

        if (chipAmount > _maxChips)
        {
            chipAmount = _maxChips;
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

        var denominationCount = _denominationValues.Count;

        // The chip-count-maximizing tie-break below only favors fewer, higher-value chips overall
        // if the highest-value denominations are considered first. Process a descending-by-value
        // copy of the buffered denominations here, independent of the order chips were fed in
        // (e.g. MainForm feeds them sorted ascending by value).
        var denominationOrder = new int[denominationCount];

        for (var index = 0; index < denominationCount; index++)
        {
            denominationOrder[index] = index;
        }

        Array.Sort(denominationOrder, (left, right) => _denominationValues[right].CompareTo(_denominationValues[left]));

        // isSumAchievable[denominationIndex, sum] is true if "sum" can be built exactly using only
        // the first "denominationIndex" denominations (in descending-value order), each within its own cap.
        var isSumAchievable = new bool[denominationCount + 1, targetValue + 1];

        // chipCountUsedForSum[denominationIndex, sum] stores how many chips of the denomination at
        // (denominationIndex - 1) were used to achieve "sum", so the choice can be reconstructed afterwards.
        var chipCountUsedForSum = new int[denominationCount + 1, targetValue + 1];

        // Base case: a sum of 0 is always achievable using zero denominations.
        isSumAchievable[0, 0] = true;

        for (var denominationIndex = 1; denominationIndex <= denominationCount; denominationIndex++)
        {
            var originalIndex = denominationOrder[denominationIndex - 1];

            var chipValue = _denominationValues[originalIndex];

            var chipCountCap = _denominationCaps[originalIndex];

            for (var candidateSum = 0; candidateSum <= targetValue; candidateSum++)
            {
                // Try using as many chips of this denomination as possible first, so that when a
                // valid combination is found it favors fewer, higher-value chips overall (since
                // higher-value denominations are considered first, in descending order).
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
                        isSumAchievable[denominationIndex, candidateSum] = true;

                        chipCountUsedForSum[denominationIndex, candidateSum] = chipCount;

                        break;
                    }
                }
            }
        }

        var bestAchievableSum = FindBestAchievableSum(isSumAchievable, denominationCount, targetValue);

        if (bestAchievableSum < 0)
        {
            // No combination of chips (not even zero of everything) matched, which should not
            // normally happen since a sum of 0 is always achievable.
            return;
        }

        // Walk the denominations backwards (i.e. from lowest to highest value, the reverse of the
        // descending order used above), reading off how many chips of each were used to reach
        // bestAchievableSum, and materialize them as actual player chips.
        var valueLeftToAllocate = bestAchievableSum;

        for (var denominationIndex = denominationCount; denominationIndex >= 1; denominationIndex--)
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
    private static int FindBestAchievableSum(bool[,] isSumAchievable, int denominationCount, int targetValue)
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

    private void AddPlayerChip(int chipAmount, int chipValue, ref int remainingValue)
    {
        _playerChips.Add(new Chip(chipAmount, chipValue));

        remainingValue -= (chipAmount * chipValue);
    }
}