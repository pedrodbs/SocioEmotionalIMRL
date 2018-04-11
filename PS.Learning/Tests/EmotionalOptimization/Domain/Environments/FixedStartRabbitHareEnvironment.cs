// ------------------------------------------
// <copyright file="FixedStartRabbitHareEnvironment.cs" company="Pedro Sequeira">
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
    public class FixedStartRabbitHareEnvironment : AutoEatEnvironment
    {
        public FixedStartRabbitHareEnvironment()
        {
            //creates food prey rabbit
            this.Rabbit = new CellElement
                              {
                                  IdToken = "rabbit",
                                  Description = "food",
                                  Reward = 0.01f,
                                  Visible = true,
                                  Walkable = true,
                                  HasSmell = true,
                                  ImagePath = "../../../../bin/resources/rabbit.png",
                              };
        }

        public CellElement Rabbit { get; protected set; }

        public override void Reset()
        {
            if (this.Agent == null) return;

            //agent always starts from 2nd row right position;
            this.Agent.Cell = this.Cells[3, 3];

            if (this.Agent.Environment == null)
                this.Agent.Environment = this;
        }

        public override void CreateCells(uint rows, uint cols, string configFile)
        {
            base.CreateCells(rows, cols, configFile);

            //sets food locations
            this.Hare.Cell = this.Cells[3, 1];
            this.Rabbit.Cell = this.Cells[3, 5];
        }

        public override double GetAgentReward(IAgent agent, IState state, IAction action)
        {
            return this.AgentFinishedTask(agent, state, action)
                       ? (((IStimuliState)state).Sensations.Contains(this.Hare.IdToken)
                              ? this.Hare.Reward
                              : this.Rabbit.Reward)
                       : 0;
        }

        public override bool AgentFinishedTask(IAgent agent, IState state, IAction action)
        {
            if ((state == null) || !(state is IStimuliState)) return false;
            var sensations = ((IStimuliState)state).Sensations;
            return (this.AutoEat || ((action != null) && (action is Eat))) &&
                   (sensations.Contains(this.Hare.IdToken) || sensations.Contains(this.Rabbit.IdToken));
        }
    }
}