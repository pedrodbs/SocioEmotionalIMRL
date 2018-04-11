// ------------------------------------------
// <copyright file="IntrinsicMotivationManager.cs" company="Pedro Sequeira">
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

//    Last updated: 2/20/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Utilities.Math;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Managers.Motivation;

namespace PS.Learning.IMRL.Domain.Managers.Motivation
{
    public abstract class IntrinsicMotivationManager : EnvironmentMotivationManager, IIntrinsicMotivationManager
    {
        protected IntrinsicMotivationManager(ISituatedAgent agent)
            : base(agent)
        {
            this.IntrinsicReward = new StatisticalQuantity();
        }

        #region IIntrinsicMotivationManager Members

        public StatisticalQuantity IntrinsicReward { get; protected set; }

        public override void Update()
        {
            base.Update();
            var stm = this.Agent.ShortTermMemory;
            this.IntrinsicReward.Value =
                this.GetIntrinsicReward(stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.IntrinsicReward.Dispose();
        }

        public override void Reset()
        {
            base.Reset();
            this.IntrinsicReward = new StatisticalQuantity();
        }

        public abstract double GetIntrinsicReward(uint prevState, uint action, uint nextState);

        public override double GetReward(uint stateID, uint actionID, uint nextStateID)
        {
            return this.GetIntrinsicReward(stateID, actionID, nextStateID);
        }

        #endregion

        public abstract DenseVector GetRewardFeatures(uint prevState, uint action, uint nextState);
    }
}