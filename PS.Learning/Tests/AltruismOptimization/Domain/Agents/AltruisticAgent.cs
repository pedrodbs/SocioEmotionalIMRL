// ------------------------------------------
// AltruisticAgent.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/3
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System;
using System.IO;
using PS.Learning.Core.Domain.Managers.Motivation;
using PS.Learning.Core.Domain.Managers.Perception;
using PS.Learning.Core.Domain.Memories;
using PS.Learning.IMRL.Domain.Agents;
using PS.Learning.IMRL.Domain.Managers.Motivation;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Environments;
using PS.Learning.Social.Domain.Managers;
using PS.Learning.Social.Testing;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;
using PS.Learning.Tests.AltruismOptimization.Domain.Managers;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Agents
{
    [Serializable]
    public class AltruisticAgent : IRAgent, IFoodSharingAgent
    {
        protected const string DO_NOTHING_ACTION_ID = "DoNothing";

        public AltruisticAgent()
        {
            this.ImagePath = string.Format("../../../../bin/resources/{0}{1}.png",
                this.Scenario.AgentImgPrefix, this.AgentIdx + 1);
        }

        public new AltruismMotivationManager MotivationManager
        {
            get { return base.MotivationManager as AltruismMotivationManager; }
        }

        protected override string MemoryBaseFilePath
        {
            get
            {
                return Path.GetFullPath(
                    string.Format("{0}{1}{2}{1}LTM", this.Scenario.TestsConfig.MemoryBaseFilePath,
                        Path.DirectorySeparatorChar, this.IdToken));
            }
        }

        public new ISocialScenario Scenario
        {
            get { return base.Scenario as ISocialScenario; }
        }

        #region IFoodSharingAgent Members

        IHungerMotivationManager IFoodSharingAgent.MotivationManager
        {
            get { return this.MotivationManager; }
        }

        public new ShortTermMemory ShortTermMemory
        {
            get { return base.ShortTermMemory; }
        }

        public uint AgentIdx { get; set; }

        public bool IsStronger { get; set; }

        public new IFoodSharingEnvironment Environment
        {
            get { return base.Environment as IFoodSharingEnvironment; }
            set { base.Environment = value; }
        }

        ISocialEnvironment ISocialAgent.Environment
        {
            get { return base.Environment as ISocialEnvironment; }
            set { base.Environment = value; }
        }

        public ISocialManager SocialManager { get; private set; }

        public override void Reset()
        {
            this.SocialManager.Reset();
            base.Reset();
        }

        public override void Dispose()
        {
            base.Dispose();
            this.SocialManager.Dispose();
        }

        public virtual void ChooseNextAction()
        {
            var stm = this.ShortTermMemory;

            //stores previous state
            this.ExtrinsicSTM.PreviousState = stm.PreviousState = stm.CurrentState;

            //choose next action to be executed based on state
            this.BehaviorManager.DeliberateNextAction();
            this.ExtrinsicSTM.CurrentAction = stm.CurrentAction;
        }

        public override void Update()
        {
            //update perception and stms
            var stm = this.ShortTermMemory;
            this.PerceptionManager.Update();
            this.ExtrinsicSTM.CurrentState = stm.CurrentState = this.LongTermMemory.GetUpdatedCurrentState();
            stm.Update();
            this.ExtrinsicSTM.Update();

            if (this.ShortTermMemory.PreviousState != null)
            {
                //update extrinsic reward, ltm and learning
                //this.ExtrinsicSTM.CurrentReward.Value =
                //this.MotivationManager.ExtrinsicReward.Value =
                //    this.MotivationManager.GetExtrinsicReward(stm.PreviousState.ID, stm.CurrentAction.ID);
                //this.ExtrinsicLTM.Update();
                //this.ExtrinsicLearningManager.Update();
                //this.ExtrinsicLTM.UpdateMinMaxValues();

                //update social environment perception
                this.SocialManager.Update();

                //this.StateRelevanceManager.Update();

                //update motivation (rewards)
                this.MotivationManager.Update();

                //update intrinsic reward, ltm and learning
                stm.CurrentReward.Value =
                    this.MotivationManager.IntrinsicReward.Value; //=
                //this.MotivationManager.GetIntrinsicReward(
                //    stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID);
            }

            //update memories (memorize)
            this.LongTermMemory.Update();

            //updates "true" learning
            if (stm.PreviousState != null) this.LearningManager.Update();
        }

        public virtual void ExecuteAction()
        {
            //executes chosen action and updates epsilon
            this.BehaviorManager.ExecuteChosenAction();
            this.BehaviorManager.UpdateEpsilon();
        }

        public override void Init()
        {
            //establishes stronger agent
            if (this.Scenario.StrongerAgent && (this.AgentIdx == 0))
                this.IsStronger = true;

            base.Init();
        }

        #endregion

        protected override void CreateManagers()
        {
            base.CreateManagers();
            this.SocialManager = this.CreateSocialManager();
        }

        protected virtual ISocialManager CreateSocialManager()
        {
            return new OthersHungerAltruismManager(this);
            //return new OthersFitnessAltruismManager(this);
            //return new FoodCyclesAltruismManager(this);
        }

        protected override PerceptionManager CreatePerceptionManager()
        {
            return new OthersHungerPerceptionManager(this);
            //return new OthersFitnessPerceptionManager(this);
            //return new FoodCyclesPerceptionManager(this);
        }

        protected override MotivationManager CreateMotivationManager()
        {
            return new AltruismMotivationManager(this);
        }

        protected override void CreateActions()
        {
            base.CreateActions();

            //add do nothing action
            //this.Actions.Add(DO_NOTHING_ACTION_ID, new Action(DO_NOTHING_ACTION_ID, this));

            //removes eat action if 'auto eat'
            if (this.Environment.AutoEat)
                this.Actions.Remove(EAT_ACTION_ID);
        }

        protected override void AddStatisticalQuantities()
        {
            base.AddStatisticalQuantities();

            var socialManager = (AltruismSocialManager) this.SocialManager;
            this.StatisticsCollection.Add("Internal L-signal", socialManager.InternalLegitimacySignal);
            this.StatisticsCollection.Add("External L-signal", socialManager.ExternalLegitimacySignal);
            this.StatisticsCollection.Add("Social signal", socialManager.SocialSignal);
            this.StatisticsCollection.Add("Cumulative Social Encounters", socialManager.CumulativeSocialEncounters);
        }

        public int CompareTo(ISocialAgent other)
        {
            return this.Fitness.Value.CompareTo(other.Fitness.Value);
        }
    }
}