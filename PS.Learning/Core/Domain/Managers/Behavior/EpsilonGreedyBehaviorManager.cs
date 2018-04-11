// ------------------------------------------
// <copyright file="EpsilonGreedyBehaviorManager.cs" company="Pedro Sequeira">
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
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Utilities.Core;
using PS.Utilities.Math;
using System;
using System.IO;

namespace PS.Learning.Core.Domain.Managers.Behavior
{
    [Serializable]
    public class EpsilonGreedyBehaviorManager : BehaviorManager
    {
        #region Private Fields

        private const string EPS_FILE_NAME = "Epsilon.csv";

        #endregion Private Fields

        #region Public Constructors

        public EpsilonGreedyBehaviorManager(IAgent agent) : base(agent)
        {
            this.Epsilon = new StatisticalQuantity();
            this.StartingEpsilon = this.Scenario.TestsConfig.Epsilon;
        }

        #endregion Public Constructors

        #region Public Properties

        public StatisticalQuantity Epsilon { get; private set; }
        public double StartingEpsilon { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void Dispose()
        {
            this.Epsilon.Dispose();
        }

        public override void PrintResults(string path)
        {
            this.Epsilon.PrintStatisticsToCSV(
                $"{path}{Path.DirectorySeparatorChar}{EPS_FILE_NAME}");
        }

        public override void Reset()
        {
            this.Epsilon = new StatisticalQuantity(this.Epsilon.Value, new Range<double>(0, 1));
        }

        public override void Update()
        {
            base.Update();
            this.UpdateEpsilon();
        }

        public virtual void UpdateEpsilon()
        {
            //updates epsilon
            this.Epsilon.Value = this.GetUpdatedEpsilonValue();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override Policy GetStatePolicy(uint stateID)
        {
            //initialize policy uniformly
            var policy = new Policy((uint)this.Actions.Count, false);

            //gets best action and tests
            var bestActionID = this.Agent.LongTermMemory.GetMaxStateAction(stateID);
            if (bestActionID == uint.MaxValue) return policy;

            var prob = Rand.NextDouble();
            var greedy = prob > this.Epsilon.Value;

            //sets prob according to whether action is the best and current epsilon value
            var greedyProb = 1d - this.Epsilon.Value;
            var otherActionsProb = this.Epsilon.Value / (this.Actions.Count - 1d);

            for (var i = 0u; i < this.Actions.Count; i++)
                policy[i] = greedy ? (i == bestActionID ? 1 : 0) : 1d / this.Actions.Count;
            //policy[i] = i == bestActionID ? greedyProb : otherActionsProb;

            return policy;
        }

        protected virtual double GetUpdatedEpsilonValue()
        {
            //just keep epsilon constant
            return this.StartingEpsilon;
        }

        #endregion Protected Methods
    }
}