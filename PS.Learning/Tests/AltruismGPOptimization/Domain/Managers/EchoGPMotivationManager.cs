// ------------------------------------------
// <copyright file="EchoGPMotivationManager.cs" company="Pedro Sequeira">
// 
//     Copyright (c) 2018 Pedro Sequeira
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// </copyright>
// <summary>
//    Project: Learning.Tests.AltruismGPOptimization
//    Last updated: 02/19/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.Core.Domain;
using PS.Learning.Core.Domain.States;
using PS.Learning.IMRL.Domain;
using PS.Learning.IMRL.Domain.Managers.Motivation;
using PS.Learning.IMRL.EC.Domain;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;

namespace PS.Learning.Tests.AltruismGPOptimization.Domain.Managers
{
    public class EchoGPMotivationManager : GPMotivationManager, IHungerMotivationManager
    {
        protected const int EXTR_RWD_IDX = 0;
        protected const int OTH_HUNG_IDX = 1;
        protected const int FIT_DIF = 2;
        protected const int NUM_ENC_IDX = 3;
        protected const int TIME_LAST_EAT_IDX = 4;
        protected const int ST_CNT_IDX = 5;
        protected const int TRANS_PROB_IDX = 6;
        protected const int OBJ_IDX = 7;
        public static readonly string[] VariableNames = {"Re", "Hng", "Fdif", "Ce", "Tle", "Cs", "Pssa", "Obj"};

        public EchoGPMotivationManager(IGPAgent agent, double[] constants) : base(agent, constants)
        {
            //agent starts hungry
            this.Hunger = new Need("Hunger", 1, 0, 0, 0) {Value = 1};
        }

        protected override string[] VariablesNames
        {
            get { return VariableNames; }
        }

        public new IFoodSharingAgent Agent
        {
            get { return base.Agent as IFoodSharingAgent; }
        }

        #region IHungerMotivationManager Members

        public Need Hunger { get; private set; }

        #endregion

        public override DenseVector GetRewardFeatures(uint prevState, uint action, uint nextState)
        {
            var altruismManager = (EchoSocialManager) this.Agent.SocialManager;
            var featureVector = new DenseVector((int) this.NumInputElements);
            this.CopyConstants(featureVector);

            featureVector[EXTR_RWD_IDX] = this.GetExtrinsicReward(prevState, action);
            featureVector[OTH_HUNG_IDX] = altruismManager.GetOtherHungerSignal(prevState, action, nextState);
            featureVector[FIT_DIF] = altruismManager.GetOtherWorseSig(prevState, action, nextState);
            featureVector[NUM_ENC_IDX] = altruismManager.GetOtherPresSignal(prevState, action, nextState);
            //featureVector[EXT_ECHO_IDX] = altruismManager.GetExtEchoSignal(prevState, action, nextState);
            //featureVector[INT_ECHO_IDX] = altruismManager.GetIntEchoSignal(prevState, action, nextState);
            //featureVector[EXT_ECHO_IDX] = this.GetExternalLegitimacySignal(prevState, action, nextState);
            //featureVector[INT_ECHO_IDX] = this.GetInternalLegitimacySignal(prevState, action, nextState);
            featureVector[TIME_LAST_EAT_IDX] = altruismManager.GetIntPerfSignal(prevState, action, nextState);
            featureVector[ST_CNT_IDX] = this.GetStateCount(prevState);
            featureVector[TRANS_PROB_IDX] = this.GetStateTransitionProb(prevState, action, nextState);
            //featureVector[CUM_FIT_IDX] = this.Agent.Fitness.Value;
            featureVector[OBJ_IDX] = this.GetNumObjects(prevState, action, nextState);

            return featureVector;
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

        public double GetInternalLegitimacySignal(uint prevState, uint action, uint nextState)
        {
            var socialObs = this.GetSocialObservations(prevState, action);

            //internal signal punishes for selfish acts and rewards if the agents see food and do not eat
            return socialObs.seeFood ? this.GetSocialSignalValue(socialObs) : 0;
        }

        public double GetExternalLegitimacySignal(uint prevState, uint action, uint nextState)
        {
            var socialObs = this.GetSocialObservations(prevState, action);

            //external legitimacy signal is 1 if the agents was the last to eat food and does not ea
            // and sees food and the other agent decides to eat
            return socialObs.seeFood && socialObs.isOtherAgentActionEat
                ? this.GetSocialSignalValue(socialObs)
                : 0;
        }

        protected int GetSocialSignalValue(SocialObservations socialObs)
        {
            //signal is modulated by the number of cycles without eating
            var maxCyclesWithoutEating = this.Agent.Environment.Agents.Count - 1;
            var truncatedCyclesWithoutEating = Math.Min(maxCyclesWithoutEating,
                this.Agent.Environment.CyclesWithoutEating[this.Agent]);
            var value = (maxCyclesWithoutEating - truncatedCyclesWithoutEating)/maxCyclesWithoutEating;
            return (socialObs.isAgentActionEat ? -1 : 1)*value;
        }

        public double GetSocialSignal(uint prevState, uint action, uint nextState)
        {
            var socialObs = this.GetSocialObservations(prevState, action);
            return socialObs.seeOtherAgents.Count > 0 ? 1 : 0;
        }

        protected double GetNumObjects(uint prevState, uint action, uint nextState)
        {
            var stimState = this.Agent.LongTermMemory.GetState(prevState) as IStimuliState;
            if (stimState == null) return 0;

            return stimState.Sensations.Count(
                sensation => sensation.Contains(LeverEnvironment.LEVER_ID));
            // || sensation.Contains(FoodSharingEnvironment.FOOD_ID));
        }

        protected double GetStateCount(uint prevState)
        {
            //checks args
            if (prevState.Equals(uint.MaxValue)) return 0;

            //gets the number of times the agent previously visited the state s within current history
            var stateCount = this.Agent.LongTermMemory.GetStateCount(prevState);

            //returns reward
            return stateCount;
        }

        protected double GetStateTransitionProb(uint prevState, uint action, uint nextState)
        {
            //checks args
            if ((prevState.Equals(uint.MaxValue)) || (action.Equals(uint.MaxValue)) || (nextState.Equals(uint.MaxValue)))
                return 0;

            //return transition prob
            return this.Agent.LongTermMemory.GetStateActionTransitionProbability(
                prevState, action, nextState);
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