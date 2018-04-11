// ------------------------------------------
// <copyright file="PreySeasonEnvironment.cs" company="Pedro Sequeira">
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
using PS.Learning.Core.Domain.States;
using PS.Learning.IMRL.Emotions.Testing;

namespace PS.Learning.Tests.EmotionalOptimization.Domain.Environments
{
    [Serializable]
    public class PreySeasonEnvironment : FixedStartRabbitHareEnvironment
    {
        public const double RABBIT_PUNISHMENT = -1f;

        protected uint maxRabbits;
        protected int numEatenRabbits;
        protected bool rabbitSeason;
        protected bool randFirstSeason;

        public PreySeasonEnvironment()
        {
            this.Rabbit.Reward = 0.1f;
            this.Hare.Reward = 1f;
            this.maxRabbits = 9;

            this.AutoEat = true;
        }

        protected new IEmotionalTestsConfig TestsConfig
        {
            get { return base.TestsConfig as IEmotionalTestsConfig; }
        }

        public override void Init()
        {
            //chooses season randomly and forces changes in season
            this.rabbitSeason = this.TestsConfig.RandStart && this.rand.Next(2) == 0;
        }

        public override void Update()
        {
            base.Update();

            //tests if season has changed
            if ((this.Agent == null) || (this.Agent.ShortTermMemory.PreviousState == null) ||
                !(this.Agent.LongTermMemory.TimeStep%(double) this.TestsConfig.NumStepsPerSeason).Equals(0f))
                return;

            //changes season
            this.ChangeSeason();
        }

        public override double GetAgentReward(IAgent agent, IState state, IAction action)
        {
            //checks for "normal" reward
            if (!this.AgentFinishedTask(agent, state, action))
                return 0;

            //in hare season just return hare reward
            if (!this.rabbitSeason)
                return this.Hare.Reward;

            //if in rabbit season, check for max eaten rabbits, in which case the agent is penalized 
            if (++this.numEatenRabbits > this.maxRabbits)
                return RABBIT_PUNISHMENT;

            return this.Rabbit.Reward;
        }

        protected void ChangeSeason()
        {
            if (this.rabbitSeason)
                this.InitHareSeason();
            else
                this.InitRabbitSeason();

            this.rabbitSeason = !this.rabbitSeason;
        }

        protected void InitHareSeason()
        {
            //puts hare in cell, takes rabbit away and resets eaten rabbits counter
            this.Hare.Cell = this.Cells[3, 5];
            this.Rabbit.Cell = null;
            this.numEatenRabbits = 0;
        }

        protected void InitRabbitSeason()
        {
            //puts rabbit in cell, takes hare away
            this.Hare.Cell = null;
            this.Rabbit.Cell = this.Cells[3, 1];
        }
    }
}