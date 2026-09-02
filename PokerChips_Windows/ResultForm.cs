namespace DoenaSoft.PokerChips;

internal partial class ResultForm : Form
{
    public ResultForm(List<Chip> chips)
    {
        this.InitializeComponent();

        for (var index = 0; index < chips.Count; index++)
        {
            var chip = chips[index];

            this.AddAmountLabel(chip, index);

            this.AddValueLabel(chip, index);
        }
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
}