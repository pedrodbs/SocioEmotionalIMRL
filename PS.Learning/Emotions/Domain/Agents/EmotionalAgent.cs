// ------------------------------------------
// <copyright file="EmotionalAgent.cs" company="Pedro Sequeira">
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

//    Last updated: 03/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using PS.Learning.Core.Domain.Managers.Behavior;
using PS.Learning.Core.Domain.Managers.Learning;
using PS.Learning.Core.Domain.Managers.Motivation;
using PS.Learning.Core.Domain.Memories;
using PS.Learning.IMRL.Domain.Agents;
using PS.Learning.IMRL.Emotions.Domain.Managers;
using PS.Learning.IMRL.Emotions.Domain.Memories;

namespace PS.Learning.IMRL.Emotions.Domain.Agents
{
    [Serializable]
    public class EmotionalAgent : IRAgent, IEmotionalAgent
    {
        public EmotionalAgent()
        {
            this.AutoEat = true;
        }

        public SchererEmotionsManager EmotionsManager { get; protected set; }

        public new PrioritySweepLearningManager LearningManager
        {
            get { return base.LearningManager as PrioritySweepLearningManager; }
        }

        public bool AutoEat { get; set; }

        #region IEmotionalAgent Members

        EmotionsManager IEmotionalAgent.EmotionsManager
        {
            get { return this.EmotionsManager; }
        }

        public new EmotionalSTM ShortTermMemory
        {
            get { return base.ShortTermMemory as EmotionalSTM; }
        }

        public override void Update()
        {
            var stm = this.ShortTermMemory;

            //stores previous state
            this.ExtrinsicSTM.PreviousState =
                this.ShortTermMemory.PreviousState = this.ShortTermMemory.CurrentState;

            //update behavior (act)
            this.BehaviorManager.Update();
            this.ExtrinsicSTM.CurrentAction = this.ShortTermMemory.CurrentAction;

            //update environment
            this.Environment.Update();

            //update perception and stms
            this.PerceptionManager.Update();
            this.ExtrinsicSTM.CurrentState =
                this.ShortTermMemory.CurrentState = this.LongTermMemory.GetUpdatedCurrentState();
            this.ShortTermMemory.Update();
            this.ExtrinsicSTM.Update();

            //update extrinsic reward, ltm and learning
            if (stm.PreviousState != null)
                this.ExtrinsicSTM.CurrentReward.Value =
                    this.MotivationManager.ExtrinsicReward.Value =
                        this.MotivationManager.GetExtrinsicReward(stm.PreviousState.ID, stm.CurrentAction.ID);

            this.ExtrinsicLTM.Update();
            this.ExtrinsicLearningManager.Update();
            this.ExtrinsicLTM.UpdateMinMaxValues();

            //update emotions (feel)
            this.StateRelevanceManager.Update();
            this.EmotionsManager.Update();

            //update intrinsic reward, ltm and learning
            if (stm.PreviousState != null)
                this.ShortTermMemory.CurrentReward.Value =
                    this.MotivationManager.IntrinsicReward.Value = this.MotivationManager.GetIntrinsicReward(
                        stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID);

            this.LongTermMemory.Update();
            this.LearningManager.Update();
        }

        public override void PrintAll(string path, string imgFormat)
        {
            //base.PrintAll(path, imgFormat);

            this.EmotionsManager.PrintResults(path);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.EmotionsManager.Dispose();
        }

        public override void Reset()
        {
            this.EmotionsManager.Reset();
            base.Reset();
        }

        #endregion

        protected override void CreateActions()
        {
            base.CreateActions();
            if (this.AutoEat && this.Actions.ContainsKey(EAT_ACTION_ID))
                this.Actions.Remove(EAT_ACTION_ID);
        }

        protected override void CreateManagers()
        {
            base.CreateManagers();
            this.EmotionsManager = this.CreateEmotionsManager();
        }

        protected virtual SchererEmotionsManager CreateEmotionsManager()
        {
            return new GeneticsEmotionsManager(this);
            //return new SchererEmotionsManager(this);
        }

        protected override ShortTermMemory CreateShortTermMemory()
        {
            return new EmotionalSTM(this);
        }

        protected override MotivationManager CreateMotivationManager()
        {
            return new EmotionalHungryMotivationManager(this);
        }

        protected override LongTermMemory CreateLongTermMemory()
        {
            //return new StateTreeLTM(this, this.ShortTermMemory);
            return new StringLTM(this, this.ShortTermMemory);
        }

        protected override BehaviorManager CreateBehaviorManager()
        {
            return new DecreaseEpsilonGreedyBehaviorManager(this);
            //return new BoltzmannBehaviorManager(this);
        }

        protected override LearningManager CreateLearningManager()
        {
            return new PrioritySweepLearningManager(this, this.LongTermMemory);
        }

        protected override void AddStatisticalQuantities()
        {
            base.AddStatisticalQuantities();

            foreach (var dimension in this.ShortTermMemory.AppraisalSet.Dimensions.Values)
                this.StatisticsCollection.Add(dimension.Id, dimension);
            foreach (var emotionLabelCount in this.EmotionsManager.EmotionLabelsCount)
                this.StatisticsCollection.Add(emotionLabelCount.Key, emotionLabelCount.Value);
        }
    }
}