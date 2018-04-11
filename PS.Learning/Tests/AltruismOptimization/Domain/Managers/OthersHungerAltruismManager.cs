// ------------------------------------------
// OthersHungerAltruismManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/16
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System;
using System.Collections.Generic;
using PS.Learning.Core.Domain.States;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    /// <summary>
    ///     Based on the fact that other agents are hungry (external -1,1) and self assessment according to given threshold
    /// </summary>
    public class OthersHungerAltruismManager : FoodSharingSocialManager
    {
        protected const uint MAX_TIME_STEPS = 500;

        protected readonly Dictionary<IFoodSharingAgent, double> othersFitness =
            new Dictionary<IFoodSharingAgent, double>();

        protected double lastFitness;
        protected ulong lastTimeStep;
        protected uint numTimeSteps;

        public OthersHungerAltruismManager(IFoodSharingAgent agent) : base(agent)
        {
        }

        protected virtual double MaxFitThreshold
        {
            get { return (0.5*(this.Agent.LongTermMemory.TimeStep - this.lastTimeStep))/11d; } // 7d; }
        }

        public override void Update()
        {
            if (this.Agent.MotivationManager.ExtrinsicReward.Value.Equals(1d))
                this.lastTimeStep = this.Agent.LongTermMemory.TimeStep;

            var socialObs = this.GetSocialObservations(
                this.Agent.ShortTermMemory.PreviousState.ID, this.Agent.ShortTermMemory.CurrentAction.ID);

            foreach (var otherAgent in socialObs.allVisibleAgents)
                this.othersFitness[otherAgent] = otherAgent.Fitness.Value;

            base.Update();
        }

        public override double GetExternalLegitimacySignal(uint prevState, uint action, uint nextState)
        {
            //ext = -(n_hungry / (n_ag-1)) + (n_satisfied / (n_ag-1))
            var socialObs = this.GetSocialObservations(prevState, action);
            var numHungry = socialObs.hungryAgents.Count;
            var numSatisfied = socialObs.satisfiedAgents.Count;
            var numVisible = socialObs.allVisibleAgents.Count;
            var numAgents = this.Agent.Environment.Agents.Count - 1d;

            return (numSatisfied - numHungry)/numAgents;
            //return (numVisible == 0) || (this.othersFitness.Count == 0)
            //           ? 0
            //           : this.Agent.Fitness.Value > this.othersFitness.Values.Average() ? 1 : -1;
        }

        public override double GetInternalLegitimacySignal(uint prevState, uint action, uint nextState)
        {
            //int = F / F_max
            //var maxFitThreshold = this.MaxFitThreshold;
            var timePassed = this.Agent.LongTermMemory.TimeStep - this.lastTimeStep;
            var maxFitThreshold = timePassed; //11d;
            return (1d/(1 + maxFitThreshold));
            //return (this.Agent.Fitness.Value-this.lastFitness) >= maxFitThreshold ? 1 : -1;
        }

        public override double GetSocialSignal(uint prevState, uint action, uint nextState)
        {
            return 0;
        }

        protected new virtual HungerSocialObservations GetSocialObservations(uint prevStateID, uint actionID)
        {
            var socialObs = new HungerSocialObservations();
            var agents = this.Agent.Environment.Agents;
            var state = this.Agent.LongTermMemory.GetState(prevStateID) as IStimuliState;

            if (prevStateID.Equals(UInt32.MaxValue) || (state == null))
                return socialObs;

            //adds agents to hungry/satisfied lists according to sensations in state
            socialObs.hungryAgents = new HashSet<IFoodSharingAgent>();
            socialObs.satisfiedAgents = new HashSet<IFoodSharingAgent>();
            socialObs.allVisibleAgents = new HashSet<IFoodSharingAgent>();
            foreach (IFoodSharingAgent otherAgent in agents)
                if (!otherAgent.Equals(this.Agent))
                    foreach (var sensation in state.Sensations)
                        if (!string.IsNullOrEmpty(sensation) && sensation.Contains(otherAgent.IdToken))
                        {
                            socialObs.allVisibleAgents.Add(otherAgent);
                            if (sensation.Contains(OthersHungerPerceptionManager.HUNGRY_SENSATION))
                                socialObs.hungryAgents.Add(otherAgent);
                            else socialObs.satisfiedAgents.Add(otherAgent);
                        }

            return socialObs;
        }

        #region Nested type: HungerSocialObservations

        protected struct HungerSocialObservations
        {
            public HashSet<IFoodSharingAgent> allVisibleAgents;
            public HashSet<IFoodSharingAgent> hungryAgents;
            public HashSet<IFoodSharingAgent> satisfiedAgents;
        }

        #endregion
    }
}