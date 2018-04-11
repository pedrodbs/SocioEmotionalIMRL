// ------------------------------------------
// <copyright file="IRMotivationManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL

//    Last updated: 7/11/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Utilities.Math;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.Core.Domain;
using PS.Learning.Core.Domain.Agents;

namespace PS.Learning.IMRL.Domain.Managers.Motivation
{
    public class IRMotivationManager : ArrayParamMotivationManager, IHungerMotivationManager
    {
        protected const string HUNGER_ID = "hunger";
        private const long NORMALIZATION_FACTOR = -100000000;

        public IRMotivationManager(CellAgent agent) : base(agent)
        {
            this.InverseRecencyReward = new StatisticalQuantity();

            //agent starts hungry
            this.Hunger = new Need(HUNGER_ID, 1, 0, 0, 0) {Value = 1};
        }

        public StatisticalQuantity InverseRecencyReward { get; protected set; }

        #region IHungerMotivationManager Members

        public Need Hunger { get; protected set; }

        public override void Reset()
        {
            base.Reset();

            //agent starts hungry
            this.Hunger.Value = 1;
        }

        public override void Update()
        {
            //updates the inverse recency reward feature
            this.InverseRecencyReward.Value = this.GetInverseRecencyValue(
                this.Agent.ShortTermMemory.PreviousState.ID, this.Agent.ShortTermMemory.CurrentAction.ID);

            base.Update();
        }

        public override double GetExtrinsicReward(uint stateID, uint actionID)
        {
            var state = this.Agent.LongTermMemory.GetState(stateID);
            var action = this.Agent.BehaviorManager.ActionList[(int) actionID];
            var agentFinishedTask = this.Agent.Environment.AgentFinishedTask(this.Agent, state, action);
            this.Hunger.Value = (uint) (agentFinishedTask ? 0 : 1);
            return base.GetExtrinsicReward(stateID, actionID);
        }

        #endregion

        public override DenseVector GetRewardFeatures(uint prevState, uint action, uint nextState)
        {
            return new DenseVector(new[]
                                       {
                                           this.GetInverseRecencyValue(prevState, action),
                                           this.GetExtrinsicReward(prevState, action)
                                       });
        }

        protected double GetInverseRecencyValue(uint state, uint action)
        {
            //checks args
            if ((state.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue))) return 0;

            //inverse recency: the number of timesteps since the agent previously executed action a in state s within current history
            var timestepsActionState = this.Agent.LongTermMemory.GetTimeStepsLastStateAction(state, action);
            var stateCount = this.Agent.LongTermMemory.GetStateCount(this.Agent.ShortTermMemory.CurrentState.ID);
            var noveltyValue = (stateCount*stateCount)/NORMALIZATION_FACTOR;

            //returns reward
            return noveltyValue; // timestepsActionState == 0 ? 0 : (1f - (1f / timestepsActionState));
        }
    }
}