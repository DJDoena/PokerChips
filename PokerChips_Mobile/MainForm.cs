using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PokerChips
{
    internal partial class MainForm : Form
    {
        private const Int32 ChipColors = 5;

        private ComboBox[] AnzahlComboBoxes;
        private ComboBox[] WertComboBoxes;

        public MainForm()
        {
            InitializeComponent();
            this.AnzahlComboBoxes = new ComboBox[ChipColors];
            this.WertComboBoxes = new ComboBox[ChipColors];
            for(Int32 i = 0; i < ChipColors; i++)
            {
                this.AnzahlComboBoxes[i] = new ComboBox();
                this.AnzahlComboBoxes[i].DropDownStyle = ComboBoxStyle.DropDownList;
                this.AnzahlComboBoxes[i].Items.Add("");
                this.AnzahlComboBoxes[i].Items.Add("50");
                this.AnzahlComboBoxes[i].Items.Add("100");
                this.AnzahlComboBoxes[i].Items.Add("150");
                this.AnzahlComboBoxes[i].Items.Add("200");
                this.AnzahlComboBoxes[i].Items.Add("250");
                this.AnzahlComboBoxes[i].Items.Add("300");
                this.AnzahlComboBoxes[i].Location = new System.Drawing.Point(3, 107 + i * 28);
                this.AnzahlComboBoxes[i].Name = "AnzahlComboBox" + i.ToString();
                this.AnzahlComboBoxes[i].Size = new System.Drawing.Size(100, 22);
                this.Controls.Add(this.AnzahlComboBoxes[i]);
                this.WertComboBoxes[i] = new ComboBox();
                this.WertComboBoxes[i].DropDownStyle = ComboBoxStyle.DropDownList;
                this.WertComboBoxes[i].Items.Add("");
                this.WertComboBoxes[i].Items.Add("25");
                this.WertComboBoxes[i].Items.Add("50");
                this.WertComboBoxes[i].Items.Add("100");
                this.WertComboBoxes[i].Items.Add("200");
                this.WertComboBoxes[i].Items.Add("500");
                this.WertComboBoxes[i].Items.Add("1000");
                this.WertComboBoxes[i].Location = new System.Drawing.Point(137, 107 + i * 28);
                this.WertComboBoxes[i].Name = "AnzahlComboBox" + i.ToString();
                this.WertComboBoxes[i].Size = new System.Drawing.Size(100, 22);
                this.Controls.Add(this.WertComboBoxes[i]);
            }
        }

        private static void CreateEndChip(Chip anfangsChip, List<Chip> endChips, Int32 anzahl, ref Int32 restSumme)
        {
            Chip chip;

            chip = new Chip(anfangsChip.Wert, anzahl);
            endChips.Add(chip);
            restSumme -= (chip.Anzahl * chip.Wert);
        }

        private List<Chip> CheckInput()
        {
            List<Chip> chips;

            chips = new List<Chip>(ChipColors);
            for(Int32 i = 0; i < ChipColors; i++)
            {
                if(((String.IsNullOrEmpty(this.AnzahlComboBoxes[i].Text) == false)
                    && (String.IsNullOrEmpty(this.WertComboBoxes[i].Text)))
                    || (String.IsNullOrEmpty(this.AnzahlComboBoxes[i].Text))
                    && (String.IsNullOrEmpty(this.WertComboBoxes[i].Text) == false))
                {
                    MessageBox.Show(String.Format("In row {0} only one value was selected!", i + 1), "Error"
                        , MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return (null);
                }
                if(this.AnzahlComboBoxes[i].SelectedIndex != -1 && this.WertComboBoxes[i].Text != String.Empty)
                {
                    Chip chip;

                    chip = new Chip(Int32.Parse(this.WertComboBoxes[i].Text)
                        , Int32.Parse(this.AnzahlComboBoxes[i].Text));
                    chips.Add(chip);
                }
            }
            return (chips);
        }

        private static Int32 CompareChips(Chip left, Chip right)
        {
            return (left.Wert.CompareTo(right.Wert));
        }

        private void OnEin500erVollMenuClick(Object sender, EventArgs e)
        {
            this.AnzahlComboBoxes[0].SelectedIndex = 3;
            this.AnzahlComboBoxes[1].SelectedIndex = 3;
            this.AnzahlComboBoxes[2].SelectedIndex = 2;
            this.AnzahlComboBoxes[3].SelectedIndex = 1;
            this.AnzahlComboBoxes[4].SelectedIndex = 1;
            this.WertComboBoxes[0].SelectedIndex = 1;
            this.WertComboBoxes[1].SelectedIndex = 3;
            this.WertComboBoxes[2].SelectedIndex = 4;
            this.WertComboBoxes[3].SelectedIndex = 5;
            this.WertComboBoxes[4].SelectedIndex = 6;
        }

        private void OnZwei500erMenuClick(Object sender, EventArgs e)
        {
            this.AnzahlComboBoxes[0].SelectedIndex = 6;
            this.AnzahlComboBoxes[1].SelectedIndex = 6;
            this.AnzahlComboBoxes[2].SelectedIndex = 4;
            this.AnzahlComboBoxes[3].SelectedIndex = 2;
            this.AnzahlComboBoxes[4].SelectedIndex = 2;
            this.WertComboBoxes[0].SelectedIndex = 1;
            this.WertComboBoxes[1].SelectedIndex = 3;
            this.WertComboBoxes[2].SelectedIndex = 4;
            this.WertComboBoxes[3].SelectedIndex = 5;
            this.WertComboBoxes[4].SelectedIndex = 6;
        }

        private void OnBerechneMenuClick(Object sender, EventArgs e)
        {
            List<Chip> anfangsChips;

            anfangsChips = this.CheckInput();
            if(anfangsChips != null)
            {
                Int32 restSumme;
                List<Chip> endChips;

                endChips = new List<Chip>(ChipColors);
                restSumme = Convert.ToInt32(this.SummeUpDown.Value);
                anfangsChips.Sort(new Comparison<Chip>(CompareChips));
                for(int i = 0; i < anfangsChips.Count; i++)
                {
                    Int32 anzahl;

                    anzahl = anfangsChips[i].Anzahl / Convert.ToInt32(this.SpielerUpDown.Value);
                    if(anzahl > Convert.ToInt32(this.MaxChipsUpDown.Value))
                    {
                        anzahl = Convert.ToInt32(this.MaxChipsUpDown.Value);
                    }
                    if((anzahl * anfangsChips[i].Wert) >= restSumme)
                    {
                        anzahl = restSumme / anfangsChips[i].Wert;
                        CreateEndChip(anfangsChips[i], endChips, anzahl, ref restSumme);
                        break;
                    }
                    else
                    {
                        Int32 tempRestSumme;

                        tempRestSumme = restSumme - (anzahl * anfangsChips[i].Wert);
                        if(i < anfangsChips.Count - 1)
                        {
                            if((tempRestSumme % anfangsChips[i + 1].Wert) != 0)
                            {
                                for(anzahl -= 1; anzahl > 0; anzahl--)
                                {
                                    tempRestSumme = restSumme - (anzahl * anfangsChips[i].Wert);
                                    if((tempRestSumme % anfangsChips[i + 1].Wert) == 0)
                                    {
                                        CreateEndChip(anfangsChips[i], endChips, anzahl, ref restSumme);
                                        break;
                                    }
                                }
                                continue;
                            }
                            else
                            {
                                CreateEndChip(anfangsChips[i], endChips, anzahl, ref restSumme);
                                continue;
                            }
                        }
                        else
                        {
                            CreateEndChip(anfangsChips[i], endChips, anzahl, ref restSumme);
                            break;
                        }
                    }
                }
                if(restSumme != 0)
                {
                    MessageBox.Show("Number of chips + value of chips is insufficient for these players!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation
                        , MessageBoxDefaultButton.Button1);
                }
                else
                {
                    using(ResultForm resultForm = new ResultForm(endChips))
                    {
                        resultForm.ShowDialog();
                    }
                }
            }
        }

        private void OnEin500erNormalMenuClick(Object sender, EventArgs e)
        {
            this.AnzahlComboBoxes[0].SelectedIndex = 3;
            this.AnzahlComboBoxes[1].SelectedIndex = 3;
            this.AnzahlComboBoxes[2].SelectedIndex = 2;
            this.AnzahlComboBoxes[3].SelectedIndex = -1;
            this.AnzahlComboBoxes[4].SelectedIndex = -1;
            this.WertComboBoxes[0].SelectedIndex = 1;
            this.WertComboBoxes[1].SelectedIndex = 3;
            this.WertComboBoxes[2].SelectedIndex = 4;
            this.WertComboBoxes[3].SelectedIndex = -1;
            this.WertComboBoxes[4].SelectedIndex = -1;
        }
    }
}