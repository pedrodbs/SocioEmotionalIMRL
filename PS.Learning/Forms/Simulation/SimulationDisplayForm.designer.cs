using PS.Utilities.Forms;

namespace PS.Learning.Forms.Simulation
{
    partial class SimulationDisplayForm
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
            this.simulationPanel = new System.Windows.Forms.Panel();
            this.fitnessLabel = new CustomRenderLabel();
            this.numStepsLabel = new CustomRenderLabel();
            this.simulationPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // simulationPanel
            // 
            this.simulationPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.simulationPanel.Controls.Add(this.fitnessLabel);
            this.simulationPanel.Controls.Add(this.numStepsLabel);
            this.simulationPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simulationPanel.Location = new System.Drawing.Point(0, 0);
            this.simulationPanel.Name = "simulationPanel";
            this.simulationPanel.Size = new System.Drawing.Size(231, 340);
            this.simulationPanel.TabIndex = 0;
            // 
            // fitnessLabel
            // 
            this.fitnessLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fitnessLabel.AutoSize = true;
            this.fitnessLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.fitnessLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fitnessLabel.Font = new System.Drawing.Font("Candara", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fitnessLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fitnessLabel.Location = new System.Drawing.Point(126, 311);
            this.fitnessLabel.Name = "fitnessLabel";
            this.fitnessLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fitnessLabel.Size = new System.Drawing.Size(93, 20);
            this.fitnessLabel.TabIndex = 0;
            this.fitnessLabel.Text = "Fitness: 1.000";
            this.fitnessLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fitnessLabel.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // numStepsLabel
            // 
            this.numStepsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numStepsLabel.AutoSize = true;
            this.numStepsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.numStepsLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numStepsLabel.Font = new System.Drawing.Font("Candara", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numStepsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.numStepsLabel.Location = new System.Drawing.Point(12, 311);
            this.numStepsLabel.Name = "numStepsLabel";
            this.numStepsLabel.Size = new System.Drawing.Size(91, 20);
            this.numStepsLabel.TabIndex = 0;
            this.numStepsLabel.Text = "Step: 100.000";
            this.numStepsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.numStepsLabel.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // SimulationDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(231, 340);
            this.Controls.Add(this.simulationPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SimulationDisplayForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simulation environment";
            this.simulationPanel.ResumeLayout(false);
            this.simulationPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Panel simulationPanel;
        private CustomRenderLabel fitnessLabel;
        private CustomRenderLabel numStepsLabel;
    }
}


