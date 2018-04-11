// ------------------------------------------
// <copyright file="PersistenceEnvironment.cs" company="Pedro Sequeira">
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
using PS.Learning.IMRL.Emotions.Testing;

namespace PS.Learning.Tests.EmotionalOptimization.Domain.Environments
{
    [Serializable]
    public class PersistenceEnvironment : FixedStartRabbitHareEnvironment
    {
        protected uint currentNumMoveActions;
        protected uint numMoveActionsRequired;
        protected double passFenceReward;
        protected Cell[] possibleFenceCells;
        protected Cell previousAgentCell;

        public PersistenceEnvironment()
        {
            //creates obstacle
            this.Fence = new CellElement
                         {
                             IdToken = "fence",
                             Reward = 0f,
                             Visible = true,
                             HasSmell = false,
                             Walkable = true,
                             ImagePath = "../../../../bin/resources/fence.png",
                         };
            this.AutoEat = true;
        }

        protected new IEmotionalTestsConfig TestsConfig
        {
            get { return base.TestsConfig as IEmotionalTestsConfig; }
        }

        protected double FenceReward
        {
            get
            {
                return 0;
                //return -this.rand.Next(10001)/10000f;
                //return -0.03f*(1f-((double)this.Agent.LongTermMemory.TimeStep / this.maxSteps));
            }
        }

        public CellElement Fence { get; protected set; }

        public override void Init()
        {
            base.Init();
            this.numMoveActionsRequired = 0;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.Fence.Dispose();
        }

        public override void Reset()
        {
            base.Reset();

            //places fence in one of possible cells (diff from previous)
            this.Fence.Cell = this.GetRandomCell(null, this.possibleFenceCells);
        }

        public override void Update()
        {
            //hare is automatically accessible during the exploration phase or 
            //if the agent is in the upper part of the environment (past the fence)
            this.Fence.Walkable =
                this.ExplorationPhase() || 
                (this.Agent.Cell.YCoord < this.Fence.Cell.YCoord) ||
                (this.numMoveActionsRequired == 0);

            this.passFenceReward = 0;

            //test if agent is trying to move past the electric fence
            if (this.AgentTriedToPassFence())
            {
                this.passFenceReward = this.FenceReward; //this.Fence.Reward;
                if (++this.currentNumMoveActions >= this.numMoveActionsRequired)
                {
                    //if number of tries is sufficient, hare becomes accessible
                    this.Fence.Walkable = true;
                }
            }
            else
            {
                //resets current move-up counter
                this.currentNumMoveActions = 0;
            }

            this.previousAgentCell = this.Agent.Cell;
            base.Update();

            //increments number of move actions needed to pass the fence next time
            var stm = this.Agent.ShortTermMemory;
            var agentAteHare = this.AgentFinishedTask(this.Agent, stm.PreviousState, stm.CurrentAction) &&
                               ((IStimuliState) stm.PreviousState).Sensations.Contains(this.Hare.IdToken);

            if (agentAteHare && (this.numMoveActionsRequired < this.TestsConfig.MaxMoveActionsRequired))
                this.numMoveActionsRequired++;
        }

        public override double GetAgentReward(IAgent agent, IState state, IAction action)
        {
            return base.GetAgentReward(agent, state, action) + this.passFenceReward;
        }

        public override void CreateCells(uint rows, uint cols, string configFile)
        {
            base.CreateCells(rows, cols, configFile);

            //creates array of possible fence cell locations
            this.possibleFenceCells = new[] {this.Cells[1, 2]}; //, this.Cells[1, 1], this.Cells[2, 1]};
        }

        protected virtual bool AgentTriedToPassFence()
        {
            if (this.previousAgentCell == null) return false;

            var action = this.Agent.ShortTermMemory.CurrentAction;
            var neighbourAgentCells = this.previousAgentCell.NeighbourCells;

            //checks previous agent action (must be a move action) and agent must be next to the fence
            return (action != null) && (this.previousAgentCell.YCoord <= 3) &&
                   ((this.Fence.Cell.Equals(neighbourAgentCells[Cell.UP_DIR_STR]) && (action is MoveUp)) ||
                    (this.Fence.Cell.Equals(neighbourAgentCells[Cell.RIGHT_DIR_STR]) && (action is MoveRight)));
        }
    }
}