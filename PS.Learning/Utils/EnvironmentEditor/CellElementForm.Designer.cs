namespace PS.Learning.Utils.EnvironmentEditor
{
    partial class CellElementForm
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
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.descTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rwdNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.visibleCheckBox = new System.Windows.Forms.CheckBox();
            this.walkCheckBox = new System.Windows.Forms.CheckBox();
            this.smellCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.colorPanel = new System.Windows.Forms.Panel();
            this.imgPathTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.changeButton = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.selectFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.rwdNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okBtn.Location = new System.Drawing.Point(14, 303);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(87, 27);
            this.okBtn.TabIndex = 9;
            this.okBtn.Text = "&Save";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.OkBtnClick);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelBtn.Location = new System.Drawing.Point(307, 303);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(87, 27);
            this.cancelBtn.TabIndex = 10;
            this.cancelBtn.Text = "&Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.CancelBtnClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(96, 12);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(298, 23);
            this.nameTextBox.TabIndex = 0;
            this.nameTextBox.TextChanged += new System.EventHandler(this.NameTextBoxTextChanged);
            // 
            // descTextBox
            // 
            this.descTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descTextBox.Location = new System.Drawing.Point(96, 42);
            this.descTextBox.Name = "descTextBox";
            this.descTextBox.Size = new System.Drawing.Size(298, 23);
            this.descTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Description:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Reward:";
            // 
            // rwdNumericUpDown
            // 
            this.rwdNumericUpDown.Location = new System.Drawing.Point(96, 73);
            this.rwdNumericUpDown.Name = "rwdNumericUpDown";
            this.rwdNumericUpDown.Size = new System.Drawing.Size(77, 23);
            this.rwdNumericUpDown.TabIndex = 2;
            // 
            // visibleCheckBox
            // 
            this.visibleCheckBox.AutoSize = true;
            this.visibleCheckBox.Checked = true;
            this.visibleCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.visibleCheckBox.Location = new System.Drawing.Point(19, 107);
            this.visibleCheckBox.Name = "visibleCheckBox";
            this.visibleCheckBox.Size = new System.Drawing.Size(65, 21);
            this.visibleCheckBox.TabIndex = 4;
            this.visibleCheckBox.Text = "Visible";
            this.visibleCheckBox.UseVisualStyleBackColor = true;
            // 
            // walkCheckBox
            // 
            this.walkCheckBox.AutoSize = true;
            this.walkCheckBox.Checked = true;
            this.walkCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.walkCheckBox.Location = new System.Drawing.Point(91, 107);
            this.walkCheckBox.Name = "walkCheckBox";
            this.walkCheckBox.Size = new System.Drawing.Size(81, 21);
            this.walkCheckBox.TabIndex = 5;
            this.walkCheckBox.Text = "Walkable";
            this.walkCheckBox.UseVisualStyleBackColor = true;
            // 
            // smellCheckBox
            // 
            this.smellCheckBox.AutoSize = true;
            this.smellCheckBox.Location = new System.Drawing.Point(181, 107);
            this.smellCheckBox.Name = "smellCheckBox";
            this.smellCheckBox.Size = new System.Drawing.Size(82, 21);
            this.smellCheckBox.TabIndex = 6;
            this.smellCheckBox.Text = "Has smell";
            this.smellCheckBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(238, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Color:";
            // 
            // colorPanel
            // 
            this.colorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.colorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorPanel.Location = new System.Drawing.Point(285, 73);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(109, 23);
            this.colorPanel.TabIndex = 3;
            // 
            // imgPathTextBox
            // 
            this.imgPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imgPathTextBox.BackColor = System.Drawing.Color.White;
            this.imgPathTextBox.Location = new System.Drawing.Point(96, 134);
            this.imgPathTextBox.Name = "imgPathTextBox";
            this.imgPathTextBox.ReadOnly = true;
            this.imgPathTextBox.Size = new System.Drawing.Size(298, 23);
            this.imgPathTextBox.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "Image path:";
            // 
            // changeButton
            // 
            this.changeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.changeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.changeButton.Location = new System.Drawing.Point(219, 253);
            this.changeButton.Name = "changeButton";
            this.changeButton.Size = new System.Drawing.Size(87, 27);
            this.changeButton.TabIndex = 8;
            this.changeButton.Text = "C&hange...";
            this.changeButton.UseVisualStyleBackColor = true;
            this.changeButton.Click += new System.EventHandler(this.ChangeButtonClick);
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(96, 164);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(116, 115);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 15;
            this.pictureBox.TabStop = false;
            // 
            // colorDialog
            // 
            this.colorDialog.FullOpen = true;
            // 
            // selectFileDialog
            // 
            this.selectFileDialog.DefaultExt = "png";
            this.selectFileDialog.InitialDirectory = "../../bin/resources";
            // 
            // CellElementForm
            // 
            this.AcceptButton = this.okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(408, 344);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.changeButton);
            this.Controls.Add(this.imgPathTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.colorPanel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.smellCheckBox);
            this.Controls.Add(this.walkCheckBox);
            this.Controls.Add(this.visibleCheckBox);
            this.Controls.Add(this.rwdNumericUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.descTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Font = new System.Drawing.Font("Candara", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CellElementForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit cell element";
            ((System.ComponentModel.ISupportInitialize)(this.rwdNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox descTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown rwdNumericUpDown;
        private System.Windows.Forms.CheckBox visibleCheckBox;
        private System.Windows.Forms.CheckBox walkCheckBox;
        private System.Windows.Forms.CheckBox smellCheckBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel colorPanel;
        private System.Windows.Forms.TextBox imgPathTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button changeButton;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.OpenFileDialog selectFileDialog;
    }
}