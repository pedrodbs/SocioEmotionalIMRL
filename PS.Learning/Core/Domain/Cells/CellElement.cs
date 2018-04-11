// ------------------------------------------
// <copyright file="CellElement.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.Core

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Utilities.IO.Serialization;
using System;
using System.Globalization;
using System.Xml;

namespace PS.Learning.Core.Domain.Cells
{
    [Serializable]
    public class CellElement : Sensation, ICellElement
    {
        #region Protected Fields

        //private Bitmap _image;
        protected Cell cell;

        #endregion Protected Fields

        #region Private Fields

        private const string COLOR_TAG = "color";

        private const string IMAGE_PATH_TAG = "image-path";

        private const string SMELL_TAG = "smell";

        private const string VISIBLE_TAG = "visible";

        private const string WALKABLE_TAG = "walkable";

        #endregion Private Fields

        #region Public Constructors

        public CellElement()
        {
            this.Description = "";
            this.Visible = true;
            this.Walkable = true;
            this.Color = new ColorInfo(0, 0, 0);
        }

        #endregion Public Constructors

        #region Public Properties

        public virtual Cell Cell
        {
            get { return this.cell; }
            set
            {
                if (Equals(value, this.cell)) return;

                if (this.cell != null) this.cell.Elements.Remove(this);
                this.cell = value;

                if (value == null) return;

                this.cell.Elements.Add(this);
                if ((this.cell.Environment != null) && !(this is Location) && !(this is InfoCellElement) &&
                    !this.cell.Environment.CellElements.ContainsKey(this.IdToken))
                    this.cell.Environment.CellElements.Add(this.IdToken, this);
            }
        }

        public ColorInfo Color { get; set; }
        public bool ForceRepaint { get; set; }
        public bool HasSmell { get; set; }
        public virtual string ImagePath { get; set; }

        public bool Visible { get; set; }

        public bool Walkable { get; set; }

        #endregion Public Properties

        #region Public Methods

        public virtual ICellElement Clone()
        {
            return new CellElement
            {
                Color = this.Color,
                ImagePath = this.ImagePath,
                Reward = this.Reward,
                IdToken = this.IdToken,
                Visible = this.Visible,
                Walkable = this.Walkable,
                HasSmell = this.HasSmell,
                Description = this.Description
            };
        }

        public override void DeserializeXML(XmlElement element)
        {
            base.DeserializeXML(element);

            bool hasSmell, visible, walkable;
            bool.TryParse(element.GetAttribute(VISIBLE_TAG), out visible);
            this.Visible = visible;
            bool.TryParse(element.GetAttribute(SMELL_TAG), out hasSmell);
            this.HasSmell = hasSmell;
            bool.TryParse(element.GetAttribute(WALKABLE_TAG), out walkable);
            this.Walkable = walkable;
            this.ImagePath = element.GetAttribute(IMAGE_PATH_TAG);

            var childElement = (XmlElement)element.SelectSingleNode(COLOR_TAG);
            if (childElement != null) this.Color.DeserializeXML(childElement);
        }

        public override void Dispose()
        {
            base.Dispose();
            //if (this.Image != null) this.Image.Dispose();
        }

        public bool Equals(ICellElement cellElement)
        {
            return cellElement is CellElement &&
                   cellElement.IdToken == this.IdToken &&
                   cellElement.Color.Equals(this.Color) &&
                   cellElement.ImagePath == this.ImagePath &&
                   ((CellElement)cellElement).Reward.Equals(this.Reward) &&
                   cellElement.Visible == this.Visible &&
                   cellElement.Walkable == this.Walkable &&
                   cellElement.HasSmell == this.HasSmell;
        }

        public override void SerializeXML(XmlElement element)
        {
            base.SerializeXML(element);

            element.SetAttribute(VISIBLE_TAG, this.Visible.ToString(CultureInfo.InvariantCulture));
            element.SetAttribute(WALKABLE_TAG, this.Walkable.ToString(CultureInfo.InvariantCulture));
            element.SetAttribute(SMELL_TAG, this.HasSmell.ToString(CultureInfo.InvariantCulture));
            element.SetAttribute(IMAGE_PATH_TAG, this.ImagePath);

            var childElement = element.OwnerDocument.CreateElement(COLOR_TAG);
            this.Color.SerializeXML(childElement);
            element.AppendChild(childElement);
        }

        public override string ToString()
        {
            return this.IdToken;
        }

        #endregion Public Methods
    }
}