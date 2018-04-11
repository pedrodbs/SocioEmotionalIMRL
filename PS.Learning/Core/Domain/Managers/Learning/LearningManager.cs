// ------------------------------------------
// <copyright file="LearningManager.cs" company="Pedro Sequeira">
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
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Memories;
using PS.Utilities.Core;
using PS.Utilities.Math;
using System;

namespace PS.Learning.Core.Domain.Managers.Learning
{
    [Serializable]
    public abstract class LearningManager : Manager
    {
        #region Protected Constructors

        protected LearningManager(IAgent agent, ILongTermMemory longTermMemory)
            : base(agent)
        {
            this.LongTermMemory = longTermMemory;
            this.LearningRate = new StatisticalQuantity(this.Scenario.TestsConfig.LearningRate, new Range<double>(0d, 1d));
            this.Discount = new StatisticalQuantity(this.Scenario.TestsConfig.Discount, new Range<double>(0d, 1d));
        }

        #endregion Protected Constructors

        #region Public Properties

        public StatisticalQuantity Discount { get; protected set; }
        public StatisticalQuantity LearningRate { get; protected set; }
        public ILongTermMemory LongTermMemory { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        public override void Dispose()
        {
            this.LearningRate.Dispose();
            this.Discount.Dispose();
        }

        public override void PrintResults(string path)
        {
            this.LearningRate.PrintStatisticsToCSV(path + "/LearningRate.csv");
        }

        public override void Reset()
        {
            this.LearningRate = new StatisticalQuantity(this.LearningRate.Value, new Range<double>(0d, 1d));
            this.Discount = new StatisticalQuantity(this.Discount.Value, new Range<double>(0d, 1d));
        }

        public override void Update()
        {
            //updates action-value function of learning algorithm
            var stm = this.LongTermMemory.ShortTermMemory;
            if (stm.PreviousState == null) return;

            this.UpdateStateActionValue(
                stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID, stm.CurrentReward.Value);
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract double GetUpdatedStateActionValue(
            uint oldStateID, uint actionID, uint newStateID, double reward);

        protected virtual void UpdateStateActionValue(uint oldStateID, uint actionID, uint newStateID, double reward)
        {
            //checks args
            if ((oldStateID == uint.MaxValue) || (actionID == uint.MaxValue) || (newStateID == uint.MaxValue))
                return;

            //gets updated Q-value
            var updatedActionValueFunction = this.GetUpdatedStateActionValue(oldStateID, actionID, newStateID, reward);

            //updates action-value
            this.LongTermMemory.UpdateStateActionValue(oldStateID, actionID, updatedActionValueFunction);
        }

        #endregion Protected Methods
    }
}