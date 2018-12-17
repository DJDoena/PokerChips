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

        internal bool AddPlayerChip(Chip currentCaseChip, Chip nextCaseChip, ref int remainingValue)
        {
            var chipValue = currentCaseChip.Value;

            var amount = GetAmount(currentCaseChip.Amount, chipValue, remainingValue);

            if (IsLastChip(chipValue, nextCaseChip, remainingValue))
            {
                AddPlayerChip(amount, chipValue, ref remainingValue);

                return true;
            }

            var finished = AddPlayerChipWithConvenientValueForNextChip(amount, chipValue, nextCaseChip.Value, ref remainingValue);

            return finished;
        }

        private int GetAmount(int caseChipAmount, int chipValue, int remainingValue)
        {
            var chipAmount = caseChipAmount / _amountPlayers;

            if (chipAmount > _maxChips)
            {
                chipAmount = _maxChips;
            }

            if (ChipsExceedRemainingValue(chipAmount, chipValue, remainingValue))
            {
                chipAmount = remainingValue / chipValue;
            }

            return chipAmount;
        }

        private bool AddPlayerChipWithConvenientValueForNextChip(int chipAmount, int currentChipValue, int nextChipValue, ref int remainingValue)
        {
            for (; chipAmount > 0; chipAmount--)
            {
                var potentialRemainingValue = remainingValue - (chipAmount * currentChipValue);

                if (IsDivisibleWithoutRest(nextChipValue, potentialRemainingValue))
                {
                    AddPlayerChip(chipAmount, currentChipValue, ref remainingValue);

                    return remainingValue == 0;
                }
            }

            return remainingValue == 0;
        }

        private void AddPlayerChip(int chipAmount, int chipValue, ref int remainingValue)
        {
            _playerChips.Add(new Chip(chipAmount, chipValue));

            remainingValue -= (chipAmount * chipValue);
        }

        private static bool IsLastChip(int chipValue, Chip nextCaseChip, int remainingValue)
            => nextCaseChip == null || IsDivisibleWithoutRest(chipValue, remainingValue);

        private static bool ChipsExceedRemainingValue(int chipAmount, int chipValue, int remainingValue)
            => (chipAmount * chipValue) >= remainingValue;

        private static bool IsDivisibleWithoutRest(int chipValue, int remainingValue)
            => (remainingValue % chipValue) == 0;
    }
}