// ------------------------------------------
// OthersHungerPerceptionManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/16
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System.Collections.Generic;
using PS.Learning.Social.Domain;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public class OthersHungerPerceptionManager : AltruismPerceptionManager
    {
       public OthersHungerPerceptionManager(IFoodSharingAgent agent)
            : base(agent)
        {
        }

        protected override void AddInternalSensations()
        {
            //adds satiation status
            this.CurrentSensations.Add(this.Agent.MotivationManager.Hunger.Value == 1
                ? HUNGRY_SENSATION
                : SATISFIED_SENSATION);

            base.AddInternalSensations();
        }

        protected override void AddExternalSensations()
        {
            base.AddExternalSensations();

            //detects other agents in corridors
            var env = this.Agent.Environment;
            var seeOtherAgents = new List<ISocialAgent>();
            foreach (var otherAgent in env.Agents)
                if (!otherAgent.Equals(this.Agent) &&
                    env.DetectInCorridors(this.Agent.Cell.XCoord, this.Agent.Cell.YCoord, otherAgent.IdToken))
                    seeOtherAgents.Add(otherAgent);


            //verifies other agents
            if (seeOtherAgents.Count == 0) return;

            //adds others id and hunger status to sensations
            foreach (IFoodSharingAgent otherAgent in seeOtherAgents)
            {
                var hungerStatus = otherAgent.MotivationManager.Hunger.Value == 1
                    ? HUNGRY_SENSATION
                    : SATISFIED_SENSATION;

                this.CurrentSensations.Add(string.Format("{0}-{1}", otherAgent.IdToken, hungerStatus));
            }
        }
    }
}