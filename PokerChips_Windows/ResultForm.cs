namespace DoenaSoft.PokerChips;

internal partial class ResultForm : Form
{
    public ResultForm(List<Chip> chips)
    {
        this.InitializeComponent();

        this.Icon = Resource.DJDSOFT;

        var grandTotal = 0;

        for (var index = 0; index < chips.Count; index++)
        {
            var chip = chips[index];

            this.AddAmountLabel(chip, index);

            this.AddValueLabel(chip, index);

            this.AddTotalLabel(chip, index);

            grandTotal += chip.Amount * chip.Value;
        }

        this.AddGrandTotalLabel(grandTotal, chips.Count);
    }

    private void AddAmountLabel(Chip chip, int index)
    {
        var amountLabel = new Label()
        {
            Location = new Point(3, 25 + index * 20),
            Name = "AmountLabel" + index.ToString(),
            Size = new Size(100, 22),
            Text = chip.Amount.ToString()
        };

        this.Controls.Add(amountLabel);
    }

    private void AddValueLabel(Chip chip, int index)
    {
        var valueLabel = new Label()
        {
            Location = new Point(137, 25 + index * 20),
            Name = "ValueLabel" + index.ToString(),
            Size = new Size(100, 22),
            Text = chip.Value.ToString()
        };

        this.Controls.Add(valueLabel);
    }

    private void AddTotalLabel(Chip chip, int index)
    {
        var totalLabel = new Label()
        {
            Location = new Point(271, 25 + index * 20),
            Name = "TotalLabel" + index.ToString(),
            Size = new Size(100, 22),
            Text = (chip.Amount * chip.Value).ToString()
        };

        this.Controls.Add(totalLabel);
    }

    private void AddGrandTotalLabel(int grandTotal, int chipCount)
    {
        var grandTotalLabel = new Label()
        {
            Font = new Font(this.Font, FontStyle.Bold),
            Location = new Point(271, 25 + chipCount * 20),
            Name = "GrandTotalLabel",
            Size = new Size(100, 22),
            Text = grandTotal.ToString()
        };

        this.Controls.Add(grandTotalLabel);
    }
}