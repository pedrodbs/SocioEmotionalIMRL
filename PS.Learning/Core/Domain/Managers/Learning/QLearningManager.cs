// ------------------------------------------
// <copyright file="QLearningManager.cs" company="Pedro Sequeira">
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
using PS.Learning.Core.Domain.Memories;

namespace PS.Learning.Core.Domain.Managers.Learning
{
    [Serializable]
    public class QLearningManager : LearningManager
    {
        public QLearningManager(IAgent agent, ILongTermMemory longTermMemory)
            : base(agent, longTermMemory)
        {
        }

        public override void Update()
        {
            var stm = this.LongTermMemory.ShortTermMemory;
            var previousStateID = stm.PreviousState.ID;
            var currentActionID = stm.CurrentAction.ID;
            var currentStateID = stm.CurrentState.ID;
            var reward = stm.CurrentReward.Value;

            //first update prediction error and then action-value function
            stm.PredictionError = this.GetPredictionError(previousStateID, currentActionID, currentStateID, reward);

            this.UpdateStateActionValue(previousStateID, currentActionID, currentStateID, reward);
        }

        protected override double GetUpdatedStateActionValue(
            uint oldStateID, uint actionID, uint newStateID, double reward)
        {
            //checks args
            if ((oldStateID == uint.MaxValue) || (actionID == uint.MaxValue) || (newStateID == uint.MaxValue))
                return 0;

            //returns new value according to Q-Learning algorithm
            var stateActionValue = this.LongTermMemory.GetStateActionValue(oldStateID, actionID);
            var predictionError = this.GetPredictionError(oldStateID, actionID, newStateID, reward);
            return stateActionValue + (this.LearningRate.Value*predictionError);
        }

        public virtual double GetPredictionError(uint oldStateID, uint actionID, uint newStateID, double reward)
        {
            //checks args
            if ((oldStateID == uint.MaxValue) || (actionID == uint.MaxValue) || (newStateID == uint.MaxValue))
                return 0;

            //gets max future value
            var maxFutureValue = this.Discount.Value.Equals(0)
                                     ? 0
                                     : this.Discount.Value * this.LongTermMemory.GetMaxStateActionValue(newStateID);

            //updates prediction error
            var stateActionValue = this.LongTermMemory.GetStateActionValue(oldStateID, actionID);
            return reward + maxFutureValue - stateActionValue;
        }
    }
}