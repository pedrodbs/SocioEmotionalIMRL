// ------------------------------------------
// GPSocialManager.cs, Learning.Tests.MultiRewardFunctionGPOptimization
//
// Created by Pedro Sequeira, 2013/3/26
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System.Collections.Generic;
using System.Linq;
using PS.Utilities.Math;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Managers;
using PS.Learning.Core.Domain.States;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Managers;
using PS.Learning.Social.IMRL.EC.Domain;

namespace PS.Learning.Tests.MultiRewardFunctionGPOptimization.Domain.Managers
{
    public class GPSocialManager : Manager, ISocialManager
    {
        public GPSocialManager(ISocialGPAgent agent) : base(agent)
        {
            this.SeeOtherAgents = new HashSet<ISocialGPAgent>();
            this.SocialEncounters = new Dictionary<ISocialGPAgent, StatisticalQuantity>();

            foreach (ISocialGPAgent otherAgent in ((ISocialAgent)this.Agent).Environment.Agents)
                this.SocialEncounters[otherAgent] = new StatisticalQuantity();
        }

        public HashSet<ISocialGPAgent> SeeOtherAgents { get; private set; }

        public Dictionary<ISocialGPAgent, StatisticalQuantity> SocialEncounters { get; private set; }

        public new ISocialGPAgent Agent
        {
            get { return base.Agent as ISocialGPAgent; }
        }

        #region ISocialManager Members

        ISocialAgent ISocialManager.Agent
        {
            get { return this.Agent; }
        }

        IAgent IManager.Agent
        {
            get { return this.Agent; }
        }

        public override void Update()
        {
            //updates social observations
            this.UpdateSocialObservations();
        }


        public override void Reset()
        {
            foreach (var quantity in this.SocialEncounters.Values)
                quantity.Reset();

            this.SeeOtherAgents.Clear();
        }

        public override void Dispose()
        {
            this.SocialEncounters.Clear();
            this.SeeOtherAgents.Clear();
        }

        #endregion

        public override void PrintResults(string path)
        {
        }

        protected void UpdateSocialObservations()
        {
            var state = this.Agent.ShortTermMemory.PreviousState;
            var agents = ((ISocialAgent)this.Agent).Environment.Agents;

            if (state == null) return;

            //checks agents in the current state
            this.SeeOtherAgents.Clear();
            this.SeeOtherAgents.UnionWith(
                agents.Cast<ISocialGPAgent>().Where(
                    otherAgent =>
                    (otherAgent != this.Agent) && ((IStimuliState) state).Sensations.Contains(otherAgent.IdToken)));


            //updates social encounters
            foreach (var otherAgent in agents.Cast<ISocialGPAgent>().Where(otherAgent => !otherAgent.Equals(this.Agent))
                )
                this.SocialEncounters[otherAgent].Value = this.SeeOtherAgents.Contains(otherAgent) ? 1 : 0;
        }
    }
}