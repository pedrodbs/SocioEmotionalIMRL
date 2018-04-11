// ------------------------------------------
// <copyright file="CertaintyEquivalentLearningManager.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.Core

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Memories;
using PS.Utilities.Collections;
using System;
using System.Linq;

namespace PS.Learning.Core.Domain.Managers.Learning
{
    [Serializable]
    public class CertaintyEquivalentLearningManager : LearningManager
    {
        #region Private Fields

        private const int MAX_ITERATIONS = 20;
        private double _oldActionValue;
        private double[][] _previousStateActionValue;

        #endregion Private Fields

        #region Public Constructors

        public CertaintyEquivalentLearningManager(IAgent agent, LongTermMemory longTermMemory)
            : base(agent, longTermMemory)
        {
            this.MaximalChangeThreshold = this.Scenario.TestsConfig.MaximalChangeThreshold;
        }

        #endregion Public Constructors

        #region Public Properties

        public double MaximalChangeThreshold { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void Dispose()
        {
            base.Dispose();
            this._previousStateActionValue = null;
        }

        public override void Reset()
        {
            base.Reset();

            var ltm = this.LongTermMemory;
            this._previousStateActionValue = ArrayUtil.Create2DArray<double>(ltm.MaxStates, ltm.NumActions);
        }

        public override void Update()
        {
            //stores old action-value
            this.StoreOldActionValue();

            //update action-values (for all state-action)
            var shortTermMemory = this.LongTermMemory.ShortTermMemory;
            this.UpdateStateActionValue(shortTermMemory.PreviousState.ID,
                shortTermMemory.CurrentAction.ID,
                shortTermMemory.CurrentState.ID,
                shortTermMemory.CurrentReward.Value);

            //update prediction error for current state and action
            this.UpdatePredictionError();
        }

        public void UpdatePredictionError()
        {
            //checks variables
            var oldState = this.LongTermMemory.ShortTermMemory.PreviousState;
            var action = this.LongTermMemory.ShortTermMemory.CurrentAction;

            if ((oldState == null) || (action == null)) return;

            //updates prediction error (not used by the learning algorithm, but by emotions manager)
            this.LongTermMemory.ShortTermMemory.PredictionError =
                this.LongTermMemory.GetStateActionValue(oldState.ID, action.ID) - this._oldActionValue;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual double GetMaxStateActionValue(uint stateID)
        {
            //checks state arg and returns max action value for given state
            return stateID >= this.LongTermMemory.NumStates ? 0 : this._previousStateActionValue[stateID].Max();
        }

        protected override double GetUpdatedStateActionValue(
            uint oldStateID, uint actionID, uint newStateID, double reward)
        {
            var longTermMemory = this.LongTermMemory;

            //checks args
            if ((oldStateID == uint.MaxValue) || (actionID == uint.MaxValue))
                return 0;

            //gets weighted sum of action-values of all possible future state transitions
            var futureTransitionValue = 0d;
            for (var transitionStateID = 0u; transitionStateID < longTermMemory.NumStates; transitionStateID++)
            {
                futureTransitionValue +=
                    longTermMemory.GetStateActionTransitionProbability(oldStateID, actionID, transitionStateID) *
                    this.GetMaxStateActionValue(transitionStateID);
            }

            //returns new value according to Model-based-learning algorithm
            return reward + (this.Discount.Value * futureTransitionValue);
        }

        protected void StoreOldActionValue()
        {
            //gets args
            var oldState = this.LongTermMemory.ShortTermMemory.PreviousState;
            var action = this.LongTermMemory.ShortTermMemory.CurrentAction;

            //stores action-value
            this._oldActionValue = this.LongTermMemory.GetStateActionValue(oldState.ID, action.ID);
        }

        protected override void UpdateStateActionValue(
            uint oldStateID, uint curActionID, uint newStateID, double curReward)
        {
            var maximalChange = double.MaxValue;
            var numIterations = 0;

            //resets all Q-values (sets to zero)
            this.LongTermMemory.ResetAllStateActionValues();

            //updates Q-values while change in values is large
            while (maximalChange > this.MaximalChangeThreshold)
            {
                if (numIterations++ > MAX_ITERATIONS)
                    break;

                //copies previous state-action values to memory buffer
                for (var stateID = 0u; stateID < this.LongTermMemory.NumStates; stateID++)
                    for (var actionID = 0u; actionID < this.LongTermMemory.NumActions; actionID++)
                        this._previousStateActionValue[stateID][actionID] =
                            this.LongTermMemory.GetStateActionValue(stateID, actionID);

                maximalChange = 0;

                //for all state-action pairs, update Q-value
                for (var stateID = 0u; stateID < this.LongTermMemory.NumStates; stateID++)
                    for (var actionID = 0u; actionID < this.LongTermMemory.NumActions; actionID++)
                    {
                        //stores old Q-value
                        var previousActionValue = this._previousStateActionValue[stateID][actionID];

                        //gets state-action reward (estimated)
                        var reward = this.LongTermMemory.GetStateActionReward(stateID, actionID);

                        //updates new Q-value for current state-action pair
                        var updatedActionValue = this.GetUpdatedStateActionValue(stateID, actionID, uint.MaxValue,
                            reward);
                        this.LongTermMemory.UpdateStateActionValue(stateID, actionID, updatedActionValue);

                        //gets and stores change in Q-value
                        var change = Math.Abs(updatedActionValue - previousActionValue);
                        maximalChange = Math.Max(maximalChange, change);
                    }
            }

            //Console.WriteLine(@"{0} iterations", numIterations);
        }

        #endregion Protected Methods
    }
}