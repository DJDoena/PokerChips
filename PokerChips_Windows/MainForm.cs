namespace DoenaSoft.PokerChips
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    internal partial class MainForm : Form
    {
        private const int ChipColors = 5;

        private readonly List<ComboBox> _amountComboBoxes;

        private readonly List<ComboBox> _valueComboBoxes;

        public MainForm()
        {
            InitializeComponent();

            _amountComboBoxes = new List<ComboBox>(ChipColors);
            _valueComboBoxes = new List<ComboBox>(ChipColors);

            for (var index = 0; index < ChipColors; index++)
            {
                AddAmountComboBox(index);

                AddValueComboBox(index);
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

            amountComboBox.Items.AddRange(new object[] { 0, 50, 100, 150, 200, 250, 300 });

            Controls.Add(amountComboBox);

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

            valueComboBox.Items.AddRange(new object[] { 0, 25, 50, 100, 200, 500, 1000 });

            Controls.Add(valueComboBox);

            _valueComboBoxes.Add(valueComboBox);
        }

        private void OnBerechneMenuClick(Object sender, EventArgs e)
        {
            var caseChips = CheckInput();

            if (caseChips == null)
            {
                return;
            }

            caseChips.Sort((left, right) => left.Value.CompareTo(right.Value));

            var playerChips = new List<Chip>(ChipColors);

            CreatePlayerChips(playerChips, caseChips, out var remainingValue);

            if (remainingValue != 0)
            {
                MessageBox.Show("Number of chips + value of chips is insufficient for these players!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                return;
            }

            using (var resultForm = new ResultForm(playerChips))
            {
                resultForm.ShowDialog();
            }
        }

        #region Check input

        private List<Chip> CheckInput()
        {
            for (var index = 0; index < ChipColors; index++)
            {
                if (InputMisMatch(index))
                {
                    MessageBox.Show($"In row {index + 1} only one value was selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

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
            => AmountIsNotEmptyButValueIs(index) || ValueIsNotEmptyButAmountIs(index);

        private bool AmountIsNotEmptyButValueIs(int index)
            => LeftIsNotEmptyButRightIs(_amountComboBoxes[index], _valueComboBoxes[index]);

        private bool ValueIsNotEmptyButAmountIs(int index)
            => LeftIsNotEmptyButRightIs(_valueComboBoxes[index], _amountComboBoxes[index]);

        private static bool LeftIsNotEmptyButRightIs(ComboBox left, ComboBox right)
            => !string.IsNullOrEmpty(left.Text) && string.IsNullOrEmpty(right.Text);

        #endregion

        private void CreatePlayerChips(List<Chip> playerChips, List<Chip> caseChips, out int remainingValue)
        {
            remainingValue = Convert.ToInt32(SumUpDown.Value);

            var chipCalculator = new ChipCalculator(playerChips, Convert.ToInt32(MaxChipsUpDown.Value), Convert.ToInt32(PlayersUpDown.Value));

            for (int index = 0; index < caseChips.Count; index++)
            {
                var caseChip = caseChips[index];

                var nextCaseChip = (index < caseChips.Count - 1) ? caseChips[index + 1] : null;

                if (chipCalculator.AddPlayerChip(caseChip, nextCaseChip, ref remainingValue))
                {
                    break;
                }
            }
        }

        #region Add specific case types

        private void OnOneNormal500Click(object sender, EventArgs e)
        {
            SetComboBoxes(0, 3, 1);
            SetComboBoxes(1, 3, 3);
            SetComboBoxes(2, 2, 4);
            SetComboBoxes(3, -1, -1);
            SetComboBoxes(4, -1, -1);
        }

        private void OnOneFull500Click(object sender, EventArgs e)
        {
            SetComboBoxes(0, 3, 1);
            SetComboBoxes(1, 3, 3);
            SetComboBoxes(2, 2, 4);
            SetComboBoxes(3, 1, 5);
            SetComboBoxes(4, 1, 6);
        }

        private void OnTwo500Click(object sender, EventArgs e)
        {
            SetComboBoxes(0, 6, 1);
            SetComboBoxes(1, 6, 3);
            SetComboBoxes(2, 4, 4);
            SetComboBoxes(3, 2, 5);
            SetComboBoxes(4, 2, 6);
        }

        private void SetComboBoxes(int comboBoxIndex, int amountIndex, int valueIndex)
        {
            _amountComboBoxes[comboBoxIndex].SelectedIndex = amountIndex;
            _valueComboBoxes[comboBoxIndex].SelectedIndex = valueIndex;
        }

        #endregion
    }
}