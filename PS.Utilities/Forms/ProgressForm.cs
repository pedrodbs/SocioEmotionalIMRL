// ------------------------------------------
// <copyright file="ProgressForm.cs" company="Pedro Sequeira">
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
//    Project: PS.Utilities

//    Last updated: 5/20/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Windows.Forms;

namespace PS.Utilities.Forms
{
    public partial class ProgressForm : Form
    {
        private double _progress;
        private DateTime _startTime;

        public ProgressForm()
        {
            InitializeComponent();
        }

        public double Progress
        {
            set
            {
                if (value > 1) value = 1;
                else if (value < 0) value = 0;
                this.progressBar.Value = (int) (value*100);
                this._progress = value;
            }
            get { return this._progress; }
        }

        public void StartTimer()
        {
            this._startTime = DateTime.Now;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var totalTime = DateTime.Now - this._startTime;
            var timeLeft =
                TimeSpan.FromSeconds(this._progress.Equals(0)
                                         ? 0
                                         : (totalTime.TotalSeconds/this._progress) - totalTime.TotalSeconds);

			this.totalTimeLabel.Text = $"Total: {totalTime:dd\\.hh\\:mm\\:ss}";
			this.timeLeftlabel.Text = $"Left: {timeLeft:dd\\.hh\\:mm\\:ss}";
        }
    }
}