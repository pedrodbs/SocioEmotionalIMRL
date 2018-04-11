// ------------------------------------------
// <copyright file="CellDescriptionForm.cs" company="Pedro Sequeira">
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
//    Project: Learning.Utils.EnvironmentEditor

//    Last updated: 2/21/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PS.Learning.Core.Domain.Cells;

namespace PS.Learning.Utils.EnvironmentEditor
{
    public partial class CellDescriptionForm : Form
    {
        public CellDescriptionForm(IEnumerable<Cell> cells)
        {
            this.Cells = cells;
            InitializeComponent();

            this.LoadCellsDescriptions();

            this.Visible = false;
            this.DialogResult = DialogResult.None;
            this.newDescTextBox.Focus();
        }

        public IEnumerable<Cell> Cells { get; set; }

        protected void LoadCellsDescriptions()
        {
            this.oldDescTextBox.Clear();
            foreach (var cell in this.Cells)
                this.oldDescTextBox.AppendText(string.Format("{0}{1}", cell.Description, Environment.NewLine));
        }

        protected virtual void SaveCellsDescription()
        {
            foreach (var cell in this.Cells)
                cell.Description = this.newDescTextBox.Text;
        }

        private void OkBtnClick(object sender, EventArgs e)
        {
            this.SaveCellsDescription();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelBtnClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}