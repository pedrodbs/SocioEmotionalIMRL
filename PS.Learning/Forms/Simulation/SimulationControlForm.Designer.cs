using System.Windows.Forms;

namespace PS.Learning.Forms.Simulation
{
    partial class SimulationControlForm
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
            this.debugCheckBox = new System.Windows.Forms.CheckBox();
            this.timerTrackBar = new System.Windows.Forms.TrackBar();
            this.label15 = new System.Windows.Forms.Label();
            this.printResCheckBox = new System.Windows.Forms.CheckBox();
            this.advanceNumUD = new System.Windows.Forms.NumericUpDown();
            this.advanceBtn = new System.Windows.Forms.Button();
            this.printWorldBtn = new System.Windows.Forms.Button();
            this.stepBtn = new System.Windows.Forms.Button();
            this.pauseBtn = new System.Windows.Forms.Button();
            this.resetBtn = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.controlAgComboBox = new System.Windows.Forms.ComboBox();
            this.showAgInfoCBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.timerTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.advanceNumUD)).BeginInit();
            this.SuspendLayout();
            // 
            // debugCheckBox
            // 
            this.debugCheckBox.AutoSize = true;
            this.debugCheckBox.Location = new System.Drawing.Point(18, 69);
            this.debugCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.debugCheckBox.Name = "debugCheckBox";
            this.debugCheckBox.Size = new System.Drawing.Size(105, 21);
            this.debugCheckBox.TabIndex = 20;
            this.debugCheckBox.Text = "Debug Mode";
            this.debugCheckBox.UseVisualStyleBackColor = true;
            this.debugCheckBox.CheckedChanged += new System.EventHandler(this.DebugCheckBoxCheckedChanged);
            // 
            // timerTrackBar
            // 
            this.timerTrackBar.LargeChange = 100;
            this.timerTrackBar.Location = new System.Drawing.Point(83, 14);
            this.timerTrackBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.timerTrackBar.Maximum = 1000;
            this.timerTrackBar.Minimum = 1;
            this.timerTrackBar.Name = "timerTrackBar";
            this.timerTrackBar.Size = new System.Drawing.Size(164, 50);
            this.timerTrackBar.SmallChange = 10;
            this.timerTrackBar.TabIndex = 19;
            this.timerTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.timerTrackBar.Value = 100;
            this.timerTrackBar.Scroll += new System.EventHandler(this.TimerTrackBarScroll);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 20);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(83, 17);
            this.label15.TabIndex = 18;
            this.label15.Text = "Update int.: ";
            // 
            // printResCheckBox
            // 
            this.printResCheckBox.AutoSize = true;
            this.printResCheckBox.Checked = true;
            this.printResCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.printResCheckBox.Location = new System.Drawing.Point(147, 167);
            this.printResCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.printResCheckBox.Name = "printResCheckBox";
            this.printResCheckBox.Size = new System.Drawing.Size(102, 21);
            this.printResCheckBox.TabIndex = 27;
            this.printResCheckBox.Text = "Print Results";
            this.printResCheckBox.UseVisualStyleBackColor = true;
            // 
            // advanceNumUD
            // 
            this.advanceNumUD.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.advanceNumUD.Location = new System.Drawing.Point(130, 132);
            this.advanceNumUD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.advanceNumUD.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.advanceNumUD.Name = "advanceNumUD";
            this.advanceNumUD.Size = new System.Drawing.Size(116, 23);
            this.advanceNumUD.TabIndex = 26;
            this.advanceNumUD.Value = new decimal(new int[] {
            99000,
            0,
            0,
            0});
            // 
            // advanceBtn
            // 
            this.advanceBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.advanceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.advanceBtn.Location = new System.Drawing.Point(18, 129);
            this.advanceBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.advanceBtn.Name = "advanceBtn";
            this.advanceBtn.Size = new System.Drawing.Size(88, 27);
            this.advanceBtn.TabIndex = 24;
            this.advanceBtn.Text = "&Advance to:";
            this.advanceBtn.UseVisualStyleBackColor = true;
            this.advanceBtn.Click += new System.EventHandler(this.AdvanceBtnClick);
            // 
            // printWorldBtn
            // 
            this.printWorldBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.printWorldBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printWorldBtn.Location = new System.Drawing.Point(17, 162);
            this.printWorldBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.printWorldBtn.Name = "printWorldBtn";
            this.printWorldBtn.Size = new System.Drawing.Size(88, 27);
            this.printWorldBtn.TabIndex = 25;
            this.printWorldBtn.Text = "Print &World";
            this.printWorldBtn.UseVisualStyleBackColor = true;
            this.printWorldBtn.Click += new System.EventHandler(this.PrintWorldBtnClick);
            // 
            // stepBtn
            // 
            this.stepBtn.Enabled = false;
            this.stepBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.stepBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stepBtn.Location = new System.Drawing.Point(94, 96);
            this.stepBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stepBtn.Name = "stepBtn";
            this.stepBtn.Size = new System.Drawing.Size(70, 27);
            this.stepBtn.TabIndex = 21;
            this.stepBtn.Text = "&Step";
            this.stepBtn.UseVisualStyleBackColor = true;
            this.stepBtn.Click += new System.EventHandler(this.StepBtnClick);
            // 
            // pauseBtn
            // 
            this.pauseBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.pauseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pauseBtn.Location = new System.Drawing.Point(18, 96);
            this.pauseBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pauseBtn.Name = "pauseBtn";
            this.pauseBtn.Size = new System.Drawing.Size(70, 27);
            this.pauseBtn.TabIndex = 22;
            this.pauseBtn.Text = "&Pause";
            this.pauseBtn.UseVisualStyleBackColor = true;
            this.pauseBtn.Click += new System.EventHandler(this.PauseBtnClick);
            // 
            // resetBtn
            // 
            this.resetBtn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.resetBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetBtn.Location = new System.Drawing.Point(176, 96);
            this.resetBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.resetBtn.Name = "resetBtn";
            this.resetBtn.Size = new System.Drawing.Size(70, 27);
            this.resetBtn.TabIndex = 23;
            this.resetBtn.Text = "&Reset";
            this.resetBtn.UseVisualStyleBackColor = true;
            this.resetBtn.Click += new System.EventHandler(this.ResetBtnClick);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(13, 200);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(113, 17);
            this.label20.TabIndex = 29;
            this.label20.Text = "Controlled agent:";
            // 
            // controlAgComboBox
            // 
            this.controlAgComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.controlAgComboBox.FormattingEnabled = true;
            this.controlAgComboBox.Items.AddRange(new object[] {
            "Agent 1",
            "Agent 2",
            "Agent 3"});
            this.controlAgComboBox.Location = new System.Drawing.Point(17, 218);
            this.controlAgComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.controlAgComboBox.Name = "controlAgComboBox";
            this.controlAgComboBox.Size = new System.Drawing.Size(229, 23);
            this.controlAgComboBox.TabIndex = 28;
            this.controlAgComboBox.SelectedIndexChanged += new System.EventHandler(this.ControlAgListBoxSelectedIndexChanged);
            // 
            // showAgInfoCBox
            // 
            this.showAgInfoCBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showAgInfoCBox.AutoSize = true;
            this.showAgInfoCBox.Checked = true;
            this.showAgInfoCBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showAgInfoCBox.Location = new System.Drawing.Point(116, 69);
            this.showAgInfoCBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.showAgInfoCBox.Name = "showAgInfoCBox";
            this.showAgInfoCBox.Size = new System.Drawing.Size(129, 21);
            this.showAgInfoCBox.TabIndex = 30;
            this.showAgInfoCBox.Text = "Show Agent Info";
            this.showAgInfoCBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.showAgInfoCBox.UseVisualStyleBackColor = true;
            // 
            // SimulationControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(260, 288);
            this.ControlBox = false;
            this.Controls.Add(this.showAgInfoCBox);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.controlAgComboBox);
            this.Controls.Add(this.printResCheckBox);
            this.Controls.Add(this.advanceNumUD);
            this.Controls.Add(this.advanceBtn);
            this.Controls.Add(this.printWorldBtn);
            this.Controls.Add(this.stepBtn);
            this.Controls.Add(this.pauseBtn);
            this.Controls.Add(this.resetBtn);
            this.Controls.Add(this.debugCheckBox);
            this.Controls.Add(this.timerTrackBar);
            this.Controls.Add(this.label15);
            this.Font = new System.Drawing.Font("Candara", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SimulationControlForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Simulation Controller";
            ((System.ComponentModel.ISupportInitialize)(this.timerTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.advanceNumUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox debugCheckBox;
        private System.Windows.Forms.TrackBar timerTrackBar;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox printResCheckBox;
        private System.Windows.Forms.NumericUpDown advanceNumUD;
        private System.Windows.Forms.Button advanceBtn;
        private System.Windows.Forms.Button printWorldBtn;
        private System.Windows.Forms.Button stepBtn;
        private System.Windows.Forms.Button pauseBtn;
        private System.Windows.Forms.Button resetBtn;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox controlAgComboBox;
        private System.Windows.Forms.CheckBox showAgInfoCBox;
    }
}