// ------------------------------------------
// <copyright file="CellPerceptionManager.cs" company="Pedro Sequeira">
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
//    Last updated: 12/3/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.States;

namespace PS.Learning.Core.Domain.Managers.Perception
{
    [Serializable]
    public class CellPerceptionManager : PerceptionManager
    {
        public CellPerceptionManager(ICellAgent agent) : base(agent)
        {
        }

        public new ICellAgent Agent
        {
            get { return base.Agent as CellAgent; }
        }

        protected override void AddInternalSensations()
        {
        }

        protected override void AddExternalSensations()
        {
            //adds cell contents (including cell location)
            foreach (var cellElement in this.Agent.Cell.Elements)
                if ((cellElement.Visible || (cellElement is Location))
                    && !cellElement.Equals(this.Agent) && !(cellElement is InfoCellElement))
                    this.CurrentSensations.Add(cellElement.IdToken);
        }

        public override void PrintResults(string path)
        {
        }

        public virtual Cell GetCellFromState(IState state)
        {
            if (!(state is StringState)) return null;

            //this.XCoord + "*" + this.YCoord
            string[] sensations;
            foreach (var sensation in ((IStimuliState) state).Sensations)
                if (sensation.Contains("*") && ((sensations = sensation.Split('*')).Length == 2))
                    return this.Agent.Environment.Cells[int.Parse(sensations[0]), int.Parse(sensations[1])];
            return null;
        }

        protected void DetectInCorridor(string elementID, string sensation)
        {
            var agentX = this.Agent.Cell.XCoord;
            var agentY = this.Agent.Cell.YCoord;
            var env = this.Agent.Environment;

            if (env.DetectInRightCorridor(agentX, agentY, elementID))
                this.CurrentSensations.Add($"{sensation}-r");
            if (env.DetectInLeftCorridor(agentX, agentY, elementID))
                this.CurrentSensations.Add($"{sensation}-l");
            if (env.DetectInUpCorridor(agentX, agentY, elementID))
                this.CurrentSensations.Add($"{sensation}-u");
            if (env.DetectInDownCorridor(agentX, agentY, elementID))
                this.CurrentSensations.Add($"{sensation}-d");
        }
    }
}