namespace DoenaSoft.PokerChips;

internal static class PlayerChipsHelper
{
    public static int CreatePlayerChips(List<Chip> playerChips
        , List<Chip> caseChips
        , int maxChips
        , int amountPlayers
        , int startingValue)
    {
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
}
