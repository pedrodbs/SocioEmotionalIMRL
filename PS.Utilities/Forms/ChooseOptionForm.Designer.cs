namespace PS.Utilities.Forms
{
    partial class ChooseOptionForm
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
            this.btnOption1 = new System.Windows.Forms.Button();
            this.btnOption2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOption1
            // 
            this.btnOption1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnOption1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOption1.Location = new System.Drawing.Point(16, 17);
            this.btnOption1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOption1.Name = "btnOption1";
            this.btnOption1.Size = new System.Drawing.Size(244, 46);
            this.btnOption1.TabIndex = 0;
            this.btnOption1.Text = "Option 1";
            this.btnOption1.UseVisualStyleBackColor = true;
            // 
            // btnOption2
            // 
            this.btnOption2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnOption2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOption2.Location = new System.Drawing.Point(16, 71);
            this.btnOption2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOption2.Name = "btnOption2";
            this.btnOption2.Size = new System.Drawing.Size(244, 46);
            this.btnOption2.TabIndex = 1;
            this.btnOption2.Text = "Option 2";
            this.btnOption2.UseVisualStyleBackColor = true;
            // 
            // ChooseOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(276, 136);
            this.Controls.Add(this.btnOption1);
            this.Controls.Add(this.btnOption2);
            this.Font = new System.Drawing.Font("Candara", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseOptionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose one option";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOption1;
        private System.Windows.Forms.Button btnOption2;
    }
}