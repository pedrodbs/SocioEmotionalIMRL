// ------------------------------------------
// <copyright file="NeighbourCellPerceptionManager.cs" company="Pedro Sequeira">
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
//    Project: Learning
//    Last updated: 10/9/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;

namespace PS.Learning.Core.Domain.Managers.Perception
{
    [Serializable]
    public class NeighbourCellPerceptionManager : PerceptionManager
    {
        protected const string DOWN_SENSATION = "Down";
        protected const string LEFT_SENSATION = "Left";
        protected const string RIGHT_SENSATION = "Right";
        protected const string UP_SENSATION = "Up";

        public NeighbourCellPerceptionManager(CellAgent agent) : base(agent)
        {
        }

        public new CellAgent Agent
        {
            get { return base.Agent as CellAgent; }
        }

        public override void PrintResults(string path)
        {
        }

        protected override void AddInternalSensations()
        {
        }

        protected override void AddExternalSensations()
        {
            //adds cell contents
            foreach (var cellElement in this.Agent.Cell.Elements)
                if (!cellElement.Equals(this.Agent) && !(cellElement is InfoCellElement))
                    this.CurrentSensations.Add(cellElement.IdToken);

            //adds neighbour sensations
            this.AddNeighbourSensations();
        }

        protected virtual void AddNeighbourSensations()
        {
            var upCell = (this.Agent.Cell.YCoord > 0)
                             ? this.Agent.Environment.Cells[this.Agent.Cell.XCoord, this.Agent.Cell.YCoord - 1]
                             : null;
            var downCell = (this.Agent.Cell.YCoord < this.Agent.Environment.Rows - 1)
                               ? this.Agent.Environment.Cells[this.Agent.Cell.XCoord, this.Agent.Cell.YCoord + 1]
                               : null;
            var leftCell = (this.Agent.Cell.XCoord > 0)
                               ? this.Agent.Environment.Cells[this.Agent.Cell.XCoord - 1, this.Agent.Cell.YCoord]
                               : null;
            var rightCell = (this.Agent.Cell.XCoord < this.Agent.Environment.Cols - 1)
                                ? this.Agent.Environment.Cells[this.Agent.Cell.XCoord + 1, this.Agent.Cell.YCoord]
                                : null;

            this.AddNeighbourCell(upCell, UP_SENSATION);
            this.AddNeighbourCell(downCell, DOWN_SENSATION);
            this.AddNeighbourCell(leftCell, LEFT_SENSATION);
            this.AddNeighbourCell(rightCell, RIGHT_SENSATION);
        }

        protected virtual void AddNeighbourCell(Cell neighbourCell, string directionSensation)
        {
            if (neighbourCell == null)
                this.CurrentSensations.Add("see Wall " + directionSensation);
            else
                foreach (var cellContent in neighbourCell.Elements)
                    if (cellContent.Visible)
                        this.CurrentSensations.Add("see " + cellContent.IdToken + " " + directionSensation);
        }
    }
}