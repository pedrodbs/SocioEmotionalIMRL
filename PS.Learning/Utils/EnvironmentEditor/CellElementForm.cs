// ------------------------------------------
// <copyright file="CellElementForm.cs" company="Pedro Sequeira">
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

//    Last updated: 1/16/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Forms;
using PS.Utilities.Forms;
using PS.Utilities.IO;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace PS.Learning.Utils.EnvironmentEditor
{
    public partial class CellElementForm : Form
    {
        #region Public Constructors

        public CellElementForm(ICellElement cellElement)
        {
            InitializeComponent();

            this.CellElement = cellElement;
            this.LoadCellElementInfo();

            this.Visible = false;
            this.DialogResult = DialogResult.None;
            this.nameTextBox.Focus();
            this.selectFileDialog.Filter = Util.GetImageFilesDialogFilter();
            this.selectFileDialog.FilterIndex = ImageCodecInfo.GetImageEncoders().Length;
            this.colorPanel.DoubleClick += this.ColorPanelDoubleClick;
        }

        #endregion Public Constructors

        #region Public Properties

        public ICellElement CellElement { get; protected set; }

        #endregion Public Properties

        #region Protected Methods

        protected void LoadCellElementInfo()
        {
            if (this.CellElement == null)
                this.CellElement = new CellElement { IdToken = "new cell element" };

            this.nameTextBox.Text = this.CellElement.IdToken;
            this.descTextBox.Text = this.CellElement.Description;
            this.imgPathTextBox.Text = this.CellElement.ImagePath;
            this.visibleCheckBox.Checked = this.CellElement.Visible;
            this.walkCheckBox.Checked = this.CellElement.Walkable;
            this.smellCheckBox.Checked = this.CellElement.HasSmell;
            this.colorPanel.BackColor = this.colorDialog.Color = this.CellElement.Color.ToColor();
            this.rwdNumericUpDown.Value = (decimal)this.CellElement.Reward;

            if (File.Exists(this.CellElement.ImagePath))
                this.pictureBox.Load(this.CellElement.ImagePath);
        }

        protected virtual void SaveCellElementInfo()
        {
            this.CellElement.IdToken = this.nameTextBox.Text;
            this.CellElement.Description = this.descTextBox.Text;
            this.CellElement.ImagePath = this.imgPathTextBox.Text;
            this.CellElement.Visible = this.visibleCheckBox.Checked;
            this.CellElement.Walkable = this.walkCheckBox.Checked;
            this.CellElement.HasSmell = this.smellCheckBox.Checked;
            this.CellElement.Color = this.colorPanel.BackColor.ToColorInfo();
            this.CellElement.Reward = (double)this.rwdNumericUpDown.Value;
        }

        #endregion Protected Methods

        #region Private Methods

        private void CancelBtnClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChangeButtonClick(object sender, EventArgs e)
        {
            var result = this.selectFileDialog.ShowDialog();

            if (!result.Equals(DialogResult.OK)) return;

            var path = this.selectFileDialog.FileName;
            this.imgPathTextBox.Text = PathUtil.GetRelativePath(path);

            if (File.Exists(path))
                this.pictureBox.Load(path);
        }

        private void ColorPanelDoubleClick(object sender, EventArgs e)
        {
            this.colorDialog.ShowDialog();
            this.colorPanel.BackColor = this.colorDialog.Color;
        }

        private void NameTextBoxTextChanged(object sender, EventArgs e)
        {
            this.okBtn.Enabled = !string.IsNullOrEmpty(this.nameTextBox.Text);
        }

        private void OkBtnClick(object sender, EventArgs e)
        {
            this.SaveCellElementInfo();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion Private Methods
    }
}