// ------------------------------------------
// <copyright file="SmellPerceptionManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.Emotions

//    Last updated: 10/9/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Linq;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.Managers.Perception;

namespace PS.Learning.IMRL.Emotions.Domain.Managers
{
    [Serializable]
    public class SmellPerceptionManager : PerceptionManager
    {
        public const string SMELL_STR = "smell";

        public SmellPerceptionManager(CellAgent agent) : base(agent)
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
            //adds cell contents (including cell location)
            foreach (var cellElement in this.Agent.Cell.Elements.Where(
                cellElement => !cellElement.Equals(this.Agent) && !(cellElement is InfoCellElement)))
                this.CurrentSensations.Add(cellElement.IdToken);

            //adds smell sensations for neighbour cells
            this.AddNeighbourSensations();
        }

        protected virtual void AddNeighbourSensations()
        {
            foreach (var neighbourCell in this.Agent.Cell.NeighbourCells.Values)
                this.AddNeighbourSensation(neighbourCell);
        }


        protected virtual void AddNeighbourSensation(Cell neighbourCell)
        {
            //adds smell sensations for neighbour cells with smell
            if (neighbourCell != null)
                foreach (var cellElement in neighbourCell.Elements.Where(cellElement => cellElement.HasSmell))
                    this.CurrentSensations.Add(string.Format("{0}-{1}", SMELL_STR, cellElement.IdToken));
        }
    }
}