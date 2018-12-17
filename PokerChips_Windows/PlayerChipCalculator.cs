namespace DoenaSoft.PokerChips
{
    using System.Collections.Generic;

    internal static class PlayerChipCalculator
    {
        private static List<Chip> _playerChips;

        internal static bool AddPlayerChip(List<Chip> playerChips, Chip caseChip, int maxChips, int amountPlayers, Chip nextCaseChip, ref int remainingValue)
        {
            _playerChips = playerChips;

            var amountChips = caseChip.Amount / amountPlayers;

            if (amountChips > maxChips)
            {
                amountChips = maxChips;
            }

            var chipValue = caseChip.Value;

            if (IsFinalChip(amountChips, chipValue, remainingValue))
            {
                amountChips = remainingValue / chipValue;

                CreatePlayerChip(amountChips, chipValue, ref remainingValue);

                return true;
            }

            if (nextCaseChip == null)
            {
                CreatePlayerChip(amountChips, chipValue, ref remainingValue);

                return true;
            }

            var finished = CreatePlayerChipWithConvenientValueForNextChip(amountChips, chipValue, nextCaseChip.Value, ref remainingValue);

            return finished;
        }

        private static bool CreatePlayerChipWithConvenientValueForNextChip(int amount, int currentChipValue, int nextChipValue, ref int remainingValue)
        {
            var tempRemainingValue = remainingValue - (amount * currentChipValue);

            if (NextChipIsDisibleWithoutRest(nextChipValue, tempRemainingValue))
            {
                CreatePlayerChip(amount, currentChipValue, ref remainingValue);

                return remainingValue == 0;
            }

            for (amount -= 1; amount > 0; amount--)
            {
                tempRemainingValue = remainingValue - (amount * currentChipValue);

                if (NextChipIsDisibleWithoutRest(nextChipValue, tempRemainingValue))
                {
                    CreatePlayerChip(amount, currentChipValue, ref remainingValue);

                    return remainingValue == 0;
                }
            }

            return remainingValue == 0;
        }

        private static void CreatePlayerChip(int amount, int value, ref int remainingValue)
        {
            var chip = new Chip(amount, value);

            _playerChips.Add(chip);

            remainingValue -= (amount * value);
        }

        private static bool IsFinalChip(int amount, int value, int remainingValue)
            => (amount * value) >= remainingValue;

        private static bool NextChipIsDisibleWithoutRest(int caseChipValue, int remainingValue)
            => (remainingValue % caseChipValue) == 0;
    }
}