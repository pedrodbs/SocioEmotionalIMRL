// ------------------------------------------
// <copyright file="ChooseOptionForm.cs" company="Pedro Sequeira">
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

//    Last updated: 03/25/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PS.Utilities.Forms
{
    public partial class ChooseOptionForm : Form
    {
        #region Private Fields

        private const int BUTTON_HEIGHT = 33;
        private const int BUTTON_WIDTH = 183;
        private const int BUTTON_X_LOCATION = 12;
        private const int BUTTON_Y_INTERVAL = 6;
        private const int BUTTON_Y_LOCATION = 12;

        private int _maxButtonWidth = BUTTON_WIDTH;

        #endregion Private Fields

        #region Public Constructors

        public ChooseOptionForm(List<string> options)
        {
            this.InitializeComponent();
            this.CreateButtons(options);
        }

        #endregion Public Constructors

        #region Public Properties

        public string ChosenOption { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public static string ShowOptions(List<string> options, string text)
        {
            using (var chooseForm = new ChooseOptionForm(options) { Text = text })
            {
                chooseForm.Show();
                while (!chooseForm.IsDisposed)
                    Application.DoEvents();

                return chooseForm.ChosenOption;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void ButtonClick(object sender, EventArgs e)
        {
            this.ChosenOption = ((Button)sender).Text;
            this.Close();
        }

        private void CreateButton(int i, string text)
        {
            var textSize = TextRenderer.MeasureText(text, this.Font);
            this._maxButtonWidth = System.Math.Max(this._maxButtonWidth, textSize.Width + 10);
            var button = new Button
            {
                Location = new Point(BUTTON_X_LOCATION, BUTTON_Y_LOCATION + i * (BUTTON_HEIGHT + BUTTON_Y_INTERVAL)),
                Name = $"button{i}",
                Size = new Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                TabIndex = i,
                Text = text,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = true
            };
            button.Click += this.ButtonClick;
            this.Controls.Add(button);
        }

        private void CreateButtons(List<string> options)
        {
            this.Controls.Clear();
            var numOptions = options.Count;
            for (var i = 0; i < numOptions; i++)
                this.CreateButton(i, options[i]);
            for (var i = 0; i < numOptions; i++)
                this.Controls[i].Width = this._maxButtonWidth;
            this.ClientSize = new Size(this._maxButtonWidth + (2 * BUTTON_X_LOCATION),
                (numOptions * BUTTON_HEIGHT) + ((numOptions - 1) * BUTTON_Y_INTERVAL) + (2 * BUTTON_Y_LOCATION));
        }

        #endregion Private Methods
    }
}