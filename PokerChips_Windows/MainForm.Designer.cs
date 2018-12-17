namespace DoenaSoft.PokerChips
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SumUpDown = new System.Windows.Forms.NumericUpDown();
            this.PlayersUpDown = new System.Windows.Forms.NumericUpDown();
            this.MaxChipsUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.MainMenu = new System.Windows.Forms.MainMenu();
            this.LoadMenu = new System.Windows.Forms.MenuItem();
            this.OneNormal500Menu = new System.Windows.Forms.MenuItem();
            this.OneFull500Menu = new System.Windows.Forms.MenuItem();
            this.Two500Menu = new System.Windows.Forms.MenuItem();
            this.CalculateMenu = new System.Windows.Forms.MenuItem();
            //((System.ComponentModel.ISupportInitialize)(this.SummeUpDown)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(this.SpielerUpDown)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(this.MaxChipsUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 23;
            this.label1.Text = "Players:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 22;
            this.label2.Text = "Chip Count:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(137, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.TabIndex = 21;
            this.label3.Text = "Chip Value:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(3, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 20);
            this.label4.TabIndex = 20;
            this.label4.Text = "Stack Size:";
            // 
            // SummeUpDown
            // 
            this.SumUpDown.Location = new System.Drawing.Point(137, 31);
            this.SumUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.SumUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.SumUpDown.Name = "SummeUpDown";
            this.SumUpDown.Size = new System.Drawing.Size(100, 20);
            this.SumUpDown.TabIndex = 9;
            this.SumUpDown.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // SpielerUpDown
            // 
            this.PlayersUpDown.Location = new System.Drawing.Point(137, 3);
            this.PlayersUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.PlayersUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.PlayersUpDown.Name = "SpielerUpDown";
            this.PlayersUpDown.Size = new System.Drawing.Size(100, 20);
            this.PlayersUpDown.TabIndex = 14;
            this.PlayersUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // MaxChipsUpDown
            // 
            this.MaxChipsUpDown.Location = new System.Drawing.Point(137, 59);
            this.MaxChipsUpDown.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.MaxChipsUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.MaxChipsUpDown.Name = "MaxChipsUpDown";
            this.MaxChipsUpDown.Size = new System.Drawing.Size(100, 20);
            this.MaxChipsUpDown.TabIndex = 19;
            this.MaxChipsUpDown.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "Max. Chips/Value:";
            // 
            // MainMenu
            // 
            this.MainMenu.MenuItems.Add(this.LoadMenu);
            this.MainMenu.MenuItems.Add(this.CalculateMenu);
            // 
            // LadeMenu
            // 

            this.LoadMenu.MenuItems.Add(this.OneNormal500Menu);
            this.LoadMenu.MenuItems.Add(this.OneFull500Menu);
            this.LoadMenu.MenuItems.Add(this.Two500Menu);
            this.LoadMenu.Text = "Load Case";
            // 
            // Ein500erNormalMenu
            // 
            this.OneNormal500Menu.Text = "One 500 (normal)";
            this.OneNormal500Menu.Click += new System.EventHandler(this.OnOneNormal500Click);
            // 
            // Ein500erVollMenu
            // 
            this.OneFull500Menu.Text = "One 500 (full)";
            this.OneFull500Menu.Click += new System.EventHandler(this.OnOneFull500Click);
            // 
            // Zwei500erMenu
            // 
            this.Two500Menu.Text = "Two 500s (full)";
            this.Two500Menu.Click += new System.EventHandler(this.OnTwo500Click);
            // 
            // BerechneMenu
            // 
            this.CalculateMenu.Text = "Calculate";
            this.CalculateMenu.Click += new System.EventHandler(this.OnBerechneMenuClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.MaxChipsUpDown);
            this.Controls.Add(this.PlayersUpDown);
            this.Controls.Add(this.SumUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Menu = this.MainMenu;
            this.Name = "MainForm";
            this.Text = "Poker Chips";
            //((System.ComponentModel.ISupportInitialize)(this.SummeUpDown)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.SpielerUpDown)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.MaxChipsUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown SumUpDown;
        private System.Windows.Forms.NumericUpDown PlayersUpDown;
        private System.Windows.Forms.NumericUpDown MaxChipsUpDown;
        private System.Windows.Forms.MainMenu MainMenu;
        private System.Windows.Forms.MenuItem LoadMenu;
        private System.Windows.Forms.MenuItem OneNormal500Menu;
        private System.Windows.Forms.MenuItem OneFull500Menu;
        private System.Windows.Forms.MenuItem Two500Menu;
        private System.Windows.Forms.MenuItem CalculateMenu;
    }
}

