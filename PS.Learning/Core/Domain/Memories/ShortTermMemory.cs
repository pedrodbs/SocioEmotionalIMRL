// ------------------------------------------
// <copyright file="ShortTermMemory.cs" company="Pedro Sequeira">
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
using System.IO;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Managers;
using PS.Learning.Core.Domain.States;
using PS.Utilities.Math;

namespace PS.Learning.Core.Domain.Memories
{
    [Serializable]
    public class ShortTermMemory : Manager, IShortTermMemory
    {
        public const string REWARD_STR = "Payoff";
        public const string ACT_VAL_STR = "ActionValue";
        public const string PRED_ERROR_STR = "PredError";
        private double _predictionError;

        public ShortTermMemory(IAgent agent) : base(agent)
        {
            this.CurrentReward = new StatisticalQuantity {Id = REWARD_STR};
            this.PredictionErrorAbs = new StatisticalQuantity {Id = PRED_ERROR_STR};
            this.CurrentStateActionValue = new StatisticalQuantity {Id = ACT_VAL_STR};
        }

        public StatisticalQuantity CurrentStateActionValue { get; protected set; }
        public StatisticalQuantity PredictionErrorAbs { get; protected set; }

        #region IShortTermMemory Members

        public IState CurrentState { get; set; }
        public IState PreviousState { get; set; }
        public IAction CurrentAction { get; set; }
        public StatisticalQuantity CurrentReward { get; private set; }

        public double PredictionError
        {
            get { return this._predictionError; }
            set
            {
                this._predictionError = value;
                this.PredictionErrorAbs.Value = Math.Abs(value);
            }
        }

        public override void Update()
        {
            //updates current action value in short memory
            if ((this.PreviousState != null) && (this.CurrentAction != null))
                this.CurrentStateActionValue.Value =
                    this.Agent.LongTermMemory.GetStateActionValue(this.PreviousState.ID, this.CurrentAction.ID);

            if (this.Agent.LogWriter == null) return;

            this.Agent.LogWriter.WriteLine(
                $@"Total reward after this action: {this.CurrentReward.Value}");
            this.Agent.LogWriter.WriteLine(@"Current State: " + this.CurrentState);
        }

        public override void Reset()
        {
            this.CurrentAction = null;
            this.CurrentState = null;
            this.PreviousState = null;
            this.CurrentReward = new StatisticalQuantity();
            this.PredictionErrorAbs = new StatisticalQuantity();
            this.CurrentStateActionValue = new StatisticalQuantity();
        }

        public virtual void InitElements()
        {
        }

        public override void Dispose()
        {
            this.CurrentReward.Dispose();
            this.CurrentStateActionValue.Dispose();
            this.PredictionErrorAbs.Dispose();
        }

        #endregion

        public override void PrintResults(string path)
        {
            path += "/STM";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            this.CurrentReward.PrintStatisticsToCSV(path + "/Reward.csv");
            this.PredictionErrorAbs.PrintStatisticsToCSV(path + "/PredictionError.csv");
            this.CurrentStateActionValue.PrintStatisticsToCSV(path + "/CurrentStateActionValue.csv");
        }
    }
}