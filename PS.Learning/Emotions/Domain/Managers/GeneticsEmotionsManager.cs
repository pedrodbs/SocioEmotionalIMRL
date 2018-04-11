// ------------------------------------------
// <copyright file="GeneticsEmotionsManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.Emotions

//    Last updated: 10/9/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Agents;

namespace PS.Learning.IMRL.Emotions.Domain.Managers
{
    [Serializable]
    public class GeneticsEmotionsManager : SchererEmotionsManager
    {
        private const long NORMALIZATION_FACTOR = -100000000;

        public GeneticsEmotionsManager(IAgent agent) : base(agent)
        {
        }

        public override double GetArousal(uint prevState, uint action, uint nextState)
        {
            return 0.5d;
        }

        public override double GetValence(uint prevState, uint action, uint newState)
        {
            //Valence(t) = Q(s, a) - V(s)

            //checks args and gets the value of state s after action a within current history
            var stateActionValue = ((prevState.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue)))
                                       ? 0
                                       : this.Agent.ExtrinsicLTM.GetStateActionValue(prevState, action);
            var stateValue = (prevState.Equals(uint.MaxValue))
                                 ? 0
                                 : this.Agent.ExtrinsicLTM.GetMaxStateActionValue(prevState);
            var valenceValue = stateActionValue - stateValue;

            return double.IsNaN(valenceValue) ? 0 : valenceValue;
        }

        public override double GetNovelty(uint prevState, uint action, uint nextState)
        {
            //Novelty(t) = - Count(s)^2
            var stateCount = this.Agent.ExtrinsicLTM.GetStateCount(nextState);
            var noveltyValue = (stateCount*stateCount)/NORMALIZATION_FACTOR;

            return double.IsNaN(noveltyValue) ? 0 : noveltyValue;
        }

        public override double GetGoalRelevance(uint prevState, uint action, uint nextState)
        {
            //GoalRelevance(t) = Q(s,a)

            //checks args and gets the value of state s after action a within current history
            var goalRelevanceValue = ((prevState.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue)))
                                         ? 0
                                         : this.Agent.ExtrinsicLTM.GetStateActionValue(prevState, action);

            return double.IsNaN(goalRelevanceValue) ? 0 : goalRelevanceValue;
        }

        public override double GetControl(uint prevState, uint action, uint nextState)
        {
            //Control(t) = Prob(s'|s,a)

            //checks args and return transition prob
            var clarityValue = ((prevState.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue)) ||
                                (nextState.Equals(uint.MaxValue)))
                                   ? 0
                                   : this.Agent.ExtrinsicLTM.GetStateActionTransitionProbability(
                                       prevState, action, nextState);

            return double.IsNaN(clarityValue) ? 0 : clarityValue;
        }
    }
}