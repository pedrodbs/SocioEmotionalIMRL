// ------------------------------------------
// <copyright file="AutoEatEnvironment.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL

//    Last updated: 10/17/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.States;

namespace PS.Learning.IMRL.Domain
{
    [Serializable]
    public class AutoEatEnvironment : IREnvironment
    {
        public AutoEatEnvironment()
        {
            this.AutoEat = true;
        }

        public bool AutoEat { get; set; }

        public override bool AgentFinishedTask(IAgent agent, IState state, IAction action)
        {
            return this.AutoEat
                       ? (state is IStimuliState) && ((IStimuliState) state).Sensations.Contains(this.Hare.IdToken)
                       : base.AgentFinishedTask(agent, state, action);
        }

        protected override void UpdateCellDebugging()
        {
            if (!this.DebugMode) return;

            //stores agent's current cell
            var curCell = this.Agent.Cell;

            //for each cell in the environment
            foreach (var cell in this.Cells)
            {
                //puts the agent in that cell and simulates perception
                this.Agent.Cell = cell;
                this.Agent.PerceptionManager.Update();

                //gets resulting state and respective value and extrinsic pred error
                var state = this.Agent.LongTermMemory.GetUpdatedCurrentState();
                cell.StateValue.IdToken = this.Agent.LongTermMemory.GetMaxStateActionValue(state.ID).ToString("0.00");
            }
            this.Agent.Cell = curCell;
        }
    }
}