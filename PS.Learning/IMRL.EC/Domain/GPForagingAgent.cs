// ------------------------------------------
// <copyright file="GPForagingAgent.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL.EC

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
using PS.Learning.IMRL.Domain.Managers.Motivation;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.IMRL.EC.Testing;

namespace PS.Learning.IMRL.EC.Domain
{
    [Serializable]
    public class GPForagingAgent : IRAgent, IGPAgent, IIMRLAgent
    {
        public GPForagingAgent()
        {
            this.AutoEat = true;
        }

        public new StringLTM LongTermMemory
        {
            get { return base.LongTermMemory as StringLTM; }
        }

        public new PrioritySweepLearningManager LearningManager
        {
            get { return base.LearningManager as PrioritySweepLearningManager; }
        }

        public bool AutoEat { get; set; }

        #region IGPAgent Members

        public IGPChromosome Chromosome
        {
            get { return this.TestParameters as GPChromosome; }
        }

        public new GPMotivationManager MotivationManager
        {
            get { return base.MotivationManager as GPMotivationManager; }
        }

        public override void Update()
        {
            var stm = this.ShortTermMemory;

            //stores previous state
            this.ExtrinsicSTM.PreviousState =
                stm.PreviousState = stm.CurrentState;

            //update behavior (act)
            this.BehaviorManager.Update();
            this.ExtrinsicSTM.CurrentAction = stm.CurrentAction;

            //update environment
            this.Environment.Update();

            //update perception and stm
            this.PerceptionManager.Update();
            this.ExtrinsicSTM.CurrentState =
                stm.CurrentState = this.LongTermMemory.GetUpdatedCurrentState();
            stm.Update();
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

            //update intrinsic reward, ltm and learning
            if (stm.PreviousState != null)
                stm.CurrentReward.Value =
                    this.MotivationManager.IntrinsicReward.Value =
                        this.MotivationManager.GetIntrinsicReward(
                            stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID);
            this.LongTermMemory.Update();
            this.LearningManager.Update();
        }

        #endregion

        #region IIMRLAgent Members

        EpsilonGreedyBehaviorManager IIMRLAgent.BehaviorManager
        {
            get { return this.BehaviorManager; }
        }

        IntrinsicMotivationManager IIMRLAgent.MotivationManager
        {
            get { return this.MotivationManager; }
        }

        #endregion

        protected override void CreateActions()
        {
            base.CreateActions();
            if (this.AutoEat && this.Actions.ContainsKey(EAT_ACTION_ID))
                this.Actions.Remove(EAT_ACTION_ID);
        }

        protected override LongTermMemory CreateLongTermMemory()
        {
            return new StringLTM(this, this.ShortTermMemory);
        }

        protected override MotivationManager CreateMotivationManager()
        {
            var testsConfig = (GPTestsConfig) this.Scenario.TestsConfig;
            return new ForagingGPMotivationManager(this, testsConfig.Constants);
        }
    }
}