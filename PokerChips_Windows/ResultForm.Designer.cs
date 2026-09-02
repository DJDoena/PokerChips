namespace DoenaSoft.PokerChips
{
    partial class ResultForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.ValueHeadlineLabel = new System.Windows.Forms.Label();
            this.AmountHeadLineLabel = new System.Windows.Forms.Label();
            this.TotalHeadlineLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ValueHeadlineLabel
            // 
            this.ValueHeadlineLabel.Location = new System.Drawing.Point(171, 6);
            this.ValueHeadlineLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.ValueHeadlineLabel.Name = "ValueHeadlineLabel";
            this.ValueHeadlineLabel.Size = new System.Drawing.Size(125, 25);
            this.ValueHeadlineLabel.TabIndex = 1;
            this.ValueHeadlineLabel.Text = "Chip Value:";
            // 
            // AmountHeadLineLabel
            // 
            this.AmountHeadLineLabel.Location = new System.Drawing.Point(4, 6);
            this.AmountHeadLineLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.AmountHeadLineLabel.Name = "AmountHeadLineLabel";
            this.AmountHeadLineLabel.Size = new System.Drawing.Size(158, 25);
            this.AmountHeadLineLabel.TabIndex = 0;
            this.AmountHeadLineLabel.Text = "Number of Chips:";
            // 
            // TotalHeadlineLabel
            // 
            this.TotalHeadlineLabel.Location = new System.Drawing.Point(339, 6);
            this.TotalHeadlineLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.TotalHeadlineLabel.Name = "TotalHeadlineLabel";
            this.TotalHeadlineLabel.Size = new System.Drawing.Size(125, 25);
            this.TotalHeadlineLabel.TabIndex = 2;
            this.TotalHeadlineLabel.Text = "Total Value:";
            // 
            // ResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(606, 452);
            this.Controls.Add(this.ValueHeadlineLabel);
            this.Controls.Add(this.AmountHeadLineLabel);
            this.Controls.Add(this.TotalHeadlineLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.Name = "ResultForm";
            this.Text = "Poker Chips Result";
            this.ResumeLayout(false);

        }

        #endregion

        private Label ValueHeadlineLabel;
        private Label AmountHeadLineLabel;
        private Label TotalHeadlineLabel;
    }
}