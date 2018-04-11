

namespace PS.Utilities.Forms.Math
{
    partial class QuantitiesForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.exitSepMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Chart = new QuantitiesChart();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitSepMenuItem,
            this.printChartToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(462, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "mainMenuStrip";
            // 
            // exitSepMenuItem
            // 
            this.exitSepMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateChartToolStripMenuItem});
            this.exitSepMenuItem.Name = "exitSepMenuItem";
            this.exitSepMenuItem.Size = new System.Drawing.Size(94, 23);
            this.exitSepMenuItem.Text = "&Update Options";
            // 
            // updateChartToolStripMenuItem
            // 
            this.updateChartToolStripMenuItem.Checked = true;
            this.updateChartToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateChartToolStripMenuItem.Name = "updateChartToolStripMenuItem";
            this.updateChartToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.updateChartToolStripMenuItem.Text = "&Update chart";
            this.updateChartToolStripMenuItem.Click += new System.EventHandler(this.UpdateChartToolStripMenuItemClick);
            // 
            // printChartToolStripMenuItem
            // 
            this.printChartToolStripMenuItem.Name = "printChartToolStripMenuItem";
            this.printChartToolStripMenuItem.Size = new System.Drawing.Size(69, 23);
            this.printChartToolStripMenuItem.Text = "&Print chart";
            this.printChartToolStripMenuItem.Click += new System.EventHandler(this.PrintChartToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // quantitiesChart
            // 
            this.Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Chart.Location = new System.Drawing.Point(0, 27);
            this.Chart.Name = "Chart";
            this.Chart.Quantities = null;
            this.Chart.Size = new System.Drawing.Size(462, 314);
            this.Chart.TabIndex = 0;
//            this.quantitiesChart.XAxisLabel = "Time step";
//            this.quantitiesChart.YAxisLabel = "Values";
            // 
            // QuantitiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 341);
            this.Controls.Add(this.Chart);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "QuantitiesForm";
            this.ShowIcon = false;
            this.Text = "QuantityForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitSepMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateChartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printChartToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}