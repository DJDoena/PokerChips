using System.Diagnostics;

namespace DoenaSoft.PokerChips
{
    [DebuggerDisplay("{Amount} chips x {Value} = {Amount * Value}")]
    internal sealed class Chip
    {
        internal int Amount { get; }

        internal int Value { get; }

        internal Chip(int amount, int value)
        {
            Amount = amount;
            Value = value;
        }
    }
}