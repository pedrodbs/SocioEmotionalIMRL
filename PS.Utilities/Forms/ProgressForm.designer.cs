namespace PS.Utilities.Forms
{
    partial class ProgressForm
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.totalTimeLabel = new System.Windows.Forms.Label();
            this.timeLeftlabel = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(378, 30);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 0;
            // 
            // totalTimeLabel
            // 
            this.totalTimeLabel.AutoSize = true;
            this.totalTimeLabel.Location = new System.Drawing.Point(16, 37);
            this.totalTimeLabel.Name = "totalTimeLabel";
            this.totalTimeLabel.Size = new System.Drawing.Size(47, 19);
            this.totalTimeLabel.TabIndex = 1;
            this.totalTimeLabel.Text = "Total:";
            // 
            // timeLeftlabel
            // 
            this.timeLeftlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.timeLeftlabel.AutoSize = true;
            this.timeLeftlabel.Location = new System.Drawing.Point(243, 37);
            this.timeLeftlabel.Name = "timeLeftlabel";
            this.timeLeftlabel.Size = new System.Drawing.Size(40, 19);
            this.timeLeftlabel.TabIndex = 2;
            this.timeLeftlabel.Text = "Left:";
            this.timeLeftlabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(378, 62);
            this.ControlBox = false;
            this.Controls.Add(this.timeLeftlabel);
            this.Controls.Add(this.totalTimeLabel);
            this.Controls.Add(this.progressBar);
            this.Font = new System.Drawing.Font("Candara", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.ShowIcon = false;
            this.Text = "TestProgressForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label totalTimeLabel;
        private System.Windows.Forms.Label timeLeftlabel;
        private System.Windows.Forms.Timer timer;
    }
}