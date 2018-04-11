// ------------------------------------------
// <copyright file="QuantitiesForm.cs" company="Pedro Sequeira">
// 
//     Copyright (c) 2018 Pedro Sequeira
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// </copyright>
// <summary>
//    Project: PS.Utilities.Forms

//    Last updated: 12/18/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Utilities.Math;
using System;
using System.IO;
using System.Windows.Forms;

namespace PS.Utilities.Forms.Math
{
    public partial class QuantitiesForm : Form
    {
        #region Private Fields

        private readonly string _filePath;

        #endregion Private Fields

        #region Public Constructors

        public QuantitiesForm(string title, string filePath, StatisticsCollection quantities)
        {
            InitializeComponent();

            this._filePath = filePath;
            this.Chart.Quantities = quantities;
            this.Chart.Title = title;
        }

        #endregion Public Constructors

        #region Public Properties

        public QuantitiesChart Chart { get; private set; }

        #endregion Public Properties

        #region Private Methods

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PrintChartToolStripMenuItemClick(object sender, EventArgs e)
        {
            var basePath = $"{this._filePath}{Path.DirectorySeparatorChar}{this.Text}";
            this.Chart.Quantities.PrintAllQuantitiesToCSV($"{basePath}.csv");
            //            this.quantitiesChart.Chart.SaveImage(string.Format("{0}.png", basePath), ChartImageFormat.Png);
        }

        private void UpdateChartToolStripMenuItemClick(object sender, EventArgs e)
        {
            //toggles chart visibility
            this.Chart.Enabled =
                this.updateChartToolStripMenuItem.Checked = !this.updateChartToolStripMenuItem.Checked;
        }

        #endregion Private Methods
    }
}