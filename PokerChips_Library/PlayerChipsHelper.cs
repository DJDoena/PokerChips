namespace DoenaSoft.PokerChips;

public static class PlayerChipsHelper
{
    public static int CreatePlayerChips(List<Chip> playerChips
        , List<Chip> caseChips
        , int maxChipsPerValue
        , int amountPlayers
        , int targetValue)
    {
        var remainingValue = targetValue;

        var chipCalculator = new ChipCalculator(playerChips, maxChipsPerValue, amountPlayers);

        for (var index = 0; index < caseChips.Count; index++)
        {
            var currentCaseChip = caseChips[index];

            var nextCaseChip = (index < caseChips.Count - 1)
                ? caseChips[index + 1] 
                : null;

            var (isDone, updatedRemainingValue) = chipCalculator.AddPlayerChip(currentCaseChip, nextCaseChip, remainingValue);

            remainingValue = updatedRemainingValue;

            if (isDone)
            {
                break;
            }
        }

        return remainingValue;
    }
}
