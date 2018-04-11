// ------------------------------------------
// <copyright file="SocialGPMotivationManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.Tests.MultiRewardFunctionGPOptimization
//    Last updated: 03/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.IMRL.EC.Domain;
using PS.Learning.Tests.MultiRewardFunctionGPOptimization.Domain.Agents;

namespace PS.Learning.Tests.MultiRewardFunctionGPOptimization.Domain.Managers
{
    public class SocialGPMotivationManager : ForagingGPMotivationManager
    {
        protected const int SOCIAL_ENCOUNTER_IDX = 0;
        protected const int OTHER_EXTRINSIC_RWD_IDX = 1;
        protected const int OTHER_ST_ACT_CNT_IDX = 2;
        protected const int OTHER_DIST_IDX = 3;
        protected const int OTHER_ST_VAL_IDX = 4;
        protected const int OTHER_ST_ACT_VAL_IDX = 5;
        protected const int OTHER_ST_ACT_PRED_ERR_IDX = 6;
        protected const int OTHER_TRANS_PROB = 7;

        public SocialGPMotivationManager(FoodSharingGPAgent agent, double[] constants)
            : base(agent, constants)
        {
        }

        protected override string[] VariablesNames
        {
            get { return GetVariableNames(this.Agent.Environment.Agents.Count - 1, this.Agent.AgentIdx); }
        }

        public new FoodSharingGPAgent Agent
        {
            get { return base.Agent as FoodSharingGPAgent; }
        }

        public static string[] GetVariableNames(int numAgents, uint agentIdx)
        {
            var numOtherVars = VariableNames.Length;
            var variableNames = new string[numOtherVars + (numAgents*8)];

            //copies from above
            VariableNames.CopyTo(variableNames, 0);

            for (var i = 0u; i < numAgents; i++)
            {
                //ignores self agent
                if (i.Equals(agentIdx)) continue;
                var idxDiff = numOtherVars + (i*numOtherVars);

                variableNames[SOCIAL_ENCOUNTER_IDX + idxDiff] = string.Format("S{0}", i);
                variableNames[OTHER_EXTRINSIC_RWD_IDX + idxDiff] = string.Format("Re{0}", i);
                variableNames[OTHER_ST_ACT_CNT_IDX + idxDiff] = string.Format("Csa{0}", i);
                variableNames[OTHER_DIST_IDX + idxDiff] = string.Format("D{0}", i);
                variableNames[OTHER_ST_VAL_IDX + idxDiff] = string.Format("Vs{0}", i);
                variableNames[OTHER_ST_ACT_VAL_IDX + idxDiff] = string.Format("Qsa{0}", i);
                variableNames[OTHER_ST_ACT_PRED_ERR_IDX + idxDiff] = string.Format("Esa{0}", i);
                variableNames[OTHER_TRANS_PROB + idxDiff] = string.Format("Pssa{0}", i);
            }
            return variableNames;
        }

        public override DenseVector GetRewardFeatures(uint prevState, uint action, uint nextState)
        {
            var features = base.GetRewardFeatures(prevState, action, nextState);

            var i = 0;
            foreach (FoodSharingGPAgent otherAgent in this.Agent.Environment.Agents)
            {
                //ignores self agent
                if (otherAgent.Equals(this.Agent)) continue;

                var idxDiff = (i++*VariableNames.Length);
                var seeOtherAgent = this.Agent.SocialManager.SeeOtherAgents.Contains(otherAgent);

                //updates social encounters num
                features[SOCIAL_ENCOUNTER_IDX + idxDiff] = this.Agent.SocialManager.SocialEncounters[otherAgent].Sum;

                //gets all other variables directly from the other agent if currently in the same position
                features[OTHER_EXTRINSIC_RWD_IDX + idxDiff] =
                    seeOtherAgent ? otherAgent.MotivationManager.GetExtrinsicReward(prevState, action) : 0;
                features[OTHER_ST_ACT_CNT_IDX + idxDiff] =
                    seeOtherAgent ? otherAgent.MotivationManager.GetStateActionCount(prevState, action) : 0;
                features[OTHER_DIST_IDX + idxDiff] =
                    seeOtherAgent ? otherAgent.StateRelevanceManager.GetDistanceToGoal(prevState) : 0;
                features[OTHER_ST_VAL_IDX + idxDiff] =
                    seeOtherAgent ? otherAgent.MotivationManager.GetStateValue(prevState) : 0;
                features[OTHER_ST_ACT_VAL_IDX + idxDiff] =
                    seeOtherAgent ? otherAgent.MotivationManager.GetStateActionValue(prevState, action) : 0;
                features[OTHER_ST_ACT_PRED_ERR_IDX + idxDiff] =
                    seeOtherAgent ? otherAgent.MotivationManager.GetStateActionPredError(prevState, action) : 0;
                features[OTHER_TRANS_PROB + idxDiff] =
                    seeOtherAgent
                        ? otherAgent.MotivationManager.GetStateTransitionProb(prevState, action, nextState)
                        : 0;
            }

            return features;
        }
    }
}