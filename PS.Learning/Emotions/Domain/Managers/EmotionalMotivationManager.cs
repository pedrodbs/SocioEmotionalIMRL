// ------------------------------------------
// <copyright file="EmotionalMotivationManager.cs" company="Pedro Sequeira">
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

//    Last updated: 10/10/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.IMRL.Domain.Managers.Motivation;
using PS.Learning.IMRL.Emotions.Domain.Agents;

namespace PS.Learning.IMRL.Emotions.Domain.Managers
{
    [Serializable]
    public class EmotionalMotivationManager : ArrayParamMotivationManager
    {
        private AppraisalSet _appraisalSet;

        public EmotionalMotivationManager(EmotionalAgent agent)
            : base(agent)
        {
        }

        public new IEmotionalAgent Agent
        {
            get { return base.Agent as IEmotionalAgent; }
        }

        public void CreateLabel(uint prevState, uint action, uint nextState)
        {
            var rewardFeatures = this.GetRewardFeatures(prevState, action, nextState);
            this._appraisalSet = new AppraisalSet
                                 {
                                     Novelty = {Value = rewardFeatures[0]},
                                     GoalRelevance = {Value = rewardFeatures[1]},
                                     Control = {Value = rewardFeatures[2]},
                                     Valence = {Value = rewardFeatures[3]},
                                     Arousal = {Value = rewardFeatures[4]},
                                 };
        }

        public override DenseVector GetRewardFeatures(uint prevState, uint action, uint nextState)
        {
            return new DenseVector(new[]
                                   {
                                       this.Agent.EmotionsManager.GetNovelty(prevState, action, nextState),
                                       this.Agent.EmotionsManager.GetGoalRelevance(prevState, action, nextState),
                                       this.Agent.EmotionsManager.GetControl(prevState, action, nextState),
                                       this.Agent.EmotionsManager.GetValence(prevState, action, nextState),
                                       this.GetExtrinsicReward(prevState, action)
                                   });
        }
    }
}