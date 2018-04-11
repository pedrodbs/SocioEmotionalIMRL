// ------------------------------------------
// <copyright file="StateRelevanceManager.cs" company="Pedro Sequeira">
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

//    Last updated: 3/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Managers;
using PS.Learning.Core.Domain.Managers.Perception;
using PS.Learning.Core.Domain.States;
using PS.Learning.IMRL.Domain.Agents;

namespace PS.Learning.IMRL.Domain.Managers
{
    [Serializable]
    public class StateRelevanceManager : Manager
    {
        public StateRelevanceManager(IIMRLAgent agent) : base(agent)
        {
        }

        public new IIMRLAgent Agent
        {
            get { return base.Agent as IIMRLAgent; }
        }

        public override void Update()
        {
            var prevState = this.Agent.ShortTermMemory.PreviousState;
            if (prevState != null)
                this.VerifyTargetState(prevState);
        }

        public double GetDistanceToGoal(uint stateID)
        {
            if (!(this.Agent.PerceptionManager is CellPerceptionManager))
                return double.MaxValue;

            //tries to get cell corresponding to given state
            var state = this.Agent.LongTermMemory.GetState(stateID);
            var stateCell = ((CellPerceptionManager) this.Agent.PerceptionManager).GetCellFromState(state);

            //updates relevance of state
            this.VerifyTargetState(state);

            //returns distance from given cell to target cell
            return stateCell == null
                       ? double.MaxValue
                       : this.Agent.Environment.GetDistanceToTargetCell(stateCell);
        }

        protected void VerifyTargetState(IState state)
        {
            //verifies state and manager
            if ((state == null) || !(this.Agent.PerceptionManager is CellPerceptionManager))
                return;

            //gets current rwd and compares with best rwd found
            var stateRelevanceValue = this.GetStateRelevanceValue(state.ID);
            if (!this.IsMostRelevantValue(stateRelevanceValue)) return;

            //if better, updates target cells on the environment
            var stateCell = ((CellPerceptionManager) this.Agent.PerceptionManager).GetCellFromState(state);
            this.Agent.Environment.TargetCells.Clear();
            this.Agent.Environment.TargetCells.Add(stateCell);
        }

        protected bool IsMostRelevantValue(double curRelevanceValue)
        {
            return curRelevanceValue.Equals(1f);
            //return curRelevanceValue >= this.BestRewardFound;
        }

        protected double GetStateRelevanceValue(uint state)
        {
            if (state.Equals(uint.MaxValue)) return 0;

            var extrinsicLTM = this.Agent.ExtrinsicLTM;
            var maxStateValue = extrinsicLTM.MaxStateValue;
            var minStateValue = extrinsicLTM.MinStateValue;
            var stateValue = extrinsicLTM.GetMaxStateActionValue(state);
            var relevanceValue = (stateValue - minStateValue)/(maxStateValue - minStateValue);

            return double.IsNaN(relevanceValue) ? 0 : relevanceValue;
            //return this.Agent.MotivationManager.ExtrinsicReward.Value;
        }

        public override void Reset()
        {
            this.Agent.Environment.TargetCells.Clear();
        }


        public override void Dispose()
        {
        }

        public override void PrintResults(string path)
        {
        }
    }
}