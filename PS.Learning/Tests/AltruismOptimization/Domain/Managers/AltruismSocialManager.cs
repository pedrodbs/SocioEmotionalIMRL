// ------------------------------------------
// AltruismSocialManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2014/1/20
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using PS.Utilities.Math;
using PS.Learning.Core.Domain.Managers;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Managers;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public abstract class AltruismSocialManager : Manager, ISocialManager
    {
        protected AltruismSocialManager(ISocialAgent agent) : base(agent)
        {
            this.ExternalLegitimacySignal = new StatisticalQuantity();
            this.InternalLegitimacySignal = new StatisticalQuantity();
            this.SocialSignal = new StatisticalQuantity();
            this.CumulativeSocialEncounters = new StatisticalQuantity();
        }

        public StatisticalQuantity ExternalLegitimacySignal { get; protected set; }
        public StatisticalQuantity InternalLegitimacySignal { get; protected set; }
        public StatisticalQuantity SocialSignal { get; protected set; }
        public StatisticalQuantity CumulativeSocialEncounters { get; protected set; }

        #region ISocialManager Members

        public new ISocialAgent Agent
        {
            get { return base.Agent as ISocialAgent; }
        }

        public override void Update()
        {
            //updates social encounters
            this.UpdateEncounters();

            //update agent's social signals
            var stm = this.Agent.ShortTermMemory;
            this.ExternalLegitimacySignal.Value = this.GetExternalLegitimacySignal(
                stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID);
            this.InternalLegitimacySignal.Value = this.GetInternalLegitimacySignal(
                stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID);
            this.SocialSignal.Value = this.GetSocialSignal(
                stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID);
        }

        public override void Reset()
        {
            this.ExternalLegitimacySignal.Reset();
            this.InternalLegitimacySignal.Reset();
            this.CumulativeSocialEncounters.Reset();
            this.SocialSignal.Reset();
        }

        public override void Dispose()
        {
            this.ExternalLegitimacySignal.Dispose();
            this.InternalLegitimacySignal.Dispose();
            this.SocialSignal.Dispose();
            this.CumulativeSocialEncounters.Dispose();
        }

        #endregion

        public override void PrintResults(string path)
        {
        }

        protected abstract void UpdateEncounters();
        public abstract double GetInternalLegitimacySignal(uint prevState, uint action, uint nextState);
        public abstract double GetExternalLegitimacySignal(uint prevState, uint action, uint nextState);
        public abstract double GetSocialSignal(uint prevState, uint action, uint nextState);
    }
}