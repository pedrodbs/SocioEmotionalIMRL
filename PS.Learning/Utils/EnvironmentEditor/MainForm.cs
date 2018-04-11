// ------------------------------------------
// <copyright file="MainForm.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.Utils.EnvironmentEditor

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.Environments;
using PS.Learning.Forms.Simulation;
using PS.Utilities.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PS.Learning.Utils.EnvironmentEditor
{
    public partial class MainForm : Form
    {
        #region Public Fields

        public const int CELL_SIZE = 50;
        public const int MIN_CELL_SIZE = 5;

        #endregion Public Fields

        #region Protected Fields

        protected readonly Dictionary<ICellElement, HashSet<Cell>> cellElemList =
            new Dictionary<ICellElement, HashSet<Cell>>();

        protected readonly List<MenuItem> contextCellElementMenuItems = new List<MenuItem>();
        protected readonly HashSet<ICellElement> copiedElements = new HashSet<ICellElement>();
        protected readonly HashSet<Cell> selectedCells = new HashSet<Cell>();

        protected readonly CellElement wallElement = new CellElement
        {
            IdToken = "wall",
            Walkable = false,
            Color = Color.FromArgb(125, 125, 125).ToColorInfo(),
            Reward = 0,
            Visible = true,
            HasSmell = false,
            ImagePath = ""
        };

        protected int cellSize = CELL_SIZE;
        protected EnvironmentControl environmentControl;
        protected Cell finalSelectCell;
        protected Cell initialSelectCell;
        protected bool mouseDown;
        protected ICellElement selectedCellElem;

        #endregion Protected Fields

        #region Public Constructors

        public MainForm()
        {
            this.InitializeComponent();

            this.splitContainer1.Panel2MinSize = this.splitContainer1.Panel2.Width;
            this.cellListBox.DoubleClick += this.CellListBoxDoubleClick;
            this.sizeTxtBox.Text = this.cellSize.ToString(CultureInfo.InvariantCulture);
        }

        #endregion Public Constructors

        #region Protected Methods

        protected virtual void AddCellElement(ICellElement cellElem)
        {
            if (!this.environmentControl.Environment.CellElements.ContainsKey(cellElem.IdToken))
                this.environmentControl.Environment.CellElements.Add(cellElem.IdToken, cellElem);

            this.cellElemList[cellElem] = new HashSet<Cell>();
            this.cellListBox.Items.Add(cellElem);
        }

        protected virtual void AddCellElement(ICellElement cellElem, Cell cell)
        {
            if ((cell != null) && (!cell.Elements.Contains(cellElem)))
                cell.Elements.Add(cellElem);

            if ((cell != null) && cell.Environment.CellElements.ContainsKey(cellElem.IdToken) &&
                !this.cellElemList[cellElem].Contains(cell))
                this.cellElemList[cellElem].Add(cell);

            this.RefreshForm(false);
        }

        protected virtual void AddCellElements(IEnvironment environment)
        {
            this.cellElemList.Clear();
            this.cellListBox.Items.Clear();
            foreach (var cellElem in environment.CellElements.Values)
                this.AddCellElement(cellElem);
            foreach (var cell in environment.Cells)
                this.AddCellElements(cell);
        }

        protected virtual void AddCellElements(Cell cell)
        {
            foreach (var cellElem in cell.Elements)
                this.AddCellElement(cellElem, cell);
        }

        protected virtual void CalculateSelectedCells()
        {
            if (this.initialSelectCell == null) return;

            //just one cell selected
            if ((this.finalSelectCell == null) || (this.finalSelectCell.Equals(this.initialSelectCell)))
            {
                this.selectedCells.Add(this.initialSelectCell);
                return;
            }

            //corrects cell order
            var minX = Math.Min(this.initialSelectCell.XCoord, this.finalSelectCell.XCoord);
            var maxX = Math.Max(this.initialSelectCell.XCoord, this.finalSelectCell.XCoord);
            var minY = Math.Min(this.initialSelectCell.YCoord, this.finalSelectCell.YCoord);
            var maxY = Math.Max(this.initialSelectCell.YCoord, this.finalSelectCell.YCoord);

            //calculates range of selected cells
            for (var x = minX; x <= maxX; x++)
                for (var y = minY; y <= maxY; y++)
                    this.selectedCells.Add(this.environmentControl.Environment.Cells[x, y]);
        }

        protected virtual void ClearCellElements(Cell cell)
        {
            foreach (var cellElem in this.GetCellElements(cell).ToList())
                this.DeleteCellElement(cellElem, cell);
        }

        protected virtual void CreateEnvironment(SizeSelectorForm sizeSeclector)
        {
            var environment = new TestEnvironment();
            environment.CreateCells(sizeSeclector.NumRows + 2, sizeSeclector.NumCols + 2);
            this.SetWallsAroundEnvironment(environment);
            this.LoadEnvironment(environment);
        }

        protected virtual void CreateEnvironmentControl(IEnvironment environment)
        {
            if (this.environmentControl != null) this.environmentControl.Dispose();
            this.environmentControl = new EnvironmentControl(environment, this.envPanel, this.cellSize, false, false);
            foreach (var cellControl in this.environmentControl.CellControls.Values)
            {
                cellControl.MouseClick += this.CellControlMouseClick;
                cellControl.MouseDoubleClick += this.CellControlMouseDoubleClick;
                cellControl.Paint += this.CellControlPaint;
            }
        }

        protected virtual ToolStripMenuItem CreateMenuStripItem(ICellElement allSelectedCellsElem)
        {
            return new ToolStripMenuItem(allSelectedCellsElem.ToString()) { Tag = allSelectedCellsElem };
        }

        protected virtual void DeleteAllCellElements()
        {
            foreach (var cellElem in this.cellElemList.Keys.ToList())
                this.DeleteCellElement(cellElem);
        }

        protected virtual void DeleteCellElement(ICellElement cellElem)
        {
            foreach (var cell in this.cellElemList[cellElem])
                cell.Elements.Remove(cellElem);

            this.environmentControl.Environment.CellElements.Remove(cellElem.IdToken);
            this.cellElemList.Remove(cellElem);
            this.cellListBox.Items.Remove(cellElem);
            this.RefreshForm(false);
        }

        protected virtual void DeleteCellElement(ICellElement cellElem, Cell cell)
        {
            if (cell.Elements.Contains(cellElem))
                cell.Elements.Remove(cellElem);
            this.cellElemList[cellElem].Remove(cell);
        }

        protected virtual HashSet<ICellElement> GetAllSelectedCellsElements()
        {
            var allCellsElems = new HashSet<ICellElement>();
            foreach (var cell in this.selectedCells)
                foreach (var cellElem in this.GetCellElements(cell))
                    if (!allCellsElems.Contains(cellElem))
                        allCellsElems.Add(cellElem);
            return allCellsElems;
        }

        protected virtual IEnumerable<ICellElement> GetCellElements(Cell cell)
        {
            return cell.Elements.Where(cellElem => cell.Environment.CellElements.ContainsKey(cellElem.IdToken));
        }

        protected virtual void RefreshForm(bool changeSize)
        {
            if (changeSize)
            {
                this.Height = ((cellSize + 1) * (int)this.environmentControl.Environment.Rows) + 44;
                var minWidth = this.splitContainer1.Width;
                this.Width = Math.Max(minWidth,
                    (cellSize + 1) * (int)this.environmentControl.Environment.Cols +
                    this.splitContainer1.Panel2.Width + 7);
            }
            this.environmentControl.Control.Invalidate();
        }

        protected virtual void SelectedCellsAddElement(ICellElement cellElement)
        {
            if (cellElement != null)
                foreach (var cell in this.selectedCells)
                    this.AddCellElement(cellElement, cell);
            this.RefreshForm(false);
        }

        protected virtual void SelectedCellsDeleteElement(ICellElement cellElement)
        {
            if (cellElement != null)
                foreach (var cell in this.selectedCells)
                    this.DeleteCellElement(cellElement, cell);
            this.RefreshForm(false);
        }

        protected virtual void SelectedCellsSignalRepaint()
        {
            foreach (var cell in this.selectedCells)
            {
                var cellControl = this.environmentControl.CellControls[cell];
                cellControl.ForceRepaint = true;
            }
        }

        protected virtual void SetWallsAroundEnvironment(IEnvironment environment)
        {
            environment.CellElements.Add(this.wallElement.IdToken, this.wallElement);
            for (var col = 0; col < environment.Cols; col++)
            {
                environment.Cells[col, 0].Elements.Add(this.wallElement);
                environment.Cells[col, environment.Rows - 1].Elements.Add(this.wallElement);
            }

            for (var row = 1; row < environment.Rows - 1; row++)
            {
                environment.Cells[0, row].Elements.Add(this.wallElement);
                environment.Cells[environment.Cols - 1, row].Elements.Add(this.wallElement);
            }
        }

        protected virtual void ShowForm(Form form)
        {
            form.StartPosition = FormStartPosition.CenterParent;
            form.Show();

            this.Enabled = false;
            while (!form.IsDisposed)
                Application.DoEvents();
            this.Enabled = true;

            this.BringToFront();
        }

        protected virtual void UpdateCellContextMenu()
        {
            this.selectedCellsToolStripMenuItem.Enabled = this.selectedCells.Count > 0;
            this.addSelectedElementToolStripMenuItem.Enabled = this.selectedCellElem != null;

            var allSelectedCellsElems = this.GetAllSelectedCellsElements();

            this.removeToolStripMenuItem.DropDownItems.Clear();
            foreach (var cellElem in allSelectedCellsElems)
            {
                var menuItem = this.CreateMenuStripItem(cellElem);
                this.removeToolStripMenuItem.DropDownItems.Add(menuItem);
                menuItem.Click += this.RemoveCellElementClick;
            }

            var count = this.addToolStripMenuItem.DropDownItems.Count - 2;
            for (var i = 0; i < count; i++)
                this.addToolStripMenuItem.DropDownItems.RemoveAt(2);

            foreach (var cellElem in this.cellElemList.Keys)
            {
                var menuItem = this.CreateMenuStripItem(cellElem);
                this.addToolStripMenuItem.DropDownItems.Add(menuItem);
                menuItem.Click += this.AddCellElementClick;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddCellElementClick(object sender, EventArgs e)
        {
            this.SelectedCellsAddElement(((ToolStripMenuItem)sender).Tag as ICellElement);
        }

        private void AddSelectedElementToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.SelectedCellsAddElement(this.selectedCellElem);
        }

        private void CellControlMouseClick(object sender, MouseEventArgs e)
        {
            if (!e.Button.Equals(MouseButtons.Left)) return;

            this.SelectedCellsSignalRepaint();
            this.selectedCells.Clear();

            this.selectedCells.Add(((CellControl)sender).Cell);
            this.SelectedCellsSignalRepaint();

            this.RefreshForm(false);
        }

        private void CellControlMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!e.Button.Equals(MouseButtons.Left)) return;

            if (this.initialSelectCell == null)
            {
                // verifies first middle button press -> first cell selected
                this.SelectedCellsSignalRepaint();
                this.selectedCells.Clear();
                this.initialSelectCell = ((CellControl)sender).Cell;
            }
            else
            {
                // verifies second middle button press -> final cell selected
                this.finalSelectCell = ((CellControl)sender).Cell;

                this.CalculateSelectedCells();
                this.SelectedCellsSignalRepaint();
                this.RefreshForm(false);

                this.initialSelectCell = this.finalSelectCell = null;
            }
        }

        private void CellControlPaint(object sender, PaintEventArgs e)
        {
            var cellControl = ((CellControl)sender);
            if (!this.selectedCells.Contains(cellControl.Cell)) return;
            e.Graphics.DrawRectangle(new Pen(Color.Red, 10), e.ClipRectangle);
        }

        private void CellListBoxDoubleClick(object sender, EventArgs e)
        {
            this.EditToolStripMenuItemClick(sender, EventArgs.Empty);
        }

        private void CellListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedCellElem = this.cellListBox.SelectedItem as ICellElement;
        }

        private void CellsContextMenuStripOpening(object sender, CancelEventArgs e)
        {
            if (this.environmentControl == null)
            {
                e.Cancel = true;
                return;
            }
            this.UpdateCellContextMenu();
        }

        private void ClearElementsToolStripMenuItemClick(object sender, EventArgs e)
        {
            foreach (var cell in this.selectedCells)
                this.ClearCellElements(cell);
        }

        private void ClearSelectionToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.SelectedCellsSignalRepaint();
            this.selectedCells.Clear();
            this.RefreshForm(false);
        }

        private void CopyAllElementsToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.copiedElements.Clear();
            this.copiedElements.UnionWith(this.GetAllSelectedCellsElements());

            this.pasteElementsToolStripMenuItem.Enabled = true;
        }

        private void DeleteAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.DeleteAllCellElements();
            this.RefreshForm(false);
        }

        private void DeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.selectedCellElem != null)
                this.DeleteCellElement(this.selectedCellElem);
            this.RefreshForm(false);
        }

        private void EditDescToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.selectedCells.Count == 0) return;
            this.ShowForm(new CellDescriptionForm(this.selectedCells));
        }

        private void EditToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.selectedCellElem == null) return;
            this.ShowForm(new CellElementForm(this.selectedCellElem));
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadEnvironment(IEnvironment environment)
        {
            this.CreateEnvironmentControl(environment);
            this.selectedCellElem = null;
            this.selectedCells.Clear();
            this.AddCellElements(environment);
            this.RefreshForm(true);

            //enables menus
            this.printToolStripMenuItem.Enabled =
                this.saveToolStripMenuItem.Enabled =
                    this.saveAsToolStripMenuItem.Enabled =
                        this.cellMenuStrip.Enabled = true;
        }

        private void NewCellToolStripMenuItemClick(object sender, EventArgs e)
        {
            var form = new CellElementForm(null);
            this.ShowForm(form);

            if (form.DialogResult != DialogResult.OK)
                return;

            this.AddCellElement(form.CellElement);
        }

        private void NewToolStripMenuItemClick(object sender, EventArgs e)
        {
            var sizeSelectorForm = new SizeSelectorForm();
            this.ShowForm(sizeSelectorForm);

            if (sizeSelectorForm.DialogResult != DialogResult.OK)
                return;

            this.CreateEnvironment(sizeSelectorForm);
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() != DialogResult.OK) return;

            var environment = new TestEnvironment();
            environment.LoadFromXmlFile(this.openFileDialog.FileName);
            this.LoadEnvironment(environment);
        }

        private void PasteElementsToolStripMenuItemClick(object sender, EventArgs e)
        {
            foreach (var cell in this.selectedCells)
                foreach (var cellElem in this.copiedElements)
                    this.AddCellElement(cellElem, cell);

            this.copiedElements.Clear();
            this.pasteElementsToolStripMenuItem.Enabled = false;
        }

        private void PrintToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.environmentControl != null)
                this.environmentControl.SaveToImage(Path.GetFullPath("./environment.png"));
        }

        private void RemoveCellElementClick(object sender, EventArgs e)
        {
            this.SelectedCellsDeleteElement(((ToolStripMenuItem)sender).Tag as ICellElement);
        }

        private void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() != DialogResult.OK) return;

            this.openFileDialog.FileName = this.saveFileDialog.FileName;
            this.SaveToolStripMenuItemClick(sender, e);
        }

        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.openFileDialog.FileName))
            {
                this.SaveAsToolStripMenuItemClick(sender, e);
                return;
            }

            this.environmentControl.Environment.SaveToXmlFile(this.openFileDialog.FileName);
        }

        private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.selectedCells.Clear();
            foreach (var cell in this.environmentControl.Environment.Cells)
                this.selectedCells.Add(cell);
            this.SelectedCellsSignalRepaint();
            this.RefreshForm(false);
        }

        private void SizeTxtBoxClick(object sender, EventArgs e)
        {
            int newCellSize;
            var result = int.TryParse(this.sizeTxtBox.Text, out newCellSize);

            if (result)
                newCellSize = Math.Max(MIN_CELL_SIZE, newCellSize);

            if (!result || this.cellSize.Equals(newCellSize))
                return;

            //reloads environment with new size
            this.cellSize = newCellSize;
            this.LoadEnvironment(this.environmentControl.Environment);
        }

        #endregion Private Methods
    }
}