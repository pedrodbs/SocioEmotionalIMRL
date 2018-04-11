// ------------------------------------------
// OthersFitnessAltruismManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/3
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using PS.Learning.Core.Domain.States;
using PS.Learning.IMRL.Domain;
using PS.Learning.Social.Domain;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public class OthersFitnessAltruismManager : FoodSharingSocialManager
    {
        public OthersFitnessAltruismManager(IFoodSharingAgent agent) : base(agent)
        {
            this.OtherFitAvg = new Dictionary<ISocialAgent, double>();
        }

        public Dictionary<ISocialAgent, double> OtherFitAvg { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            this.OtherFitAvg.Clear();
        }

        public override void Update()
        {
            this.UpdateFitnessEstimates();
            base.Update();
        }

        protected virtual void UpdateFitnessEstimates()
        {
            //updates estimates for other agents' fitness
            var prevState = this.Agent.ShortTermMemory.PreviousState.ID;
            var action = this.Agent.ShortTermMemory.CurrentAction.ID;
            var socialObs = this.GetSocialObservations(prevState, action);
            foreach (var otherAgent in socialObs.seeOtherAgents)
                this.OtherFitAvg[otherAgent] = otherAgent.Fitness.Value;
        }

        public override double GetExternalLegitimacySignal(uint prevState, uint action, uint nextState)
        {
            //ext = F_avg - F_other_avg
            var socialObs = this.GetSocialObservations(prevState, action);
            if (socialObs.seeOtherAgents.Count == 0)
                return 0;

            var seeOtherFitAvg = socialObs.seeOtherAgents.Average(otherAgent => this.OtherFitAvg[otherAgent]);
            var agentFitAvg = this.Agent.Fitness.Value;
            return seeOtherFitAvg < agentFitAvg ? -1 : 1; //(seeOtherFitAvg - agentFitAvg) : 0;
        }

        public override double GetInternalLegitimacySignal(uint prevState, uint action, uint nextState)
        {
            //int = F_avg - F_pop_avg
            var groupFitAvg = this.OtherFitAvg.Count == 0 ? 0 : this.OtherFitAvg.Values.Average();
            var agentFitAvg = this.Agent.Fitness.Value;
            //var state = this.Agent.LongTermMemory.GetState(prevState);
            //var stateCell = ((CellPerceptionManager)this.Agent.PerceptionManager).GetCellFromState(state);
            //var distanceToGoal = this.Agent.Environment.GetDistanceToTargetCell(stateCell);
            return groupFitAvg < agentFitAvg ? -1 : 1; //(1+distanceToGoal) : 1;//(groupFitAvg - agentFitAvg) : 0;
        }

        public override double GetSocialSignal(uint prevState, uint action, uint nextState)
        {
            return 0;
        }

        protected virtual double GetTimeTasks()
        {
            return this.Agent.LongTermMemory.TimeStep/7d;
        }

        protected override SocialObservations GetSocialObservations(uint prevStateID, uint actionID)
        {
            var socialObs = new SocialObservations();
            var agents = this.Agent.Environment.Agents;
            var foodSharingEnvironment = this.Agent.Environment;
            var state = this.Agent.LongTermMemory.GetState(prevStateID) as IStimuliState;
            var action = this.Agent.LongTermMemory.GetAction(actionID);

            if (prevStateID.Equals(UInt32.MaxValue) || (state == null))
                return socialObs;

            socialObs.seeOtherAgents = new HashSet<IFoodSharingAgent>();
            foreach (IFoodSharingAgent otherAgent in agents)
                if ((otherAgent != this.Agent) &&
                    state.Sensations.Any(sensation => sensation.Contains(otherAgent.IdToken)))
                    socialObs.seeOtherAgents.Add(otherAgent);

            socialObs.seeFood = foodSharingEnvironment.FoodResources.Any(
                foodResource => state.Sensations.Contains(foodResource.IdToken));

            socialObs.isAgentActionEat = foodSharingEnvironment.AutoEat || action is Eat;

            socialObs.isOtherAgentActionEat = socialObs.seeOtherAgents.Count > 0 &&
                                              (foodSharingEnvironment.AutoEat ||
                                               agents.Any(otherAgent =>
                                                          (otherAgent != this.Agent) &&
                                                          (otherAgent.ShortTermMemory.CurrentAction is Eat)));

            return socialObs;
        }
    }
}