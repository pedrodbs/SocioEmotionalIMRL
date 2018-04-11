// ------------------------------------------
// <copyright file="FormUpdater.cs" company="Pedro Sequeira">
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

using PS.Utilities.Core;
using System;
using System.Threading;
using System.Windows.Forms;

namespace PS.Utilities.Forms
{
    public abstract class FormUpdater : IDisposable, IResetable
    {
        #region Protected Fields

        protected Thread formThread;
        protected bool visible;

        #endregion Protected Fields

        #region Protected Constructors

        protected FormUpdater()
        {
            this.UpdateInterval = 100;
        }

        #endregion Protected Constructors

        #region Public Events

        public event EventHandler FormTerminated;

        #endregion Public Events

        #region Public Properties

        public bool IsRunning
        {
            get { return this.formThread != null; }
        }

        public string Text { get; set; }

        public int UpdateInterval { get; set; }

        public bool Visible
        {
            set
            {
                this.visible = value;
                if (value)
                {
                    //starts progrees thread
                    this.formThread = new Thread(this.ControlForm);
                    this.formThread.Start();
                }
                else if (this.formThread != null)
                {
                    //waits thread to end or aborts
                    this.formThread.Abort();
                    this.formThread.Join(1000);
                    this.formThread = null;
                }
            }
            get { return this.visible; }
        }

        #endregion Public Properties

        #region Protected Properties

        protected bool EventsAreHandled
        {
            get { return this.FormTerminated != null; }
        }

        #endregion Protected Properties

        #region Public Methods

        public void Dispose()
        {
            this.Visible = false;
        }

        public void Reset()
        {
            var isShowing = this.visible;
            if (isShowing) this.Visible = false;
            this.Visible = isShowing;
        }

        public void WaitFormClose()
        {
            this.formThread.Join();
        }

        #endregion Public Methods

        #region Protected Methods

        protected void ControlForm()
        {
            Form form = null;

            while (this.visible)
            {
                //waits for update interval
                Thread.Sleep(this.UpdateInterval);

                //creates form if null
                if (form == null) form = this.CreateForm();

                //updates form and application events
                this.UpdateForm(form);
                Application.DoEvents();
            }

            //closes progress form, detaches event handler
            if (form == null) return;
            form.Closed -= this.FormClosed;
            form.Close();
            form.Dispose();
        }

        protected abstract Form CreateForm();

        protected void FormClosed(object sender, EventArgs e)
        {
            //sends progress terminated event if someone is listening
            if (this.FormTerminated != null)
                this.FormTerminated(this, EventArgs.Empty);

            //terminates progress thread
            this.Visible = false;
        }

        protected virtual void UpdateForm(Form form)
        {
        }

        #endregion Protected Methods
    }
}