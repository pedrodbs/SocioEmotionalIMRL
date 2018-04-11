// ------------------------------------------
// <copyright file="QuantitiesChart.cs" company="Pedro Sequeira">
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
using System.Windows.Forms;

namespace PS.Utilities.Forms.Math
{
    public partial class QuantitiesChart : UserControl
    {
        #region Private Fields

        private readonly object _locker = new object();

        #endregion Private Fields

        #region Public Constructors

        public QuantitiesChart()
        {
            this.InitializeComponent();
            this.Quantities = new StatisticsCollection();
        }

        #endregion Public Constructors

        #region Public Properties

        public StatisticsCollection Quantities { get; internal set; }
        public string Title { get; set; }

        #endregion Public Properties

        #region Public Methods

        public virtual void SaveImage(string imgFilePath, int width, int height, ChartImageFormat format)
        {
            PlottingUtil.ExportChartToImage(imgFilePath, width, height, this.plotView.Model, format);
        }

        public void UpdateData()
        {
            //refreshes the chart on demand, if such option is selected and chart has new data
            lock (this._locker)
            {
                if (!this.Enabled)
                    return;

                this.plotView.Model = StatisticsCollectionExtensions.CreateChart(this.Quantities, this.Title);
            }
        }

        #endregion Public Methods
    }
}