using System.Diagnostics;

namespace DoenaSoft.PokerChips;

[DebuggerDisplay("{Amount} chips x {Value} = {Amount * Value}")]
public sealed class Chip
{
    public int Amount { get; }

    public int Value { get; }

    public Chip(int amount, int value)
    {
        this.Amount = amount;
        this.Value = value;
    }
}
