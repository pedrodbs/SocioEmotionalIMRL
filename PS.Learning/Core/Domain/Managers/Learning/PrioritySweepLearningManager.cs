// ------------------------------------------
// <copyright file="PrioritySweepLearningManager.cs" company="Pedro Sequeira">
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
using System.Collections.Generic;
using System.Linq;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Memories;
using PS.Utilities.Collections;
using PS.Utilities.Math;

namespace PS.Learning.Core.Domain.Managers.Learning
{
    [Serializable]
    public class PrioritySweepLearningManager : QLearningManager
    {
        protected bool[][] updatedStateActionValues;

        public PrioritySweepLearningManager(IAgent agent, LongTermMemory longTermMemory)
            : base(agent, longTermMemory)
        {
            //default thresholds
            this.MaxBackups = 10;
            this.MinimumPriority = this.Scenario.TestsConfig.MaximalChangeThreshold;
            this.NumBackups = new StatisticalQuantity();
        }

        public int MaxBackups { get; set; }

        public double MinimumPriority { get; set; }

        public StatisticalQuantity NumBackups { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            this.NumBackups.Dispose();
            this.updatedStateActionValues = null;
        }

        public override void Reset()
        {
            base.Reset();
            this.NumBackups = new StatisticalQuantity();
            this.ClearUpdatedList();
        }

        public override void Update()
        {
            //update action-values (DynaQ)
            var shortTermMemory = this.LongTermMemory.ShortTermMemory;
            var previousState = shortTermMemory.PreviousState;
            var currentAction = shortTermMemory.CurrentAction;
            var currentState = shortTermMemory.CurrentState;

            if ((previousState == null) || (currentAction == null) || (currentState == null)) return;

            this.UpdateStateActionValue(
                previousState.ID, currentAction.ID, currentState.ID, shortTermMemory.CurrentReward.Value);
        }

        public override void PrintResults(string path)
        {
            base.PrintResults(path);
            this.NumBackups.PrintStatisticsToCSV(path + "/NumBackups.csv");
        }

        protected override double GetUpdatedStateActionValue(uint oldStateID, uint actionID, uint newStateID, double reward)
        {
            var ltm = this.LongTermMemory;

            //checks args
            if ((oldStateID == uint.MaxValue) || (actionID == uint.MaxValue)) return 0;

            //get values
            var oldStateActionValue = ltm.GetStateActionValue(oldStateID, actionID);
            var maxFutureValue = ltm.GetMaxStateActionValue(newStateID);

            //returns new value according to Q-Learning algorithm
            return oldStateActionValue + this.LearningRate.Value*
                   (reward + (this.Discount.Value*maxFutureValue) - oldStateActionValue);
        }

        protected override void UpdateStateActionValue(uint oldStateID, uint actionID, uint newStateID, double reward)
        {
            //gets s, a, r, s'
            var ltm = this.LongTermMemory;

            if ((oldStateID == uint.MaxValue) || (oldStateID == uint.MaxValue) || (oldStateID == uint.MaxValue))
            {
                this.NumBackups.Value = 0;
                return;
            }

            //gets state priority p and adds s, a to queue if necessary
            var prevStatePriority = this.GetStateActionPriority(oldStateID, actionID, newStateID, reward);
            if (prevStatePriority.priority <= this.MinimumPriority)
            {
                this.NumBackups.Value = 0;
                return;
            }

            var stateActionQueue = new SortedSet<StateActionPriority>(new StateActionPriority()) {prevStatePriority};
            this.ClearUpdatedList();

            //updates N state-action pairs in queue
            var numBackupsLeft = this.MaxBackups;
            while ((numBackupsLeft > 0) && (stateActionQueue.Count != 0))
            {
                //gets s, a from queue
                var stateActionPriority = stateActionQueue.First();
                var modelStateID = stateActionPriority.stateID;
                var modelActionID = stateActionPriority.actionID;

                //removes s, a from queue
                stateActionQueue.Remove(stateActionPriority);
                numBackupsLeft--;

                //gets predicted (model) r, s'
                var modelNextStateID = ltm.GetRandomStateActionTransition(modelStateID, modelActionID);
                //var modelNextState = ltm.GetMaxStateActionTransition(modelState, modelAction);
                //var modelReward = this.Agent.MotivationManager.GetReward(modelStateID, modelActionID, modelNextStateID);
                var modelReward = ltm.GetStateActionReward(modelStateID, modelActionID);

                if (modelNextStateID == uint.MaxValue) continue;

                //updates state-action (Q) value for s, a, r, s'
                this.UpdateModelStateActionValue(modelStateID, modelActionID, modelNextStateID, modelReward);

                //adds predecessors of s to queue if necessary
                foreach (var predecessor in ltm.GetStatePredecessors(modelStateID))
                {
                    //gets predecessor _a, _s
                    var predecessorStateID = predecessor.Key;
                    var predecessorActionID = predecessor.Value;

                    //ignores predecessors already updated
                    if (this.updatedStateActionValues[predecessorStateID][predecessorActionID])
                        continue;

                    //gets predicted reward _r
                    var predictedReward =
                        ltm.GetStateActionReward(predecessorStateID, predecessorActionID);

                    //gets predecessor priority p
                    var predecessorPriority =
                        this.GetStateActionPriority(predecessorStateID, predecessorActionID, modelStateID,
                            predictedReward);

                    //adds _a, _s, to queue if necessary
                    if (predecessorPriority.priority > this.MinimumPriority)
                        stateActionQueue.Add(predecessorPriority);
                }
            }

            stateActionQueue.Clear();
            this.NumBackups.Value = this.MaxBackups - numBackupsLeft;
        }

        protected StateActionPriority GetStateActionPriority(uint stateID, uint actionID, uint nextStateID,
            double reward)
        {
            var priority = Math.Abs(
                reward + (this.Discount.Value*this.LongTermMemory.GetMaxStateActionValue(nextStateID)) -
                this.LongTermMemory.GetStateActionValue(stateID, actionID));

            return new StateActionPriority(stateID, actionID, priority);
        }

        protected virtual double UpdateModelStateActionValue(uint stateID, uint actionID, uint nextStateID, double reward)
        {
            //stores updated state-action pair reference
            this.updatedStateActionValues[stateID][actionID] = true;

            //updates state action value
            var oldValue = this.LongTermMemory.GetStateActionValue(stateID, actionID);
            var newValue = this.GetUpdatedStateActionValue(stateID, actionID, nextStateID, reward);
            this.LongTermMemory.UpdateStateActionValue(stateID, actionID, newValue);
            return Math.Abs(newValue - oldValue);
        }

        protected virtual void ClearUpdatedList()
        {
            var ltm = this.LongTermMemory;
            this.updatedStateActionValues = ArrayUtil.Create2DArray<bool>(ltm.MaxStates, ltm.NumActions);
        }

        #region Nested type: StateActionPriority

        protected struct StateActionPriority : IComparer<StateActionPriority>
        {
            public uint actionID;
            public double priority;
            public uint stateID;

            public StateActionPriority(uint stateID, uint actionID, double priority)
            {
                this.stateID = stateID;
                this.actionID = actionID;
                this.priority = priority;
            }

            #region IComparer<StateActionPriority> Members

            public int Compare(StateActionPriority x, StateActionPriority y)
            {
                return -x.priority.CompareTo(y.priority);
            }

            #endregion
        }

        #endregion
    }
}