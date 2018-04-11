// ------------------------------------------
// <copyright file="Cell.cs" company="Pedro Sequeira">
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

using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Environments;
using PS.Utilities.IO.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace PS.Learning.Core.Domain.Cells
{
    [Serializable]
    public class Cell : XmlResource
    {
        #region Public Fields

        public const string DOWN_DIR_STR = "down";

        public const string LEFT_DIR_STR = "left";

        public const string RIGHT_DIR_STR = "right";

        public const string UP_DIR_STR = "up";

        #endregion Public Fields

        #region Private Fields

        private const string CONTENTS_TAG = "contents";

        private const string ID_TAG = "id";

        private const string XCOORD_TAG = "x-coord";

        private const string YCOORD_TAG = "y-coord";

        #endregion Private Fields

        #region Public Constructors

        public Cell(IEnvironment environment)
                                                                            : base("cell")
        {
            this.ElementIDs = new List<string>();
            this.Environment = environment;
        }

        #endregion Public Constructors

        #region Public Properties

        public Location CellLocation { get; private set; }
        public List<string> ElementIDs { get; }
        public HashSet<ICellElement> Elements { get; private set; }
        public IEnvironment Environment { get; set; }
        public Dictionary<string, Cell> NeighbourCells { get; protected set; }

        public InfoCellElement StateValue { get; private set; }
        public bool Visible { get; set; }
        public int XCoord { get; set; }
        public int YCoord { get; set; }

        #endregion Public Properties

        #region Public Methods

        public bool ContainsElement(string elementID)
        {
            return this.Elements.Any(cellElement => cellElement.IdToken.Equals(elementID));
        }

        public override void DeserializeXML(XmlElement element)
        {
            base.DeserializeXML(element);

            this.YCoord = int.Parse(element.GetAttribute(YCOORD_TAG), CultureInfo.InvariantCulture);
            this.XCoord = int.Parse(element.GetAttribute(XCOORD_TAG), CultureInfo.InvariantCulture);

            foreach (XmlElement childElement in element.SelectNodes(CONTENTS_TAG + "/" + ID_TAG))
            {
                var contentID = childElement.InnerXml.ToLower();
                if (contentID != "") this.ElementIDs.Add(contentID);
            }

            this.InitElements();
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var cellElement in this.Elements)
                if (!(cellElement is Agent))
                    cellElement.Dispose();
            this.Elements.Clear();
        }

        public ICellElement GetElement(string elementID)
        {
            return this.Elements.FirstOrDefault(element => element.IdToken == elementID);
        }

        public override void InitElements()
        {
            this.Elements = new HashSet<ICellElement>();
            this.CellLocation = new Location { IdToken = this.XCoord + "*" + this.YCoord, Cell = this, Visible = false };
            this.IdToken = this.CellLocation.IdToken;
            this.StateValue = new InfoCellElement
            {
                IdToken = "StVal",
                Color = new ColorInfo(0, 100, 0),
                Cell = this,
                Visible = false
            };
            this.Visible = true;
        }

        public override void SerializeXML(XmlElement element)
        {
            base.SerializeXML(element);

            element.SetAttribute(YCOORD_TAG, this.YCoord.ToString(CultureInfo.InvariantCulture));
            element.SetAttribute(XCOORD_TAG, this.XCoord.ToString(CultureInfo.InvariantCulture));

            var childElement = element.OwnerDocument.CreateElement(CONTENTS_TAG);
            element.AppendChild(childElement);
            foreach (var content in this.Elements)
            {
                if ((content is Location) || (content is InfoCellElement)) continue;

                var newChild = element.OwnerDocument.CreateElement(ID_TAG);
                newChild.InnerXml = content.IdToken;
                childElement.AppendChild(newChild);
            }
        }

        public void SetNeighbourCells()
        {
            this.NeighbourCells = new Dictionary<string, Cell>();

            //gets all cells in up, down, left, right directions, if possible
            if (this.YCoord > 0)
                this.NeighbourCells.Add(UP_DIR_STR, this.Environment.Cells[this.XCoord, this.YCoord - 1]);
            if (this.YCoord < this.Environment.Rows - 1)
                this.NeighbourCells.Add(DOWN_DIR_STR, this.Environment.Cells[this.XCoord, this.YCoord + 1]);
            if (this.XCoord > 0)
                this.NeighbourCells.Add(LEFT_DIR_STR, this.Environment.Cells[this.XCoord - 1, this.YCoord]);
            if (this.XCoord < this.Environment.Cols - 1)
                this.NeighbourCells.Add(RIGHT_DIR_STR, this.Environment.Cells[this.XCoord + 1, this.YCoord]);
        }

        public override string ToString()
        {
            return this.CellLocation.ToString();
        }

        #endregion Public Methods
    }
}