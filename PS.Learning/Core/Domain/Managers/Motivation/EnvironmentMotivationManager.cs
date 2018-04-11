// ------------------------------------------
// <copyright file="EnvironmentMotivationManager.cs" company="Pedro Sequeira">
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

namespace PS.Learning.Core.Domain.Managers.Motivation
{
    [Serializable]
    public class EnvironmentMotivationManager : MotivationManager
    {
        public EnvironmentMotivationManager(ISituatedAgent agent)
            : base(agent)
        {
        }

        public new ISituatedAgent Agent
        {
            get { return base.Agent as ICellAgent; }
        }

        public override double GetExtrinsicReward(uint stateID, uint actionID)
        {
            var state = this.Agent.LongTermMemory.GetState(stateID);
            var action = this.Agent.BehaviorManager.ActionList[(int) actionID];
            return this.Agent.Environment.GetAgentReward(this.Agent, state, action);
        }
    }
}