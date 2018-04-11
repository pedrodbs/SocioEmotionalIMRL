// ------------------------------------------
// <copyright file="SocialEnvironment.cs" company="Pedro Sequeira">
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
//    Project: Learning.Social

//    Last updated: 10/17/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.States;
using Environment = PS.Learning.Core.Domain.Environments.Environment;

namespace PS.Learning.Social.Domain.Environments
{
    [Serializable]
    public class SocialEnvironment : Core.Domain.Environments.Environment, ISocialEnvironment
    {
        public SocialEnvironment() : base(3, 3)
        {
        }

        #region ISocialEnvironment Members

        public IList<ISocialAgent> Agents { get; protected set; }

        public virtual void SetAgents(IList<ISocialAgent> agents)
        {
            this.Agents = agents;
        }

        public override void Update()
        {
            //resets environment when one of the agents finishes task
            var agentFinishedTask = this.Agents.Any(agent => this.AgentFinishedTask(
                agent, agent.ShortTermMemory.PreviousState, agent.ShortTermMemory.CurrentAction));

            if (agentFinishedTask)
                this.Reset();
        }

        public override double GetAgentReward(IAgent agent, IState state, IAction action)
        {
            return agent.BehaviorManager.LastReward;
        }

        public override bool AgentFinishedTask(IAgent agent, IState state, IAction action)
        {
            return false;
        }

        public override void Init()
        {
        }

        #endregion
    }
}