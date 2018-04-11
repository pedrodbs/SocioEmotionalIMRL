// ------------------------------------------
// <copyright file="PolicyAdjustLearningManager.cs" company="Pedro Sequeira">
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
//    Last updated: 1/22/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Memories;

namespace PS.Learning.Core.Domain.Managers.Learning
{
    [Serializable]
    public class PolicyAdjustLearningManager : LearningManager
    {
        public PolicyAdjustLearningManager(IAgent agent, ILongTermMemory longTermMemory)
            : base(agent, longTermMemory)
        {
        }

        public override void Update()
        {
            var stm = this.LongTermMemory.ShortTermMemory;
            var ltm = this.Agent.LongTermMemory;
            var oldStateID = stm.PreviousState.ID;
            var actionID = stm.CurrentAction.ID;
            var newStateID = stm.CurrentState.ID;
            var reward = stm.CurrentReward.Value;

            //first update prediction error and then action-value function
            stm.PredictionError = this.GetPredictionError(oldStateID, actionID, newStateID, reward);

            //adjust q-value/policy from given action
            this.UpdateStateActionValue(oldStateID, actionID, newStateID, reward);

            if ((oldStateID == uint.MaxValue) || (actionID == uint.MaxValue))
                return;

            //gets state policy based on (updated) q-values
            var policy = this.Agent.BehaviorManager.GetPolicy(oldStateID);

            //updates state-action values directly based on (normalized) policy
            foreach (var action in this.Agent.Actions.Values)
                ltm.UpdateStateActionValue(oldStateID, action.ID, policy[action.ID]);
        }

        protected override double GetUpdatedStateActionValue(
            uint oldStateID, uint actionID, uint newStateID, double reward)
        {
            //checks args
            if ((oldStateID == uint.MaxValue) || (actionID == uint.MaxValue) || (newStateID == uint.MaxValue))
                return 0;

            //adjust value according to new reward
            var stateActionValue = this.LongTermMemory.GetStateActionValue(oldStateID, actionID);
            var adjustment = this.GetPredictionError(oldStateID, actionID, newStateID, reward);

            return stateActionValue + adjustment;
        }

        public virtual double GetPredictionError(uint oldStateID, uint actionID, uint newStateID, double reward)
        {
            //checks args
            if ((oldStateID == uint.MaxValue) || (actionID == uint.MaxValue) || (newStateID == uint.MaxValue))
                return 0;

            //updates prediction error -> adjustment
            return (this.LearningRate.Value*reward);
        }
    }
}