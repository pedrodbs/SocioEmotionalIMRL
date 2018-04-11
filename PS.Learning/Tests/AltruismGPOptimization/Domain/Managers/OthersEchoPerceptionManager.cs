// ------------------------------------------
// OthersEchoPerceptionManager.cs, Learning.Tests.AltruismGPOptimization
//
// Created by Pedro Sequeira, 2014/2/20
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System.Collections.Generic;
using System.Linq;
using PS.Learning.Tests.AltruismGPOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Managers;

namespace PS.Learning.Tests.AltruismGPOptimization.Domain.Managers
{
    public class OthersEchoPerceptionManager : AltruismPerceptionManager
    {
        public const string NOT_HUNGRY_SENSATION = "sat";
        public const string OTHERS_NOT_HUNGRY_SENSATION = "oNhun";
        public const string OTHERS_HUNGRY_SENSATION = "oHun";
        public const string OTHERS_NOT_WORSE_SENSATION = "oNworse";
        public const string OTHERS_WORSE_SENSATION = "oWorse";
        public const string OTHERS_NOT_ECHO_SENSATION = "nEcho";
        public const string OTHERS_ECHO_SENSATION = "echo";
        public const string DO_NOT_SEE_OTHER = "no-one";

        public OthersEchoPerceptionManager(IFoodSharingAgent agent)
            : base(agent)
        {
        }

        protected override void AddInternalSensations()
        {
            //adds satiation status
            this.CurrentSensations.Add(this.Agent.MotivationManager.Hunger.Value.Equals(1u)
                ? HUNGRY_SENSATION
                : NOT_HUNGRY_SENSATION);

            base.AddInternalSensations();
        }

        //public override bool SeeOtherAgents(IState state)
        //{
        //    return !((IStimuliState) state).Sensations.Contains(DO_NOT_SEE_OTHER);
        //}

        protected override void AddExternalSensations()
        {
            //adds cell elements
            base.AddExternalSensations();

            //detects other agents in corridors
            var env = this.Agent.Environment;
            var seeOtherAgents = new List<IFoodSharingAgent>();
            foreach (var otherAgent in env.Agents)
            {
                if (!otherAgent.Equals(this.Agent) &&
                    env.DetectInCorridors(this.Agent.Cell.XCoord, this.Agent.Cell.YCoord, otherAgent.IdToken))
                    seeOtherAgents.Add((IFoodSharingAgent) otherAgent);
                //this.DetectInCorridor(otherAgent.IdToken, "see-other");
            }

            ((EchoSocialManager) this.Agent.SocialManager).CurSeenAgents = seeOtherAgents;

            //verifies other agents
            if (seeOtherAgents.Count == 0)
            {
                this.CurrentSensations.Add(DO_NOT_SEE_OTHER);
                return;
            }

            var numWorse = seeOtherAgents.Count(other => other.Fitness.Value <= this.Agent.Fitness.Value);
            var numHungry = seeOtherAgents.Count(other => other.MotivationManager.Hunger.Value.Equals(1));
            var numEcho = seeOtherAgents.Count(
                other => other.ShortTermMemory.CurrentAction.IdToken.Equals(EchoSocialAgent.ECHO_ACTION_ID));
            var halfAgents = (this.Agent.Environment.Agents.Count - 1d)*0.5;

            this.CurrentSensations.Add(numHungry >= halfAgents ? OTHERS_HUNGRY_SENSATION : OTHERS_NOT_HUNGRY_SENSATION);
            //this.CurrentSensations.Add(numWorse >= halfAgents ? OTHERS_WORSE_SENSATION : OTHERS_NOT_WORSE_SENSATION);
            //this.CurrentSensations.Add(numEcho >= halfAgents ? OTHERS_ECHO_SENSATION : OTHERS_NOT_ECHO_SENSATION);
        }
    }
}