// ------------------------------------------
// <copyright file="LairsEnvironment.cs" company="Pedro Sequeira">
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
using PS.Learning.Core.Domain.Environments;
using PS.Learning.Core.Domain.States;
using PS.Learning.IMRL.Domain;
using PS.Learning.Tests.EmotionalOptimization.Domain.Actions;

namespace PS.Learning.Tests.EmotionalOptimization.Domain.Environments
{
    [Serializable]
    public class LairsEnvironment : SingleAgentEnvironment
    {
        protected const int CLOSE_PROB = 1;
        protected const double RABBIT_REWARD = 1f;
        protected const string RABBIT_IMG_PATH = "../../../../bin/resources/rabbit.png";
        protected const string CLOSED_LAIR_IMG_PATH = "../../../../bin/resources/closed-lair.png";
        protected const string OPENED_LAIR_IMG_PATH = "../../../../bin/resources/opened-lair.png";
        protected Cell[] possibleLairCells;

        public LairsEnvironment() : base(0, 0)
        {
        }

        public RabbitLair Lair1 { get; protected set; }

        public RabbitLair Lair2 { get; protected set; }

        public override void Init()
        {
            base.Init();

            //creates lair elements
            this.Lair1 = new RabbitLair
                             {
                                 IdToken = "lair1",
                                 Description = "lair",
                                 Reward = RABBIT_REWARD,
                                 Visible = true,
                                 Walkable = true,
                                 HasSmell = true,
                                 ImagePath = RABBIT_IMG_PATH,
                                 ClosedLairImagePath = CLOSED_LAIR_IMG_PATH,
                                 OpenedLairImagePath = OPENED_LAIR_IMG_PATH,
                             };

            this.Lair2 = new RabbitLair
                             {
                                 IdToken = "lair2",
                                 Description = "lair",
                                 Reward = RABBIT_REWARD,
                                 Visible = true,
                                 Walkable = true,
                                 HasSmell = true,
                                 ImagePath = RABBIT_IMG_PATH,
                                 ClosedLairImagePath = CLOSED_LAIR_IMG_PATH,
                                 OpenedLairImagePath = OPENED_LAIR_IMG_PATH,
                             };
        }

        public override void Update()
        {
            base.Update();

            var action = this.Agent.ShortTermMemory.CurrentAction;
            this.UpdateLair(action, this.Lair1);
            this.UpdateLair(action, this.Lair2);
        }

        protected void UpdateLair(IAction action, RabbitLair rabbitLair)
        {
            if ((rabbitLair.State == LairState.Empty) && (this.rand.Next(10) < CLOSE_PROB))
            {
                //closes the lair if open with certain probability
                rabbitLair.State = LairState.Closed;
            }
            else if ((rabbitLair.State == LairState.Closed) && (action != null) && (action is OpenLair)
                     && (this.Agent.Cell == rabbitLair.Cell))
            {
                //shows prey if the lair if closed, agent is colocated and performed action "open"
                rabbitLair.State = LairState.Rabbit;
            }
            else if (rabbitLair.State == LairState.Rabbit)
            {
                //the prey flees after one time step
                rabbitLair.State = LairState.Empty;
            }
            else
            {
                rabbitLair.ForceRepaint = false;
                return;
            }

            rabbitLair.ForceRepaint = true;
        }

        public override double GetAgentReward(IAgent agent, IState state, IAction action)
        {
            return this.AgentFinishedTask(agent, state, action) ? RABBIT_REWARD : 0;
        }

        public override bool AgentFinishedTask(IAgent agent, IState state, IAction action)
        {
            //task ends when the agent eats a prey
            return (action != null) && (action is Eat) &&
                   (state != null) && (state is IStimuliState) &&
                   ((IStimuliState) state).Sensations.Contains(LairState.Rabbit.ToString());
        }

        public override void Reset()
        {
            //agent starts from top-left position, only in the beggining
            if ((this.Agent != null) && (this.Agent.Cell == null))
            {
                this.Agent.Environment = this;

                if (this.TestsConfig.RandStart)
                {
                    //places prey, agent and water in random possible cells
                    this.Agent.Cell = this.Cells[5, 3];
                    this.Lair1.Cell = this.GetRandomCell(new[] {this.Agent.Cell}, this.possibleLairCells);
                    this.Lair2.Cell = this.GetRandomCell(new[] {this.Lair1.Cell, this.Agent.Cell},
                                                         this.possibleLairCells);
                }
                else
                {
                    this.Agent.Cell = this.Cells[5, 3];
                    this.Lair1.Cell = this.Cells[1, 1];
                    this.Lair2.Cell = this.Cells[5, 5];
                }
            }
        }

        public override void CreateCells(uint rows, uint cols, string configFile)
        {
            base.CreateCells(rows, cols, configFile);

            //creates array of possible prey/water cell locations (corners)
            this.possibleLairCells =
                new[] {this.Cells[1, 1], this.Cells[1, 5], this.Cells[5, 1], this.Cells[5, 5]};
        }
    }
}