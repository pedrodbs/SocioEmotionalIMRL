// ------------------------------------------
// <copyright file="CellControl.cs" company="Pedro Sequeira">
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
using AForge.Imaging.Filters;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Utilities.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Image = AForge.Imaging.Image;

namespace PS.Learning.Forms.Simulation
{
    public class CellControl : Panel
    {
        #region Private Fields

        private readonly int _cellSize;

        private readonly EnvironmentControl _environmentControl;

        private bool _forceRepaint = true;

        private HashSet<ICellElement> _previousElements;

        #endregion Private Fields

        #region Public Constructors

        public CellControl(Cell cell, EnvironmentControl environmentControl, int cellSize)
        {
            this.Cell = cell;
            this._cellSize = cellSize;
            this._environmentControl = environmentControl;
            this.InitElements();
        }

        #endregion Public Constructors

        #region Public Properties

        public Cell Cell { get; private set; }

        public bool ForceRepaint
        {
            set { this._forceRepaint = value; }
        }

        #endregion Public Properties

        #region Public Methods

        public virtual void InitElements()
        {
            this.BackColor = this._environmentControl.Environment.Color.ToColor();
            this.ForeColor = Color.FromArgb(0, 0, 0);
            this.Size = new Size(_cellSize, _cellSize);
            //this.Location = new Point((_cellSize + 1) * this.Cell.XCoord, (_cellSize + 1) * this.Cell.YCoord);
            this.Location = new Point(_cellSize * this.Cell.XCoord, _cellSize * this.Cell.YCoord);
            this.BringToFront();
            this.Parent = this._environmentControl.Control;
            this._environmentControl.Control.Controls.Add(this);
        }

        public bool NeedsRepaint()
        {
            var forceRepaint = this._forceRepaint;
            this.ForceRepaint = false;

            //checks conditions for repaint
            forceRepaint = forceRepaint ||
                           (this._previousElements == null) ||
                           (this.Cell.Elements.Count != this._previousElements.Count) ||
                           this.Cell.Elements.Any(
                               element => element.ForceRepaint || !this._previousElements.Contains(element));

            this._previousElements = new HashSet<ICellElement>(this.Cell.Elements);

            return forceRepaint;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            //this._environmentControl.Control.Controls.Remove(this);
            this.Visible = false;
            base.Dispose(disposing);
        }

        protected void DrawFinishedTask(ICellElement cellElement, Graphics graphics)
        {
            if (!(cellElement is CellAgent)) return;

            //checks if agent finished task
            var agent = (CellAgent)cellElement;
            var agentFinishedTask = agent.Environment.AgentFinishedTask(
                agent, agent.ShortTermMemory.PreviousState, agent.ShortTermMemory.CurrentAction);
            if (agentFinishedTask)
                graphics.DrawRectangle(new Pen(Color.Red, 10), new Rectangle(new Point(0, 0), this.Size));
        }

        protected void DrawImages(List<Bitmap> imageList, Graphics graphics)
        {
            if (imageList.Count == 0) return;

            var mergedImage = Image.Clone(imageList[0], PixelFormat.Format24bppRgb);
            var filter = new Morph();

            //merges all images in the list
            for (var i = 1; i < imageList.Count; i++)
            {
                //sets filter parameters
                filter.OverlayImage = mergedImage;
                filter.SourcePercent = 1.0 / (i + 1.0);

                //applies morph (merge) filter
                var newImage = Image.Clone(imageList[i], PixelFormat.Format24bppRgb);
                //var newMergedImage = filter.Apply(newImage);

                //disposes of images created
                mergedImage.Dispose();
                newImage.Dispose();
                //mergedImage = newMergedImage;
            }

            //draws resulting image in graphics
            graphics.DrawImage(mergedImage, new Rectangle(new Point(0, 0), this.Size));
            mergedImage.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //don't display cell unless visible
            if (this.Cell.Visible)
            {
                this.PaintContents(e);
            }
            else
            {
                this.BackColor = Color.FromArgb(0, 0, 0);
                this.ForceRepaint = true;
            }

            base.OnPaint(e);
        }

        protected void PaintContents(PaintEventArgs e)
        {
            this.BackColor = this._environmentControl.Environment.Color.ToColor();
            var elements = new List<ICellElement>(this.Cell.Elements);

            var imageList = new List<Bitmap>();

            for (var i = 0; i < elements.Count; i++)
            {
                var cellElement = elements[i];
                cellElement.ForceRepaint = false;

                //if not visible, don't draw content
                if (!cellElement.Visible) continue;

                //adds image to list or draws text
                var bitmap = this._environmentControl.GetElementBitmap(cellElement);
                if (bitmap != null)
                    //imageList.Add(cellElement.Image);
                    e.Graphics.DrawImage(bitmap, new Rectangle(new Point(0, 0), this.Size));
                else
                    //e.Graphics.DrawString(cellElement.IdToken, new Font("Arial", 7), cellElement.Brush, 0.3f, 12*i);
                    e.Graphics.FillRectangle(new SolidBrush(cellElement.Color.ToColor()),
                        new Rectangle(new Point(0, 0), this.Size));

                this.DrawFinishedTask(cellElement, e.Graphics);
            }

            this.DrawImages(imageList, e.Graphics);
        }

        #endregion Protected Methods
    }
}