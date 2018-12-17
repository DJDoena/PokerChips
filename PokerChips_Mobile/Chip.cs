using System;

namespace PokerChips
{
    internal class Chip
    {
        internal Int32 Wert;
        internal Int32 Anzahl;

        internal Chip(Int32 wert, Int32 anzahl)
        {
            this.Wert = wert;
            this.Anzahl = anzahl;
        }
    }
}
