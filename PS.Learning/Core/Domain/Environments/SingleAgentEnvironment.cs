// ------------------------------------------
// <copyright file="SingleAgentEnvironment.cs" company="Pedro Sequeira">
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
//    Last updated: 10/10/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Managers.Behavior;

namespace PS.Learning.Core.Domain.Environments
{
    [Serializable]
    public abstract class SingleAgentEnvironment : Environment
    {
        protected SingleAgentEnvironment(uint rows, uint cols) : base(rows, cols)
        {
        }

        public ICellAgent Agent { get; set; }

        public override void Update()
        {
            this.UpdateCellDebugging();

            //resets environment when agent finishes task
            var agentFinishedTask = this.AgentFinishedTask(
                this.Agent, this.Agent.ShortTermMemory.PreviousState, this.Agent.ShortTermMemory.CurrentAction);

            if (agentFinishedTask) this.Reset();
        }

        protected virtual void UpdateCellDebugging()
        {
            if (!this.DebugMode) return;

            var oldCell = this.Agent.Cell;
            foreach (var cell in this.Cells)
            {
                this.Agent.Cell = cell;
                this.Agent.PerceptionManager.Update();
                var state = this.Agent.LongTermMemory.GetUpdatedCurrentState();
                cell.StateValue.IdToken = this.Agent.LongTermMemory.GetMaxStateActionValue(state.ID).ToString("0.00");
            }
            this.Agent.Cell = oldCell;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (this.Agent != null) this.Agent.Dispose();
        }

        protected bool ExplorationPhase()
        {
            if (!(this.Agent.BehaviorManager is TwoPhaseBehaviorManager)) return false;

            var behaviorManager = (TwoPhaseBehaviorManager) this.Agent.BehaviorManager;
            return behaviorManager.ExplorationPhase &&
                   (this.Agent.LongTermMemory.TimeStep < behaviorManager.ExplorationSteps);
        }

        public override void Init()
        {
        }
    }
}