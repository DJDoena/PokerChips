namespace DoenaSoft.PokerChips
{
    using System.Collections.Generic;

    internal sealed class ChipCalculator
    {
        private readonly List<Chip> _playerChips;

        private readonly int _maxChips;

        private readonly int _amountPlayers;

        public ChipCalculator(List<Chip> playerChips, int maxChips, int amountPlayers)
        {
            _playerChips = playerChips;
            _maxChips = maxChips;
            _amountPlayers = amountPlayers;
        }

        internal bool AddPlayerChip(Chip caseChip, Chip nextCaseChip, ref int remainingValue)
        {
            var amountChips = caseChip.Amount / _amountPlayers;

            if (amountChips > _maxChips)
            {
                amountChips = _maxChips;
            }

            var chipValue = caseChip.Value;

            if (IsFinalChip(amountChips, chipValue, remainingValue))
            {
                amountChips = remainingValue / chipValue;

                AddPlayerChip(amountChips, chipValue, ref remainingValue);

                return true;
            }

            if (ChipsExceedRemainingValue(amountChips, chipValue, remainingValue))
            {
                amountChips = remainingValue / chipValue;
            }

            if (nextCaseChip == null)
            {
                AddPlayerChip(amountChips, chipValue, ref remainingValue);

                return true;
            }

            var finished = AddPlayerChipWithConvenientValueForNextChip(amountChips, chipValue, nextCaseChip.Value, ref remainingValue);

            return finished;
        }

        private bool AddPlayerChipWithConvenientValueForNextChip(int amount, int currentChipValue, int nextChipValue, ref int remainingValue)
        {
            var tempRemainingValue = remainingValue - (amount * currentChipValue);

            if (NextChipIsDivisibleWithoutRest(nextChipValue, tempRemainingValue))
            {
                AddPlayerChip(amount, currentChipValue, ref remainingValue);

                return remainingValue == 0;
            }

            for (amount -= 1; amount > 0; amount--)
            {
                tempRemainingValue = remainingValue - (amount * currentChipValue);

                if (NextChipIsDivisibleWithoutRest(nextChipValue, tempRemainingValue))
                {
                    AddPlayerChip(amount, currentChipValue, ref remainingValue);

                    return remainingValue == 0;
                }
            }

            return remainingValue == 0;
        }

        private void AddPlayerChip(int amount, int value, ref int remainingValue)
        {
            _playerChips.Add(new Chip(amount, value));

            remainingValue -= (amount * value);
        }

        private static bool IsFinalChip(int amount, int value, int remainingValue)
            => ChipsExceedRemainingValue(amount, value, remainingValue) && NextChipIsDivisibleWithoutRest(value, remainingValue);

        private static bool ChipsExceedRemainingValue(int amount, int value, int remainingValue)
            => (amount * value) >= remainingValue;

        private static bool NextChipIsDivisibleWithoutRest(int caseChipValue, int remainingValue)
            => (remainingValue % caseChipValue) == 0;
    }
}