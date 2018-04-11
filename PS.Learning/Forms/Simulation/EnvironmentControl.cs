// ------------------------------------------
// <copyright file="EnvironmentControl.cs" company="Pedro Sequeira">
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
//    Project: Learning.Forms

//    Last updated: 12/9/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.Environments;

namespace PS.Learning.Forms.Simulation
{
    public class EnvironmentControl : IControlable, IDisposable
    {
        #region Constructors

        public EnvironmentControl(IEnvironment environment, Panel control, int cellSize, bool limitedUser, bool cellView)
        {
            this.CellControls = new Dictionary<Cell, CellControl>();
            this.CellView = cellView;
            this._control = control;
            this._limitedUser = limitedUser;
            //this._control.BackColor = Color.FromArgb(0, 0, 0);
            this._control.Paint += this.ControlOnPaint;
            this.Environment = environment;
            this.CellSize = cellSize;
            this.CreateCellControls();
        }

        #endregion

        #region Fields

        private readonly Panel _control;
        private readonly bool _limitedUser;
        private Graphics _previousGraphics;
        private bool _visibleCells = true;
        private readonly Dictionary<ICellElement, Bitmap> _elementBitmaps = new Dictionary<ICellElement, Bitmap>();
        private readonly Dictionary<ICellElement, string> _elementBitmapPaths = new Dictionary<ICellElement, string>();

        #endregion

        #region Properties

        public IEnvironment Environment { get; private set; }

        public int CellSize { get; protected set; }

        public bool CellView { get; protected set; }

        public bool VisibleCells
        {
            set
            {
                this._visibleCells = value;
                foreach (var cellControl in this.CellControls.Values)
                    cellControl.Cell.Visible = value;
            }
            get { return this._visibleCells; }
        }

        public Dictionary<Cell, CellControl> CellControls { get; private set; }

        public Control Control
        {
            get { return this._control; }
        }

        #endregion

        #region Public Methods

        public virtual void Dispose()
        {
            //this.Environment.Dispose();
            foreach (var cellControl in this.CellControls.Values)
            {
                cellControl.Dispose();
                this._control.Controls.Remove(cellControl);
            }
            this.CellControls.Clear();
            this._control.Paint -= this.ControlOnPaint;
            this._control.Controls.Clear();

            foreach (var bitmap in this._elementBitmaps.Values)
                bitmap.Dispose();
            this._elementBitmaps.Clear();
            this._elementBitmapPaths.Clear();
        }

        public Bitmap GetElementBitmap(ICellElement element)
        {
            //check new image (not loaded or path changed)
            if (!this._elementBitmapPaths.ContainsKey(element) ||
                element.ImagePath != this._elementBitmapPaths[element])
            {
                Bitmap bitmap = null;
                if (File.Exists(element.ImagePath))
                    bitmap = (Bitmap) Image.FromFile(element.ImagePath);
                if (this._elementBitmaps.ContainsKey(element) && (this._elementBitmaps[element] != null))
                    this._elementBitmaps[element].Dispose();

                this._elementBitmaps[element] = bitmap;
                this._elementBitmapPaths[element] = element.ImagePath;
            }
            return this._elementBitmaps[element];
        }

        public void SaveToImage(string path)
        {
            if (this._previousGraphics == null) return;

            var bitmap = new Bitmap(this.Control.Width, this.Control.Height);
            this.Control.DrawToBitmap(bitmap, new Rectangle(0, 0, this.Control.Width, this.Control.Height));
            bitmap.Save(path, ImageFormat.Png);
        }

        public void UnPrintBlack()
        {
            foreach (var cellControl in this.CellControls.Values)
                cellControl.Cell.Visible = true;
        }

        #endregion

        #region Protected Methods

        protected virtual void CreateCellControls()
        {
            //creates cell control for each cell
            foreach (var cell in this.Environment.Cells)
            {
                cell.Visible = !this._limitedUser;
                cell.CellLocation.Visible = this._limitedUser && this.CellView;
                this.CellControls.Add(cell, this._limitedUser
                    ? new LimitedUserCellControl(cell, this, this.CellSize, this.CellView)
                    : new CellControl(cell, this, this.CellSize));
            }
        }

        protected void ControlOnPaint(object sender, PaintEventArgs e)
        {
            //paints cells
            this._previousGraphics = e.Graphics;
            foreach (var cellControl in this.CellControls.Values)
                if (cellControl.NeedsRepaint() || this.Environment.DebugMode)
                    cellControl.Invalidate(true);
        }

        #endregion
    }
}