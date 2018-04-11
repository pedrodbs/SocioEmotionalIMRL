// ------------------------------------------
// <copyright file="PacmanPerceptionManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.Tests.EmotionalOptimization

//    Last updated: 12/3/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.Managers.Perception;
using PS.Learning.Tests.EmotionalOptimization.Domain.Environments;

namespace PS.Learning.Tests.EmotionalOptimization.Domain.Managers
{
    public class PacmanPerceptionManager : CellPerceptionManager
    {
        public PacmanPerceptionManager(ICellAgent agent) : base(agent)
        {
        }

        protected override void AddExternalSensations()
        {
            //agent sees current location and other visible elements
            this.CurrentSensations.Add(this.Agent.Cell.CellLocation.IdToken);
            foreach (var cellElement in this.Agent.Cell.Elements)
                if (cellElement.Visible && !cellElement.Equals(this.Agent) && !(cellElement is InfoCellElement))
                    this.CurrentSensations.Add(cellElement.IdToken);

            //detect elements in corridors
            this.DetectInCorridor(PacmanEnvironment.GHOST_ID, PacmanEnvironment.GHOST_ID);
            this.DetectInCorridor(PacmanEnvironment.DOT_ID, PacmanEnvironment.DOT_ID);
            this.DetectInCorridor(PacmanEnvironment.BIG_DOT_ID, PacmanEnvironment.DOT_ID);
            this.DetectInCorridor(PacmanEnvironment.FINAL_GHOST_ID, PacmanEnvironment.GHOST_ID);
            this.DetectInCorridor(PacmanEnvironment.DEADLY_GHOST_ID, PacmanEnvironment.GHOST_ID);
            this.DetectInCorridor(PacmanEnvironment.WEAK_GHOST_ID, PacmanEnvironment.GHOST_ID);
            this.DetectInCorridor(PacmanEnvironment.FINAL_DOT_ID, PacmanEnvironment.DOT_ID);
        }
    }
}