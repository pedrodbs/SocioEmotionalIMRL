using System.Windows.Forms;
using PS.Learning.Forms;
using PS.Utilities.Forms;

namespace PS.Learning.Tests.EmotionalOptimization.Forms
{
    partial class PacmanLightSimForm
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
           
            this.lifePBox4 = new CustomRenderPictureBox();
            this.lifePBox3 = new CustomRenderPictureBox();
            this.lifePBox2 = new CustomRenderPictureBox();
            this.lifePBox1 = new CustomRenderPictureBox();
            
            ((System.ComponentModel.ISupportInitialize)(this.lifePBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lifePBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lifePBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lifePBox1)).BeginInit();
            this.SuspendLayout();
            
            // 
            // lifePBox4
            // 
            this.lifePBox4.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            this.lifePBox4.Image = global::PS.Learning.Tests.EmotionalOptimization.Properties.Resources.life;
            this.lifePBox4.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.lifePBox4.Location = new System.Drawing.Point(90, 13);
            this.lifePBox4.Name = "lifePBox4";
            this.lifePBox4.Size = new System.Drawing.Size(25, 25);
            this.lifePBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.lifePBox4.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.lifePBox4.TabIndex = 3;
            this.lifePBox4.TabStop = false;
            this.lifePBox4.Visible = false;
            // 
            // lifePBox3
            // 
            this.lifePBox3.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            this.lifePBox3.Image = global::PS.Learning.Tests.EmotionalOptimization.Properties.Resources.life;
            this.lifePBox3.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.lifePBox3.Location = new System.Drawing.Point(65, 13);
            this.lifePBox3.Name = "lifePBox3";
            this.lifePBox3.Size = new System.Drawing.Size(25, 25);
            this.lifePBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.lifePBox3.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.lifePBox3.TabIndex = 3;
            this.lifePBox3.TabStop = false;
            this.lifePBox3.Visible = false;
            // 
            // lifePBox2
            // 
            this.lifePBox2.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            this.lifePBox2.Image = global::PS.Learning.Tests.EmotionalOptimization.Properties.Resources.life;
            this.lifePBox2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.lifePBox2.Location = new System.Drawing.Point(40, 13);
            this.lifePBox2.Name = "lifePBox2";
            this.lifePBox2.Size = new System.Drawing.Size(25, 25);
            this.lifePBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.lifePBox2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.lifePBox2.TabIndex = 3;
            this.lifePBox2.TabStop = false;
            this.lifePBox2.Visible = false;
            // 
            // lifePBox1
            // 
            this.lifePBox1.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            this.lifePBox1.Image = global::PS.Learning.Tests.EmotionalOptimization.Properties.Resources.life;
            this.lifePBox1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.lifePBox1.Location = new System.Drawing.Point(15, 13);
            this.lifePBox1.Name = "lifePBox1";
            this.lifePBox1.Size = new System.Drawing.Size(25, 25);
            this.lifePBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.lifePBox1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.lifePBox1.TabIndex = 3;
            this.lifePBox1.TabStop = false;
            this.lifePBox1.Visible = false;
            // 
            // PacmanLightSimForm
            // 
            this.simulationPanel.Controls.Add(this.lifePBox1);
            this.simulationPanel.Controls.Add(this.lifePBox2);
            this.simulationPanel.Controls.Add(this.lifePBox3);
            this.simulationPanel.Controls.Add(this.lifePBox4);
            
            ((System.ComponentModel.ISupportInitialize)(this.lifePBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lifePBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lifePBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lifePBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        
        private CustomRenderPictureBox lifePBox1;
        private CustomRenderPictureBox lifePBox3;
        private CustomRenderPictureBox lifePBox2;
        private CustomRenderPictureBox lifePBox4;
    }
}