// ------------------------------------------
// EchoSocialManager.cs, Learning.Tests.AltruismGPOptimization
//
// Created by Pedro Sequeira, 2014/2/20
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System.Collections.Generic;
using System.Linq;
using PS.Utilities.Math;
using PS.Learning.Core.Domain.Managers;
using PS.Learning.Core.Domain.States;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Managers;
using PS.Learning.Tests.AltruismGPOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismGPOptimization.Domain.Managers
{
    public class EchoSocialManager : Manager, ISocialManager
    {
        protected readonly Dictionary<ISocialAgent, uint> encounters = new Dictionary<ISocialAgent, uint>();
        protected readonly Dictionary<ISocialAgent, double> otherFitness = new Dictionary<ISocialAgent, double>();
        protected ulong lastTimeStep;

        public EchoSocialManager(EchoSocialAgent agent) : base(agent)
        {
            this.OtherHungerSignal = new StatisticalQuantity();
            this.OtherPresenceSignal = new StatisticalQuantity();
            this.OtherWorseSignal = new StatisticalQuantity();
            this.IntEchoSignal = new StatisticalQuantity();
            this.ExtEchoSignal = new StatisticalQuantity();
            this.IntPerfSignal = new StatisticalQuantity();
            this.CumulativeSocialEncounters = new StatisticalQuantity();
        }

        public List<IFoodSharingAgent> CurSeenAgents { get; set; }

        public StatisticalQuantity OtherHungerSignal { get; protected set; }
        public StatisticalQuantity OtherPresenceSignal { get; protected set; }
        public StatisticalQuantity OtherWorseSignal { get; protected set; }
        public StatisticalQuantity IntEchoSignal { get; protected set; }
        public StatisticalQuantity ExtEchoSignal { get; protected set; }
        public StatisticalQuantity IntPerfSignal { get; protected set; }
        public StatisticalQuantity CumulativeSocialEncounters { get; protected set; }

        #region ISocialManager Members

        public new ISocialAgent Agent
        {
            get { return base.Agent as ISocialAgent; }
        }

        public override void Update()
        {
            //updates last time step ate food
            if (this.Agent.MotivationManager.ExtrinsicReward.Value.Equals(1d))
                this.lastTimeStep = this.Agent.LongTermMemory.TimeStep;

            //updates social encounters
            this.UpdateEncounters();

            //update agent's social signals
            var stm = this.Agent.ShortTermMemory;
            var prevState = stm.PreviousState.ID;
            var action = stm.CurrentAction.ID;
            var nextState = stm.CurrentState.ID;
            this.OtherHungerSignal.Value = this.GetOtherHungerSignal(prevState, action, nextState);
            this.OtherPresenceSignal.Value = this.GetOtherPresSignal(prevState, action, nextState);
            this.OtherWorseSignal.Value = this.GetOtherWorseSig(prevState, action, nextState);
            this.IntEchoSignal.Value = this.GetIntEchoSignal(prevState, action, nextState);
            this.IntPerfSignal.Value = this.GetIntPerfSignal(prevState, action, nextState);
        }

        public override void Reset()
        {
            this.OtherHungerSignal.Reset();
            this.OtherPresenceSignal.Reset();
            this.OtherWorseSignal.Reset();
            this.IntEchoSignal.Reset();
            this.ExtEchoSignal.Reset();
            this.IntPerfSignal.Reset();
            this.CumulativeSocialEncounters.Reset();
        }

        public override void Dispose()
        {
            this.OtherHungerSignal.Dispose();
            this.OtherPresenceSignal.Dispose();
            this.OtherWorseSignal.Dispose();
            this.IntEchoSignal.Dispose();
            this.ExtEchoSignal.Dispose();
            this.IntPerfSignal.Dispose();
            this.CumulativeSocialEncounters.Dispose();
        }

        #endregion

        public override void PrintResults(string path)
        {
        }

        protected void UpdateEncounters()
        {
            if (this.CurSeenAgents == null) return;

            //updates the number of encounters with other agents
            foreach (var otherAgent in this.CurSeenAgents)
            {
                if (!this.encounters.ContainsKey(otherAgent))
                    this.encounters[otherAgent] = 0;

                this.encounters[otherAgent]++;
                this.otherFitness[otherAgent] = otherAgent.Fitness.Value;
            }

            //updates total encounter counter
            this.CumulativeSocialEncounters.Value += this.CurSeenAgents.Count > 0 ? 1 : 0;
        }

        protected bool SeeOtherAgents(IState prevState)
        {
            return !((IStimuliState) prevState).Sensations.Contains(OthersEchoPerceptionManager.DO_NOT_SEE_OTHER);
        }

        protected bool SeeHungryAgents(IState prevState)
        {
            return ((IStimuliState) prevState).Sensations.Contains(OthersEchoPerceptionManager.OTHERS_HUNGRY_SENSATION);
        }

        protected bool SeeWorseAgents(IState prevState)
        {
            return ((IStimuliState) prevState).Sensations.Contains(OthersEchoPerceptionManager.OTHERS_WORSE_SENSATION);
        }

        protected bool SeeEcho(IState prevState)
        {
            return ((IStimuliState) prevState).Sensations.Contains(OthersEchoPerceptionManager.OTHERS_ECHO_SENSATION);
        }

        public double GetOtherPresSignal(uint prevStateID, uint actionID, uint nextStateID)
        {
            //pres = n_visible / (n_ag-1)
            return (this.CurSeenAgents == null) || (this.CurSeenAgents.Count == 0)
                ? 0
                : this.CurSeenAgents.Average(otherAgent => this.encounters[otherAgent]);
        }

        public double GetOtherHungerSignal(uint prevStateID, uint actionID, uint nextStateID)
        {
            //hung =(n_satisfied - n_hungry) / (n_ag-1)
            var prevState = this.Agent.LongTermMemory.GetState(prevStateID);
            return !this.SeeOtherAgents(prevState) ? 0 : this.SeeHungryAgents(prevState) ? -1 : 1;
        }

        public double GetOtherWorseSig(uint prevStateID, uint actionID, uint nextStateID)
        {
            //ext =(n_better - n_worse) / (n_ag-1)
            //var prevState = this.Agent.LongTermMemory.GetState(prevStateID);
            //return !this.SeeOtherAgents(prevState) ? 0 : this.SeeWorseAgents(prevState) ? -1 : 1;
            //return this.CurSeenAgents.Count == 0
            //    ? 0
            //    : this.Agent.Fitness.Value - this.CurSeenAgents.Average(otherAgent => otherAgent.Fitness.Value);
            return this.otherFitness.Count == 0
                ? 0
                : Comparer<double>.Default.Compare(this.otherFitness.Values.Average(), this.Agent.Fitness.Value);
        }

        public double GetIntEchoSignal(uint prevStateID, uint actionID, uint nextStateID)
        {
            var action = this.Agent.LongTermMemory.GetAction(actionID);
            return action.IdToken.Equals(EchoSocialAgent.ECHO_ACTION_ID) ? 1 : 0;
        }

        public double GetExtEchoSignal(uint prevStateID, uint actionID, uint nextStateID)
        {
            //echo =(n_echo - n_noEcho) / (n_ag-1)
            var prevState = this.Agent.LongTermMemory.GetState(prevStateID);
            return !this.SeeOtherAgents(prevState) ? 0 : this.SeeEcho(prevState) ? 1 : 0;
        }

        public double GetIntPerfSignal(uint prevStateID, uint actionID, uint nextStateID)
        {
            //perf = 1 / (1 + t_lastAte)
            var timePassed = this.Agent.LongTermMemory.TimeStep - this.lastTimeStep;
            //return (1d/(1 + timePassed));
            return timePassed;
        }

        //protected virtual HashSet<IFoodSharingAgent> GetVisibleAgents(uint prevStateID, uint actionID)
        //{
        //    var agents = this.Agent.Environment.Agents;
        //    var state = this.Agent.LongTermMemory.GetState(prevStateID) as IStimuliState;

        //    var seeOtherAgents = new HashSet<IFoodSharingAgent>();
        //    if (prevStateID.Equals(uint.MaxValue) || (state == null))
        //        return seeOtherAgents;

        //    foreach (var socialAgent in agents)
        //    {
        //        var otherAgent = (IFoodSharingAgent) socialAgent;
        //        if ((otherAgent != this.Agent) && state.Sensations.Contains(otherAgent.IdToken))
        //            seeOtherAgents.Add(otherAgent);
        //    }

        //    return seeOtherAgents;
        //}
    }
}