// ------------------------------------------
// <copyright file="SchererEmotionsManager.cs" company="Pedro Sequeira">
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
using PS.Learning.IMRL.Emotions.Domain.Agents;

namespace PS.Learning.IMRL.Emotions.Domain.Managers
{
    [Serializable]
    public class SchererEmotionsManager : EmotionsManager
    {
        public SchererEmotionsManager(IAgent agent) : base(agent)
        {
        }

        public new EmotionalAgent Agent
        {
            get { return base.Agent as EmotionalAgent; }
        }

        public override double GetArousal(uint prevState, uint action, uint nextState)
        {
            //Arousal(t) = 
            //  [0,3 * (|Rt - RAvg| / 2 RStdDev)] + 
            //  [0,4 * (1 - Control(t))] + 
            //  [0,3 * GoalRelevance(t)] + 

            // gets arousal's contributions
            var currentReward = this.Agent.MotivationManager.ExtrinsicReward;
            //var currentReward = this.Agent.ShortTermMemory.CurrentReward;
            var rewardContrib = currentReward.StdDev.Equals(0.0)
                ? 0
                : Math.Abs(
                    currentReward.Value - currentReward.Mean)/(2*currentReward.StdDev);

            if (rewardContrib < 0) rewardContrib = 0;
            else if (rewardContrib > 1) rewardContrib = 1;
            rewardContrib *= 0.3d;
            var controlContrib = 0.4d*(1d - this.AppraisalSet.Control.Value);
            var motivationContrib = 0.3d*this.AppraisalSet.GoalRelevance.Value;

            //weighted sum to arousal
            var arousalValue = rewardContrib + motivationContrib + controlContrib;

            return IsInvalidReward(arousalValue) ? 0 : arousalValue;
        }

        public override double GetValence(uint prevState, uint action, uint newState)
        {
            var maxStateValue = this.Agent.ExtrinsicLTM.MaxStateValue;
            var minStateValue = this.Agent.ExtrinsicLTM.MinStateValue;
            var curStateValue = this.Agent.ExtrinsicLTM.GetMaxStateActionValue(prevState);
            var curStateActionValue = this.Agent.ExtrinsicLTM.GetStateActionValue(prevState, action);

            var valenceStateValue = (curStateValue - minStateValue)/(maxStateValue - minStateValue);
            var valenceStateActionValue = curStateActionValue/curStateValue;
            var valenceValue = .5f*valenceStateActionValue + .5f*valenceStateValue;

            return IsInvalidReward(valenceValue) ? 0 : valenceValue;
        }

        protected virtual double GetInverseRecency(uint state, uint action)
        {
            var timeStateAction = this.Agent.LongTermMemory.GetTimeStepsLastStateAction(state, action);

            return timeStateAction == 0 ? 1 : 1 - (1/timeStateAction);
        }

        public override double GetNovelty(uint prevState, uint action, uint nextState)
        {
            //Novelty(t) = (0,5 * 1.001^-Count(s, a)) + (0,5 * 1.001^-Count(s))
            var stateActionCount =
                this.Agent.LongTermMemory.GetStateActionCount(prevState, action);

            var stateCount = this.Agent.LongTermMemory.GetStateCount(prevState);

            var noveltyValue = ((0.5d*Math.Pow(this.NoveltyDecay, -stateCount)) +
                                (0.5d*Math.Pow(this.NoveltyDecay, -stateActionCount)));

            return IsInvalidReward(noveltyValue) ? 0 : noveltyValue;
        }

        public override double GetGoalRelevance(uint prevState, uint action, uint nextState)
        {
            //GoalRelevance(t) = 1 / (d(s, BestRextS) + 1)

            var distanceToBestRwd = this.Agent.StateRelevanceManager.GetDistanceToGoal(prevState);
            var goalRelevanceValue = 1d/(distanceToBestRwd + 1d);
            return IsInvalidReward(goalRelevanceValue) ? 0 : goalRelevanceValue;
        }

        public override double GetControl(uint prevState, uint action, uint nextState)
        {
            //Control(t) = 1-PredErr(t)
            var stateActionPredErrAvg = this.Agent.ExtrinsicLTM.GetStateActionPredErrAvg(prevState, action);

            var clarityValue = 1d - stateActionPredErrAvg;

            return IsInvalidReward(clarityValue) ? 0 : clarityValue;
        }

        public override double GetMood(uint prevState, uint action, uint nextState)
        {
            //Mood(t) = ( Arousal(t) * Valence(t) ) + ( (1 - Arousal(t)) * Mood(t-1) )
            var moodValue =
                (this.AppraisalSet.Arousal.Value*this.AppraisalSet.Valence.Value) +
                ((1 - this.AppraisalSet.Arousal.Value)*this.emotionalSTM.Mood.Value);

            return IsInvalidReward(moodValue) ? 0 : moodValue;
        }

        protected static bool IsInvalidReward(double value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }

        protected double GetIndicator(bool value)
        {
            return value ? 1 : 0;
        }
    }
}