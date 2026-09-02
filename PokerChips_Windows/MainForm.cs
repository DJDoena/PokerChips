namespace DoenaSoft.PokerChips;

internal partial class MainForm : Form
{
    private const int ChipColors = 5;

    private readonly List<ComboBox> _amountComboBoxes;

    private readonly List<ComboBox> _valueComboBoxes;

    public MainForm()
    {
        this.InitializeComponent();

        this.Icon = Resource.DJDSOFT;

        _amountComboBoxes = new List<ComboBox>(ChipColors);
        _valueComboBoxes = new List<ComboBox>(ChipColors);

        for (var index = 0; index < ChipColors; index++)
        {
            this.AddAmountComboBox(index);

            this.AddValueComboBox(index);
        }
    }

    private void AddAmountComboBox(int index)
    {
        var amountComboBox = new ComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(3, 107 + index * 28),
            Name = "AmountComboBox" + index.ToString(),
            Size = new Size(100, 22),
        };

        amountComboBox.Items.AddRange([0, 50, 100, 150, 200, 250, 300]);

        this.Controls.Add(amountComboBox);

        _amountComboBoxes.Add(amountComboBox);
    }

    private void AddValueComboBox(int index)
    {
        var valueComboBox = new ComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(137, 107 + index * 28),
            Name = "ValueComboBox" + index.ToString(),
            Size = new Size(100, 22)
        };

        valueComboBox.Items.AddRange([0, 25, 50, 100, 200, 500, 1000]);

        this.Controls.Add(valueComboBox);

        _valueComboBoxes.Add(valueComboBox);
    }

    private void OnCalculateMenuClick(object sender, EventArgs e)
    {
        var caseChips = this.CheckInput();

        if (caseChips == null)
        {
            return;
        }

        caseChips.Sort((left, right) => left.Value.CompareTo(right.Value));

        var playerChips = new List<Chip>(ChipColors);

        var remainingValue = PlayerChipsHelper.CreatePlayerChips(playerChips
            , caseChips
            , Convert.ToInt32(MaxChipsUpDown.Value)
            , Convert.ToInt32(PlayersUpDown.Value)
            , Convert.ToInt32(SumUpDown.Value));

        if (remainingValue != 0)
        {
            MessageBox.Show("Number of chips + value of chips is insufficient for these players!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            return;
        }

        using var resultForm = new ResultForm(playerChips);

        resultForm.ShowDialog();
    }

    #region Check input

    private List<Chip> CheckInput()
    {
        for (var index = 0; index < ChipColors; index++)
        {
            if (this.InputMisMatch(index))
            {
                MessageBox.Show($"In row {index + 1} only one value was selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return null;
            }

            if (this.AmountIsSetButValueIsZero(index))
            {
                MessageBox.Show($"In row {index + 1} an amount was selected without a chip value!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return null;
            }
        }

        var caseChips = new List<Chip>(ChipColors);

        for (var index = 0; index < ChipColors; index++)
        {
            if (_amountComboBoxes[index].SelectedIndex != -1)
            {
                var value = int.Parse(_valueComboBoxes[index].Text);

                var amount = int.Parse(_amountComboBoxes[index].Text);

                var chip = new Chip(amount, value);

                caseChips.Add(chip);
            }
        }

        return caseChips;
    }

    private bool InputMisMatch(int index)
        => this.AmountIsNotEmptyButValueIs(index) || this.ValueIsNotEmptyButAmountIs(index);

    /// <summary>
    /// A selected amount greater than 0 combined with a selected chip value of 0 is not a valid
    /// case chip (see <see cref="ChipCalculator.AddPlayerChip"/>), so this is rejected here before
    /// it ever reaches the solver.
    /// </summary>
    private bool AmountIsSetButValueIsZero(int index)
    {
        var amountComboBox = _amountComboBoxes[index];

        var valueComboBox = _valueComboBoxes[index];

        if (amountComboBox.SelectedIndex == -1 || valueComboBox.SelectedIndex == -1)
        {
            return false;
        }

        var amount = int.Parse(amountComboBox.Text);

        var value = int.Parse(valueComboBox.Text);

        return amount > 0 && value == 0;
    }

    private bool AmountIsNotEmptyButValueIs(int index)
        => LeftIsNotEmptyButRightIs(_amountComboBoxes[index], _valueComboBoxes[index]);

    private bool ValueIsNotEmptyButAmountIs(int index)
        => LeftIsNotEmptyButRightIs(_valueComboBoxes[index], _amountComboBoxes[index]);

    private static bool LeftIsNotEmptyButRightIs(ComboBox left, ComboBox right)
        => !string.IsNullOrEmpty(left.Text) && string.IsNullOrEmpty(right.Text);

    #endregion

    #region Add specific case types

    private void OnOneNormal500Click(object sender, EventArgs e)
    {
        this.SetComboBoxes(0, 3, 1);
        this.SetComboBoxes(1, 3, 3);
        this.SetComboBoxes(2, 2, 4);
        this.SetComboBoxes(3, -1, -1);
        this.SetComboBoxes(4, -1, -1);
    }

    private void OnOneFull500Click(object sender, EventArgs e)
    {
        this.SetComboBoxes(0, 3, 1);
        this.SetComboBoxes(1, 3, 3);
        this.SetComboBoxes(2, 2, 4);
        this.SetComboBoxes(3, 1, 5);
        this.SetComboBoxes(4, 1, 6);
    }

    private void OnTwo500Click(object sender, EventArgs e)
    {
        this.SetComboBoxes(0, 6, 1);
        this.SetComboBoxes(1, 6, 3);
        this.SetComboBoxes(2, 4, 4);
        this.SetComboBoxes(3, 2, 5);
        this.SetComboBoxes(4, 2, 6);
    }

    private void SetComboBoxes(int comboBoxIndex, int amountIndex, int valueIndex)
    {
        _amountComboBoxes[comboBoxIndex].SelectedIndex = amountIndex;
        _valueComboBoxes[comboBoxIndex].SelectedIndex = valueIndex;
    }

    #endregion
}