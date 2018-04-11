// ------------------------------------------
// <copyright file="ForagingGPMotivationManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL.EC

//    Last updated: 03/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.Core.Domain;
using PS.Learning.IMRL.Domain.Managers.Motivation;

namespace PS.Learning.IMRL.EC.Domain
{
    public class ForagingGPMotivationManager : GPMotivationManager, IHungerMotivationManager
    {
        protected const int EXTRINSIC_RWD_IDX = 0;
        protected const int ST_ACT_CNT_IDX = 1;
        protected const int ST_CNT_IDX = 2;
        protected const int DIST_IDX = 3;
        protected const int ST_VAL_IDX = 4;
        protected const int ST_ACT_VAL_IDX = 5;
        protected const int ST_ACT_PRED_ERR_IDX = 6;
        protected const int TRANS_PROB_IDX = 7;
        public static readonly string[] VariableNames = {"Re", "Csa", "Cs", "D", "Vs", "Qsa", "Esa", "Pssa"};

        public ForagingGPMotivationManager(GPForagingAgent agent, double[] constants) : base(agent, constants)
        {
            //agent starts hungry
            this.Hunger = new Need("Hunger", 1, 0, 0, 0) {Value = 1};
        }

        protected override string[] VariablesNames
        {
            get { return VariableNames; }
        }

        public new GPForagingAgent Agent
        {
            get { return base.Agent as GPForagingAgent; }
        }

        #region IHungerMotivationManager Members

        public Need Hunger { get; protected set; }

        public override void Reset()
        {
            base.Reset();

            //agent starts hungry
            this.Hunger.Value = 1;
        }

        public override double GetExtrinsicReward(uint stateID, uint actionID)
        {
            var state = this.Agent.LongTermMemory.GetState(stateID);
            var action = this.Agent.BehaviorManager.ActionList[(int) actionID];
            var agentFinishedTask = this.Agent.Environment.AgentFinishedTask(this.Agent, state, action);

            //updates hunger status
            this.Hunger.Value = (uint) (agentFinishedTask ? 0 : 1);

            return base.GetExtrinsicReward(stateID, actionID);
        }

        #endregion

        public override DenseVector GetRewardFeatures(uint prevState, uint action, uint nextState)
        {
            var featureVector = new DenseVector((int) this.NumInputElements);
            this.CopyConstants(featureVector);

            //updates extrinsic reward variable
            featureVector[EXTRINSIC_RWD_IDX] = this.GetExtrinsicReward(prevState, action);

            //updates state-action count diff variable
            featureVector[ST_ACT_CNT_IDX] = this.GetStateActionCount(prevState, action);
            // this.GetIRStateActionTimeSteps();

            //updates state count diff variable
            featureVector[ST_CNT_IDX] = this.GetStateCount(prevState); //this.GetIRStateTimeSteps();

            //updates distance to position/state when last achieved best reward
            featureVector[DIST_IDX] = this.Agent.StateRelevanceManager.GetDistanceToGoal(prevState);

            //updates extrinsic state and state-action value
            featureVector[ST_VAL_IDX] = this.GetStateValue(prevState);
            featureVector[ST_ACT_VAL_IDX] = this.GetStateActionValue(prevState, action);

            //updates extrinsic state-action value prediction error average 
            featureVector[ST_ACT_PRED_ERR_IDX] = this.GetStateActionPredError(prevState, action);

            //updates extrinsic state-action-state transition probability
            featureVector[TRANS_PROB_IDX] = this.GetStateTransitionProb(prevState, action, nextState);

            //resets hunger state
            if (this.Hunger.Value == 0) this.Hunger.Value = 1;

            return featureVector;
        }

        protected double GetStateTransitionProb(uint prevState, uint action, uint nextState)
        {
            //checks args
            if ((prevState.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue)) || (nextState.Equals(uint.MaxValue)))
                return 0;

            //return transition prob
            return this.Agent.ExtrinsicLTM.GetStateActionTransitionProbability(
                prevState, action, nextState);
        }

        protected double GetStateActionPredError(uint prevState, uint action)
        {
            //checks args
            if ((prevState.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue))) return 0;

            //return pred error avg
            return this.Agent.ExtrinsicLTM.GetStateActionPredErrAvg(prevState, action);
        }

        protected double GetIRStateActionTimeSteps(uint prevState, uint action)
        {
            //checks args
            if ((prevState.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue))) return 0;

            //inverse recency: the number of timesteps since the agent previously executed action a in state s within current history
            var timestepsActionState = this.Agent.LongTermMemory.GetTimeStepsLastStateAction(prevState, action);

            //returns reward
            return timestepsActionState == 0 ? 0 : (1d - (1d/timestepsActionState));
        }

        protected double GetIRStateTimeSteps(uint prevState)
        {
            //checks args
            if (prevState.Equals(uint.MaxValue)) return 0;

            //inverse recency: the number of timesteps since the agent previously visited the state s within current history
            var timestepsState = this.Agent.LongTermMemory.GetTimeStepsLastState(prevState);

            //returns reward
            return timestepsState == 0 ? 0 : (1d - (1d/timestepsState));
        }

        protected double GetStateActionCount(uint prevState, uint action)
        {
            //checks args
            if ((prevState.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue))) return 0;

            //gets the number of times the agent previously executed action a in state s within current history
            var stateActionCount = this.Agent.ExtrinsicLTM.GetStateActionCount(prevState, action);

            //returns reward
            return stateActionCount;
        }

        protected double GetStateCount(uint prevState)
        {
            //checks args
            if (prevState.Equals(uint.MaxValue)) return 0;

            //gets the number of times the agent previously visited the state s within current history
            var stateCount = this.Agent.LongTermMemory.GetStateCount(prevState);

            //returns reward
            return stateCount;
        }

        protected double GetStateValue(uint prevState)
        {
            //checks args
            if (prevState.Equals(uint.MaxValue)) return 0;

            //gets the value of state s within current history
            var stateValue = this.Agent.ExtrinsicLTM.GetMaxStateActionValue(prevState);

            //returns reward
            return stateValue;
        }

        protected double GetStateActionValue(uint prevState, uint action)
        {
            //checks args
            if ((prevState.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue))) return 0;

            //gets the value of state s within current history
            var stateActionValue = this.Agent.ExtrinsicLTM.GetStateActionValue(prevState, action);

            //returns reward
            return stateActionValue;
        }
    }
}