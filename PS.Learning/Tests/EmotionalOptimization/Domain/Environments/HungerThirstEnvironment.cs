// ------------------------------------------
// <copyright file="HungerThirstEnvironment.cs" company="Pedro Sequeira">
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

//    Last updated: 10/10/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.States;
using PS.Learning.IMRL.Domain;

namespace PS.Learning.Tests.EmotionalOptimization.Domain.Environments
{
    [Serializable]
    public class HungerThirstEnvironment : AutoEatEnvironment
    {
        private const int THIRST_PROB = 2;

        public HungerThirstEnvironment()
        {
            this.AutoEat = true;
        }

        public CellElement Water { get; protected set; }

        public bool IsThirsty { get; protected set; }

        public override void Init()
        {
            base.Init();

            //creates water element
            this.Water = new CellElement
                             {
                                 IdToken = "water",
                                 Description = "water",
                                 Reward = 1f,
                                 Visible = true,
                                 Walkable = true,
                                 HasSmell = true,
                                 ImagePath = "../../../../bin/resources/water.png",
                             };
        }

        public override void Update()
        {
            base.Update();

            var state = this.Agent.ShortTermMemory.PreviousState;
            if ((state is IStimuliState) && ((IStimuliState) state).Sensations.Contains(this.Water.IdToken))
                this.IsThirsty = false;
            else
                this.IsThirsty = this.IsThirsty || this.rand.Next(10) < THIRST_PROB;
        }

        public override bool AgentFinishedTask(IAgent agent, IState state, IAction action)
        {
            return !this.IsThirsty && base.AgentFinishedTask(agent, state, action);
        }

        public override void Reset()
        {
            //agent starts from top-left position, only in the beggining
            if ((this.Agent == null) || (this.Agent.Cell != null)) return;

            this.Agent.Environment = this;

            if (this.TestsConfig.RandStart)
            {
                //places prey, agent and water in random possible cells
                this.Agent.Cell = this.Cells[5, 3];
                this.Hare.Cell = this.GetRandomCell(new[] {this.Agent.Cell}, this.possiblePreyCells);
                this.Water.Cell = this.GetRandomCell(new[] {this.Hare.Cell, this.Agent.Cell}, this.possiblePreyCells);
            }
            else
            {
                this.Agent.Cell = this.Cells[5, 3]; //middle position
                this.Hare.Cell = this.Cells[5, 5];
                this.Water.Cell = this.Cells[1, 1];
            }
        }

        public override void CreateCells(uint rows, uint cols, string configFile)
        {
            base.CreateCells(rows, cols, configFile);

            //creates array of possible prey/water cell locations (corners)
            this.possiblePreyCells =
                new[] {this.Cells[1, 1], this.Cells[1, 5], this.Cells[5, 1], this.Cells[5, 5]};
        }
    }
}