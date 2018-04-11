// ------------------------------------------
// <copyright file="IREnvironment.cs" company="Pedro Sequeira">
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

//    Last updated: 2/6/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.Environments;
using PS.Learning.Core.Domain.States;
using PS.Learning.IMRL.Domain.Agents;

namespace PS.Learning.IMRL.Domain
{
    [Serializable]
    public class IREnvironment : SingleAgentEnvironment
    {
        protected const int WORLD_SIZE = 3;

        protected Cell[] possiblePreyCells;

        public IREnvironment() : base(3, 3)
        {
            //creates food prey hare
            this.Hare = new CellElement
                            {
                                IdToken = "food",
                                Description = "food",
                                Reward = 1f,
                                Visible = true,
                                Walkable = true,
                                HasSmell = true,
                                ImagePath = "../../../../bin/resources/hare.png",
                            };
        }

        public CellElement Hare { get; protected set; }

        public new IRAgent Agent
        {
            get { return base.Agent as IRAgent; }
            set { base.Agent = value; }
        }

        public override void Reset()
        {
            //agent starts from top-left position, only in the beggining
            if ((this.Agent != null) && (this.Agent.Cell == null))
            {
                this.Agent.Environment = this;
                this.Agent.Cell = this.Cells[1, 1];
            }

            //places prey in random right-cell
            this.Hare.Cell = this.GetRandomCell(new[] {this.Hare.Cell}, this.possiblePreyCells);
        }

        public override double GetAgentReward(IAgent agent, IState state, IAction action)
        {
            return this.AgentFinishedTask(agent, state, action) ? this.Hare.Reward : 0;
        }

        public override bool AgentFinishedTask(IAgent agent, IState state, IAction action)
        {
            //task ends when agent eats the prey
            return (action != null) && (action is Eat) && (state != null) &&
                   ((IStimuliState) state).Sensations.Contains(this.Hare.IdToken);
        }

        public override void CreateCells(uint rows, uint cols, string configFile)
        {
            base.CreateCells(rows, cols, configFile);

            //creates array of possible prey cell locations
            this.possiblePreyCells = new[] {this.Cells[3, 1], this.Cells[3, 3], this.Cells[3, 5]};
        }

        public override void Init()
        {
        }
    }
}