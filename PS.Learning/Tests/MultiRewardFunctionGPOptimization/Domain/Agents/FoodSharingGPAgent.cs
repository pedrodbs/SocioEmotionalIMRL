// ------------------------------------------
// FoodSharingGPAgent.cs, Learning.Tests.MultiRewardFunctionGPOptimization
//
// Created by Pedro Sequeira, 2013/3/26
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using PS.Learning.Core.Domain.Managers.Motivation;
using PS.Learning.Core.Domain.Managers.Perception;
using PS.Learning.Core.Domain.Memories;
using PS.Learning.IMRL.Domain.Managers.Motivation;
using PS.Learning.IMRL.EC.Domain;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Environments;
using PS.Learning.Social.Domain.Managers;
using PS.Learning.Social.IMRL.EC.Domain;
using PS.Learning.Social.IMRL.EC.Testing;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;
using PS.Learning.Tests.AltruismOptimization.Domain.Managers;
using PS.Learning.Tests.AltruismOptimization.Testing;
using PS.Learning.Tests.MultiRewardFunctionGPOptimization.Domain.Managers;

namespace PS.Learning.Tests.MultiRewardFunctionGPOptimization.Domain.Agents
{
    public class FoodSharingGPAgent : GPForagingAgent, ISocialGPAgent, IFoodSharingAgent
    {
        public FoodSharingGPAgent()
        {
            this.AutoEat = true;
        }

        public new SocialGPMotivationManager MotivationManager
        {
            get { return base.MotivationManager as SocialGPMotivationManager; }
        }

        public GPSocialManager SocialManager { get; private set; }

        public new IFoodSharingScenario Scenario
        {
            get { return base.Scenario as IFoodSharingScenario; }
        }

        #region IFoodSharingAgent Members

        public bool IsStronger { get; set; }

        IFoodSharingEnvironment IFoodSharingAgent.Environment
        {
            get { return this.Environment as IFoodSharingEnvironment; }
            set { this.Environment = value; }
        }

        IHungerMotivationManager IFoodSharingAgent.MotivationManager
        {
            get { return this.MotivationManager; }
        }

        #endregion

        #region ISocialGPAgent Members

        ISocialManager ISocialAgent.SocialManager
        {
            get { return this.SocialManager; }
        }

        public new ISocialEnvironment Environment
        {
            get { return base.Environment as ISocialEnvironment; }
            set { base.Environment = value; }
        }

        public uint AgentIdx { get; set; }

        public void ExecuteAction()
        {
            //executes chosen action and updates epsilon
            this.BehaviorManager.ExecuteChosenAction();
            this.BehaviorManager.UpdateEpsilon();
        }

        public void ChooseNextAction()
        {
            if (this.LogWriter != null) this.LogWriter.WriteLine("");

            //update previous state
            this.ExtrinsicSTM.PreviousState =
                this.ShortTermMemory.PreviousState = this.ShortTermMemory.CurrentState;

            //choose next action to be executed based on state
            this.BehaviorManager.DeliberateNextAction();
        }

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

        public override void Update()
        {
            var stm = this.ShortTermMemory;

            //update perception (sense)
            this.PerceptionManager.Update();
            this.ExtrinsicSTM.CurrentState =
                stm.CurrentState = this.LongTermMemory.GetUpdatedCurrentState();
            stm.Update();
            this.ExtrinsicSTM.Update();

            if (stm.PreviousState == null) return;

            //update social environment perception
            this.SocialManager.Update();

            //update extrinsic reward, ltm and learning
            this.ExtrinsicSTM.CurrentReward.Value =
                this.MotivationManager.ExtrinsicReward.Value =
                    this.MotivationManager.GetExtrinsicReward(stm.PreviousState.ID, stm.CurrentAction.ID);
            this.ExtrinsicLTM.Update();
            this.ExtrinsicLearningManager.Update();
            this.ExtrinsicLTM.UpdateMinMaxValues();

            //update goal relevance
            this.StateRelevanceManager.Update();
        }

        public virtual void UpdateIntrinsicReward()
        {
            //update others intrinsic rewards
            var stm = this.ShortTermMemory;
            stm.CurrentReward.Value =
                this.MotivationManager.IntrinsicReward.Value =
                    this.MotivationManager.GetIntrinsicReward(
                        stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID);

            //update memories (memorize)
            this.LongTermMemory.Update();

            //update learning
            if (stm.PreviousState != null)
                this.LearningManager.Update();
        }

        public int CompareTo(ISocialAgent other)
        {
            return this.Fitness.Value.CompareTo(other.Fitness.Value);
        }

        #endregion

        protected override void CreateManagers()
        {
            base.CreateManagers();
            this.SocialManager = this.CreateSocialManager();
        }

        protected virtual GPSocialManager CreateSocialManager()
        {
            return new GPSocialManager(this);
        }

        protected override PerceptionManager CreatePerceptionManager()
        {
            return new FoodCyclesPerceptionManager(this);
        }

        protected override MotivationManager CreateMotivationManager()
        {
            var testsConfig = (ISocialGPTestsConfig) this.Scenario.TestsConfig;
            return new SocialGPMotivationManager(this, testsConfig.Constants);
        }

        protected override ShortTermMemory CreateShortTermMemory()
        {
            return new ShortTermMemory(this);
        }

        protected override void AddStatisticalQuantities()
        {
            base.AddStatisticalQuantities();
            foreach (var socialEncounter in this.SocialManager.SocialEncounters)
                this.StatisticsCollection.Add(
                    string.Format("SocialEncounters{0}", socialEncounter.Key.AgentIdx), socialEncounter.Value);
        }

        protected override void CreateActions()
        {
            base.CreateActions();

            //removes up and down actions
            this.Actions.Remove(MOVE_UP);
            this.Actions.Remove(MOVE_DOWN);
        }
    }
}