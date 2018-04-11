// ------------------------------------------
// FoodSharingSocialManager.cs, Learning.Tests.AltruismOptimization
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
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public abstract class FoodSharingSocialManager : AltruismSocialManager
    {
        protected FoodSharingSocialManager(IFoodSharingAgent agent) : base(agent)
        {
        }

        public new IFoodSharingAgent Agent
        {
            get { return base.Agent as IFoodSharingAgent; }
        }


        protected override void UpdateEncounters()
        {
            //updates the number of encounters with other agents

            //task counter is reset when agent finishes task
            var stm = this.Agent.ShortTermMemory;
            var socialObs = this.GetSocialObservations(stm.PreviousState.ID, stm.CurrentAction.ID);

            //updates total encounter counter
            this.CumulativeSocialEncounters.Value += socialObs.seeOtherAgents.Count > 0 ? 1 : 0;
        }

        protected virtual SocialObservations GetSocialObservations(uint prevStateID, uint actionID)
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
                if ((otherAgent != this.Agent) && state.Sensations.Contains(otherAgent.IdToken))
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

        #region Nested type: SocialObservations

        protected struct SocialObservations
        {
            public bool isAgentActionEat;
            public bool isOtherAgentActionEat;
            public bool seeFood;
            public HashSet<IFoodSharingAgent> seeOtherAgents;
        }

        #endregion
    }
}