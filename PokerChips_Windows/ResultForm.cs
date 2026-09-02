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
        => this.AddLabel(index, AmountHeadLineLabel.Location.X, "AmountLabel", chip.Amount);

    private void AddValueLabel(Chip chip, int index)
        => this.AddLabel(index, ValueHeadlineLabel.Location.X, "ValueLabel", chip.Value);

    private void AddTotalLabel(Chip chip, int index)
        => this.AddLabel(index, TotalHeadlineLabel.Location.X, "TotalLabel", chip.Amount * chip.Value);

    private void AddGrandTotalLabel(int grandTotal, int chipCount)
    {
        var label = this.AddLabel(chipCount, TotalHeadlineLabel.Location.X, "GrandTotalLabel", grandTotal);

        label.Font = new Font(this.Font, FontStyle.Bold);
    }

    private Label AddLabel(int index
        , int x
        , string name
        , int text)
    {
        var y = 25 + index * 20;

        var label = new Label()
        {
            Location = new Point(x, y),
            Name = $"{name}{index}",
            Size = new Size(100, 22),
            Text = text.ToString()
        };

        this.Controls.Add(label);

        return label;
    }
}