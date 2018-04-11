// ------------------------------------------
// <copyright file="StochasticTransitionAgent.cs" company="Pedro Sequeira">
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

//    Last updated: 12/18/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Managers.Behavior;
using PS.Learning.Core.Domain.Managers.Learning;
using PS.Learning.Core.Domain.Managers.Motivation;
using PS.Learning.Core.Domain.Memories;
using PS.Learning.IMRL.Emotions.Domain.Managers;
using PS.Learning.IMRL.Emotions.Domain.Memories;

namespace PS.Learning.IMRL.Emotions.Domain.Agents
{
    public class StochasticTransitionAgent : Agent, IEmotionalAgent
    {
        public new StochasticTransitionLTM LongTermMemory
        {
            get { return base.LongTermMemory as StochasticTransitionLTM; }
        }

        public new PrioritySweepLearningManager LearningManager
        {
            get { return base.LearningManager as PrioritySweepLearningManager; }
        }

        public new DecreaseEpsilonGreedyBehaviorManager BehaviorManager
        {
            get { return base.BehaviorManager as DecreaseEpsilonGreedyBehaviorManager; }
        }

        public new StochasticMotivationManager MotivationManager
        {
            get { return base.MotivationManager as StochasticMotivationManager; }
        }

        public SchererEmotionsManager EmotionsManager { get; protected set; }

        #region IEmotionalAgent Members

        public new EmotionalSTM ShortTermMemory
        {
            get { return base.ShortTermMemory as EmotionalSTM; }
        }

        EmotionsManager IEmotionalAgent.EmotionsManager
        {
            get { return this.EmotionsManager; }
        }

        public override void Update()
        {
            if (this.LogWriter != null) this.LogWriter.WriteLine("");

            this.ShortTermMemory.PreviousState = this.ShortTermMemory.CurrentState;

            //update behavior (execute "best" action)
            this.BehaviorManager.Update();

            //gets next state
            this.LongTermMemory.GetUpdatedCurrentState();

            //update emotions
            this.MotivationManager.Update();
            this.EmotionsManager.Update();

            //update reward 
            this.ShortTermMemory.CurrentReward.Value = this.MotivationManager.IntrinsicReward.Value;

            //update memories (memorize)
            this.LongTermMemory.Update();
            this.ShortTermMemory.Update();

            //update learning
            this.LearningManager.Update();
        }

        public override void Reset()
        {
            this.EmotionsManager.Reset();
            base.Reset();
        }

        #endregion

        protected override void AddStatisticalQuantities()
        {
            base.AddStatisticalQuantities();

            this.StatisticsCollection.Add("NumBackups", this.LearningManager.NumBackups);
            this.StatisticsCollection.Add(this.ShortTermMemory.Mood.Id, this.ShortTermMemory.Mood);
            this.StatisticsCollection.Add(this.ShortTermMemory.Clarity.Id, this.ShortTermMemory.Clarity);
            foreach (var dimension in this.ShortTermMemory.AppraisalSet.Dimensions.Values)
                this.StatisticsCollection.Add(dimension.Id, dimension);
            foreach (var emotionLabelCount in this.EmotionsManager.EmotionLabelsCount)
                this.StatisticsCollection.Add(emotionLabelCount.Key, emotionLabelCount.Value);
        }

        protected override void CreateActions()
        {
        }

        protected override void CreateManagers()
        {
            base.CreateManagers();
            this.EmotionsManager = this.CreateEmotionsManager();
        }

        protected virtual SchererEmotionsManager CreateEmotionsManager()
        {
            return new SchererEmotionsManager(this);
        }

        protected override LongTermMemory CreateLongTermMemory()
        {
            return new StochasticTransitionLTM(this, this.ShortTermMemory);
        }

        protected override MotivationManager CreateMotivationManager()
        {
            return new StochasticMotivationManager(this);
        }

        protected override ShortTermMemory CreateShortTermMemory()
        {
            return new EmotionalSTM(this);
        }

        protected override LearningManager CreateLearningManager()
        {
            return new PrioritySweepLearningManager(this, this.LongTermMemory);
        }

        protected override BehaviorManager CreateBehaviorManager()
        {
            return new DecreaseEpsilonGreedyBehaviorManager(this);
        }
    }
}