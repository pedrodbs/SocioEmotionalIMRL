// ------------------------------------------
// <copyright file="MotivationManager.cs" company="Pedro Sequeira">
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
using PS.Utilities.Math;

namespace PS.Learning.Core.Domain.Managers.Motivation
{
    [Serializable]
    public abstract class MotivationManager : Manager, IMotivationManager
    {
        protected double extrinsicReward;

        protected MotivationManager(IAgent agent) : base(agent)
        {
            this.ExtrinsicReward = new StatisticalQuantity();
        }

        #region IMotivationManager Members

        public StatisticalQuantity ExtrinsicReward { get; protected set; }

        public virtual double GetReward(uint prevStateID, uint actionID, uint nextStateID)
        {
            return this.GetExtrinsicReward(prevStateID, actionID);
        }

        public override void Update()
        {
            this.ExtrinsicReward.Value = this.GetExtrinsicReward(
                this.Agent.ShortTermMemory.PreviousState.ID, this.Agent.ShortTermMemory.CurrentAction.ID);
        }

        public override void Dispose()
        {
            this.ExtrinsicReward.Dispose();
        }

        public override void Reset()
        {
            this.ExtrinsicReward = new StatisticalQuantity();
        }

        #endregion

        public abstract double GetExtrinsicReward(uint stateID, uint actionID);

        public override void PrintResults(string path)
        {
        }
    }
}