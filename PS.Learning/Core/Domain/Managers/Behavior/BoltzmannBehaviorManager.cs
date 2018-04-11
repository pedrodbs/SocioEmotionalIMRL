// ------------------------------------------
// <copyright file="BoltzmannBehaviorManager.cs" company="Pedro Sequeira">
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
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;

namespace PS.Learning.Core.Domain.Managers.Behavior
{
    [Serializable]
    public class BoltzmannBehaviorManager : DecreaseEpsilonGreedyBehaviorManager
    {
        public BoltzmannBehaviorManager(IAgent agent) : base(agent)
        {
            //default eps value
            this.StartingEpsilon = this.Scenario.TestsConfig.Temperature;
        }

        protected override Policy GetStatePolicy(uint stateID)
        {
            //for each action get weightevalue according to Q and Eps (temp)
            var policy = new Policy((uint) this.Actions.Count, false);
            for (var i = 0u; i < this.Actions.Count; i++)
                policy[i] = this.GetWeightedQValue(stateID, i);
            return policy;
        }

        private double GetWeightedQValue(uint stateID, uint actionID)
        {
            var qValue = this.Agent.LongTermMemory.GetStateActionValue(stateID, actionID);
            return System.Math.Exp(qValue/this.Epsilon.Value);
        }

        protected override double GetUpdatedEpsilonValue()
        {
            //decreases epsilon overtime
            return this.StartingEpsilon*
                   System.Math.Pow(this.ExploratoryDecay, -(double) this.Agent.LongTermMemory.TimeStep);
        }
    }
}