// ------------------------------------------
// <copyright file="EmotionalHungryMotivationManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.Emotions

//    Last updated: 10/10/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.IMRL.Domain.Managers.Motivation;
using PS.Learning.IMRL.Emotions.Domain.Agents;

namespace PS.Learning.IMRL.Emotions.Domain.Managers
{
    [Serializable]
    public class EmotionalHungryMotivationManager : EmotionalMotivationManager, IHungerMotivationManager
    {
        public const string HUNGER_ID = "hunger";

        public EmotionalHungryMotivationManager(EmotionalAgent agent) : base(agent)
        {
            //agent starts hungry
            this.Hunger = new Need(HUNGER_ID, 1, 0, 0, 0) {Value = 1};
        }

        public new CellAgent Agent
        {
            get { return base.Agent as CellAgent; }
        }

        #region IHungerMotivationManager Members

        public Need Hunger { get; protected set; }

        public override double GetExtrinsicReward(uint stateID, uint actionID)
        {
            var state = this.Agent.LongTermMemory.GetState(stateID);
            var action = this.Agent.BehaviorManager.ActionList[(int) actionID];
            var agentFinishedTask = this.Agent.Environment.AgentFinishedTask(this.Agent, state, action);

            this.Hunger.Value = (uint) (agentFinishedTask ? 0 : 1);

            return base.GetExtrinsicReward(stateID, actionID);
        }

        #endregion
    }
}