// ------------------------------------------
// EchoSocialAgent.cs, Learning.Tests.AltruismGPOptimization
//
// Created by Pedro Sequeira, 2014/2/20
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
using PS.Learning.Tests.AltruismGPOptimization.Domain.Managers;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;
using PS.Learning.Tests.AltruismOptimization.Testing;

namespace PS.Learning.Tests.AltruismGPOptimization.Domain.Agents
{
    [Serializable]
    public class EchoSocialAgent : IRAgent, IFoodSharingAgent
    {
        public const string ECHO_ACTION_ID = "Echo";

        protected override string MemoryBaseFilePath
        {
            get
            {
                return Path.GetFullPath(
                    string.Format("{0}{1}{2}{1}LTM", this.Scenario.TestsConfig.MemoryBaseFilePath,
                        Path.DirectorySeparatorChar, this.IdToken));
            }
        }

        public new IFoodSharingScenario Scenario
        {
            get { return base.Scenario as IFoodSharingScenario; }
            set { base.Scenario = value; }
        }

        #region IFoodSharingAgent Members

        IHungerMotivationManager IFoodSharingAgent.MotivationManager
        {
            get { return (IHungerMotivationManager) this.MotivationManager; }
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
            stm.PreviousState = stm.CurrentState;

            //choose next action to be executed based on state
            this.BehaviorManager.DeliberateNextAction();
        }

        public override void Update()
        {
            //update perception and stms
            var stm = this.ShortTermMemory;
            this.PerceptionManager.Update();
            stm.CurrentState = this.LongTermMemory.GetUpdatedCurrentState();
            stm.Update();

            if (this.ShortTermMemory.PreviousState != null)
            {
                //update social environment perception
                this.SocialManager.Update();

                //update motivation (rewards)
                this.MotivationManager.Update();

                //update intrinsic reward, ltm and learning
                stm.CurrentReward.Value =
                    this.MotivationManager.IntrinsicReward.Value;
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
            return new EchoSocialManager(this);
        }

        protected override PerceptionManager CreatePerceptionManager()
        {
            //return new FoodCyclesPerceptionManager(this);
            return new OthersEchoPerceptionManager(this);
        }

        protected override MotivationManager CreateMotivationManager()
        {
            return new EchoMotivationManager(this);
        }

        protected override void CreateActions()
        {
            base.CreateActions();

            //removes eat action if 'auto eat'
            if (this.Environment.AutoEat)
                this.Actions.Remove(EAT_ACTION_ID);

            ////add eacho action
            //this.Actions.Add(ECHO_ACTION_ID, new Action(ECHO_ACTION_ID, this));
        }

        protected override void AddStatisticalQuantities()
        {
            base.AddStatisticalQuantities();

            var socialManager = (EchoSocialManager) this.SocialManager;
            this.StatisticsCollection.Add("OtherHungerSignal", socialManager.OtherHungerSignal);
            this.StatisticsCollection.Add("OtherWorseSignal", socialManager.OtherWorseSignal);
            this.StatisticsCollection.Add("OtherPresenceSignal", socialManager.OtherPresenceSignal);
            this.StatisticsCollection.Add("IntPerfSignal", socialManager.IntPerfSignal);
            this.StatisticsCollection.Add("IntEchoSignal", socialManager.IntEchoSignal);
            this.StatisticsCollection.Add("ExtEchoSignal", socialManager.ExtEchoSignal);
            this.StatisticsCollection.Add("Cumulative Social Encounters", socialManager.CumulativeSocialEncounters);
        }

        public int CompareTo(ISocialAgent other)
        {
            return this.Fitness.Value.CompareTo(other.Fitness.Value);
        }
    }
}