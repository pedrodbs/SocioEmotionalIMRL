// ------------------------------------------
// OthersFitnessPerceptionManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/3
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System.Collections.Generic;
using System.Linq;
using PS.Learning.Social.Domain;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public class OthersFitnessPerceptionManager : AltruismPerceptionManager
    {
        public OthersFitnessPerceptionManager(IFoodSharingAgent agent) : base(agent)
        {
        }

        protected override void AddInternalSensations()
        {
            //adds satiation status
            this.CurrentSensations.Add(this.Agent.MotivationManager.Hunger.Value == 1
                ? HUNGRY_SENSATION
                : SATISFIED_SENSATION);

            base.AddInternalSensations();

            //verifies other agents fitness statistics
            var otherAgents = ((OthersFitnessAltruismManager) this.Agent.SocialManager).OtherFitAvg.Values;
            if (otherAgents.Count == 0) return;

            //calculates others avg fitness and adds to sensations
            var groupFitAvg = otherAgents.Average();
            var agentFit = this.Agent.Fitness.Value;
            this.CurrentSensations.Add(string.Format("feel-{0}", agentFit > groupFitAvg ? "bad" : "good"));
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

            //adds others id to sensations
            var sensation = "";
            seeOtherAgents.ForEach(otherAgent => sensation = string.Format("{0}-{1}", sensation, otherAgent.IdToken));

            //calculates others avg fitness and adds to sensations
            var groupFitAvg = seeOtherAgents.Average(otherAgent => otherAgent.Fitness.Value);
            this.CurrentSensations.Add(string.Format("{0}-sent-{1}", sensation,
                this.Agent.Fitness.Value > groupFitAvg ? "bad" : "good"));
        }
    }
}