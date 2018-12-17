using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PokerChips
{
    internal partial class ResultForm : Form
    {
        private Label[] AnzahlLabels;
        private Label[] WertLabels;

        public ResultForm(List<Chip> chips)
        {
            InitializeComponent();
            this.AnzahlLabels = new Label[chips.Count];
            this.WertLabels = new Label[chips.Count];
            for(Int32 i = 0; i < chips.Count; i++)
            {
                this.AnzahlLabels[i] = new Label();
                this.AnzahlLabels[i].Location = new System.Drawing.Point(3, 25 + i * 20);
                this.AnzahlLabels[i].Name = "AnzahlLabel" + i.ToString();
                this.AnzahlLabels[i].Size = new System.Drawing.Size(100, 22);
                this.AnzahlLabels[i].Text = chips[i].Anzahl.ToString();
                this.Controls.Add(this.AnzahlLabels[i]);
                this.WertLabels[i] = new Label();
                this.WertLabels[i].Location = new System.Drawing.Point(137, 25 + i * 20);
                this.WertLabels[i].Name = "WertLabel" + i.ToString();
                this.WertLabels[i].Size = new System.Drawing.Size(100, 22);
                this.WertLabels[i].Text = chips[i].Wert.ToString();
                this.Controls.Add(this.WertLabels[i]);
            }
        }
    }
}