// ------------------------------------------
// <copyright file="ProgressFormUpdater.cs" company="Pedro Sequeira">
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

//    Last updated: 10/24/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Windows.Forms;
using PS.Utilities.Core;

namespace PS.Utilities.Forms
{
    public class ProgressFormUpdater : FormUpdater
    {
        #region Protected Fields

        protected IProgressHandler progressHandler;

        #endregion Protected Fields

        #region Public Constructors

        public ProgressFormUpdater(IProgressHandler progressHandler)
        {
            this.progressHandler = progressHandler;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override Form CreateForm()
        {
            //creates progress form and starts time
            var progressForm = new ProgressForm();
            progressForm.Closed += this.FormClosed;
            progressForm.ControlBox = this.EventsAreHandled;
            progressForm.ShowInTaskbar = true;
            progressForm.Show();
            progressForm.Select();
            progressForm.StartTimer();
            Application.DoEvents();
            return progressForm;
        }

        protected override void UpdateForm(Form form)
        {
            if (!(form is ProgressForm)) return;
            var progressForm = (ProgressForm)form;

            //updates progress bar and allows application event-handling
            progressForm.Text = $"{(progressForm.Progress * 100):000}% - {this.Text}";
            progressForm.Progress = this.progressHandler.ProgressValue;
        }

        #endregion Protected Methods
    }
}