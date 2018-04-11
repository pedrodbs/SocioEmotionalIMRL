// ------------------------------------------
// <copyright file="RewardParametersManager.cs" company="Pedro Sequeira">
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
using System.Collections.Generic;
using PS.Utilities.Math;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.Core.Domain.Managers;
using PS.Learning.IMRL.Domain.Agents;
using PS.Learning.IMRL.Domain.Managers.Motivation;

namespace PS.Learning.IMRL.Domain.Managers.Reward
{
    public abstract class RewardParametersManager : Manager
    {
        private const int UPDATE_STEPS_DEF = 500;
        private uint _updateSteps;
        protected uint curNumSteps;
        protected StatisticalQuantity extrinsicReward;

        protected RewardParametersManager(INORCAgent agent, uint numParams) : base(agent)
        {
            this.UpdateSteps = UPDATE_STEPS_DEF;
            this.NumParams = numParams;

            this.ResetVariables();

            this.RwdParamsStats = new List<StatisticalQuantity>();
            for (var i = 0; i < this.NumParams; i++)
                this.RwdParamsStats.Add(new StatisticalQuantity());
        }

        public uint UpdateSteps
        {
            get { return this._updateSteps; }
            set
            {
                this._updateSteps = value;
                this.extrinsicReward = new StatisticalQuantity(value);
            }
        }

        public new INORCAgent Agent
        {
            get { return base.Agent as INORCAgent; }
        }

        protected DenseVector RewardParameters
        {
            get { return ((ArrayParamMotivationManager) this.Agent.MotivationManager).RewardParameters; }
            set { ((ArrayParamMotivationManager) this.Agent.MotivationManager).RewardParameters = value; }
        }

        public List<StatisticalQuantity> RwdParamsStats { get; private set; }

        public uint NumParams { get; private set; }

        public override void Reset()
        {
        }

        public override void Dispose()
        {
            if (this.extrinsicReward != null)
                this.extrinsicReward.Dispose();

            this.RwdParamsStats.Clear();
            this.RwdParamsStats = null;
        }

        public override void Update()
        {
            //verifies params
            var prevState = this.Agent.ShortTermMemory.PreviousState;
            var action = this.Agent.ShortTermMemory.CurrentAction;
            var curState = this.Agent.ShortTermMemory.CurrentState;
            if ((prevState == null) || (action == null) || (curState == null)) return;

            //stores current extrinsic reward
            this.extrinsicReward.Value = this.Agent.MotivationManager.ExtrinsicReward.Value;

            //updates stats for each param
            for (var i = 0; i < this.NumParams; i++)
                this.RwdParamsStats[i].Value = this.RewardParameters[i];

            //NORC basic implementation, check for rwd params update after some time-interval
            if (++this.curNumSteps < this.UpdateSteps) return;

            //updates params
            this.RewardParameters = this.GetUpdatedRewardParameters(prevState.ID, action.ID, curState.ID);
            //if (this.RewardParameters.Contains(double.NaN))
            this.NormalizeParams();

            //if (this.RewardParameters.Contains(double.NaN))
            //{
            //    int i = 0;
            //    i++;
            //}

            this.ResetVariables();
        }

        protected void NormalizeParams()
        {
            for (var i = 0; i < this.RewardParameters.Count; i++)
                if (this.RewardParameters[i] > 1)
                    this.RewardParameters[i] = 1;
                else if (this.RewardParameters[i] < -1)
                    this.RewardParameters[i] = -1;
        }

        protected virtual void ResetVariables()
        {
            //resets algorithms variables
            this.curNumSteps = 0;
            this.extrinsicReward.Dispose();
            this.extrinsicReward = new StatisticalQuantity(this.UpdateSteps);
        }

        protected abstract DenseVector GetUpdatedRewardParameters(uint prevStateID, uint actionID, uint nextStateID);
    }
}