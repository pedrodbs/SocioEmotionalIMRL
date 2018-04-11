// ------------------------------------------
// <copyright file="PGRDManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL

//    Last updated: 7/11/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.IMRL.Domain.Agents;

namespace PS.Learning.IMRL.Domain.Managers.Reward
{
    public class PGRDManager : RewardParametersManager
    {
        protected Dictionary<uint, Dictionary<uint, Dictionary<uint, Dictionary<uint, DenseVector>>>> gradientsCache =
            new Dictionary<uint, Dictionary<uint, Dictionary<uint, Dictionary<uint, DenseVector>>>>();

        protected DenseVector rwdParamsGradient;

        public PGRDManager(INORCAgent agent, uint numParams) : base(agent, numParams)
        {
            this.rwdParamsGradient = new DenseVector((int) numParams);
        }

        public double DecayAvgParam { get; set; }

        public double RegularizationParam { get; set; }

        public uint PlanningDepth { get; set; }

        protected double LearningRate
        {
            get { return 0.00005; } // this.Agent.LearningManager.LearningRate.Value; }
        }

        protected double Temperature
        {
            get { return 100; } // this.Agent.BehaviorManager.Epsilon.Value; }
        }

        protected double Discount
        {
            get { return 0.95; } // return this.Agent.LearningManager.Discount.Value; }
        }

        protected double ExtrinsicReward
        {
            get { return this.extrinsicReward.Mean; }
        }

        protected uint NumActions
        {
            get { return this.Agent.LongTermMemory.NumActions; }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.gradientsCache.Clear();
            this.gradientsCache = null;
            this.rwdParamsGradient.Clear();
            this.rwdParamsGradient = null;
        }

        public override void PrintResults(string path)
        {
        }

        protected override void ResetVariables()
        {
            base.ResetVariables();
            this.gradientsCache.Clear();
        }

        protected override DenseVector GetUpdatedRewardParameters(uint prevStateID, uint actionID, uint nextStateID)
        {
            //gets gradient
            //zt+1 = β.zt + (∇θt μ(at|it;Qt)/ μ(at|it;Qt))
            var actionProb = this.GetActionProb(prevStateID, actionID);
            var updatedRwdParamsGradient = this.GetUpdatedRwdParamsGradient(prevStateID, actionID, nextStateID);
            if (updatedRwdParamsGradient.Contains(double.NaN))
            {
                int i = 0;
                i++;
            }
            this.rwdParamsGradient =
                (this.DecayAvgParam*this.rwdParamsGradient) +
                (actionProb.Equals(0) ? new DenseVector((int) this.NumParams) : (updatedRwdParamsGradient/actionProb));
            if (this.rwdParamsGradient.Contains(double.NaN))
            {
                int i = 0;
                i++;
            }


            //updates params
            //θt+1 = θt + αt(rt+1.zt+1 - λθt)
            return this.RewardParameters +
                   (this.LearningRate*((this.ExtrinsicReward*this.rwdParamsGradient) -
                                       (this.RegularizationParam*this.RewardParameters)));
        }

        protected virtual DenseVector GetUpdatedRwdParamsGradient(uint prevStateID, uint actionID, uint nextStateID)
        {
            //updates gradient of policy wrt params

            //gets [∇θt Qt(a|it;θt) - ∑ ∇θt Qt(b|it;θt)]
            var sumActionQGradient = new DenseVector(this.RewardParameters.Count);
            var otherActions = this.GetActionsExcept(actionID);
            foreach (var otherAction in otherActions)
                sumActionQGradient += this.GetQGradient(prevStateID, otherAction, nextStateID, this.PlanningDepth);

            if (sumActionQGradient.Contains(double.NaN))
            {
                int i = 0;
                i++;
            }

            //∇θt μ(at|it;Qt) = τ·μ(a|Qt) [∇θt Qt(a|it;θt) - ∑ ∇θt Qt(b|it;θt)]
            var gradient = this.Temperature*this.GetActionProb(prevStateID, actionID)*(- sumActionQGradient);
            if (gradient.Contains(double.NaN))
            {
                int i = 0;
                i++;
            }
            return gradient;
        }

        protected virtual DenseVector GetQGradient(uint prevStateID, uint actionID, uint nextStateID, uint curPlanDepth)
        {
            //verifies depth
            var rewardFeatures = this.GetRewardFeatures(prevStateID, actionID, nextStateID);
            if (rewardFeatures.Any(double.IsInfinity) || rewardFeatures.Contains(double.NaN))
            {
                int i = 0;
                i++;
            }
            if (curPlanDepth == 0)
                return rewardFeatures;

            //verifies cache
            var qGradientCache = this.GetQGradientCache(prevStateID, actionID, nextStateID, curPlanDepth);
            if (qGradientCache != null)
                return qGradientCache;

            //updates gradient of Q wrt params (recursive forward)

            //gets ∑ T(o'|o,a) ∑ π(b|o').∇θ Q(o',b;θ)
            var otherStates = this.Agent.LongTermMemory.GetAllStateActionTransitions(prevStateID, actionID);
            var sumOthersQGradient = new DenseVector(this.RewardParameters.Count);
            foreach (var otherState in otherStates)
                for (var otherAction = 0u; otherAction < this.NumActions; otherAction++)
                {
                    var nextState = this.GetRandomNextState(otherState, otherAction);
                    if (nextState.Equals(uint.MaxValue)) continue;
                    sumOthersQGradient +=
                        this.GetStateTransitionProb(prevStateID, actionID, otherState)*
                        this.GetActionProbability(otherState, otherAction)*
                        this.GetQGradient(otherState, otherAction, nextState, curPlanDepth - 1);
                    //this.NormalizeParams(sumOthersQGradient);
                }

            if (sumOthersQGradient.Any(double.IsInfinity))
            {
                int i = 0;
                i++;
            }

            //∇θ Q(o,a;θ) = ∇θ R(o,a;θ) + γ ∑ T(o'|o,a) ∑ π(b|o').∇θ Q(o',b;θ)
            var qGradient = rewardFeatures +
                            (this.Discount*sumOthersQGradient);
            //this.NormalizeParams(qGradient);

            if (qGradient.Any(double.IsInfinity) || qGradient.Contains(double.NaN))
            {
                int i = 0;
                i++;
            }

            //sets cache
            this.SetGradientCache(prevStateID, actionID, nextStateID, curPlanDepth, qGradient);
            return qGradient;
        }

        protected void NormalizeParams(DenseVector vector)
        {
            var sum = vector.Select(System.Math.Abs).Sum();
            if (!double.IsInfinity(sum) && !sum.Equals(0))
                for (var i = 0; i < vector.Count; i++)
                    vector[i] = vector[i]/sum;
            else
            {
                int i = 0;
                i++;
            }
        }

        protected double GetActionProbability(uint otherState, uint otherAction)
        {
            return this.Agent.BehaviorManager.GetActionProbability(otherState, otherAction);
        }

        protected IEnumerable<uint> GetActionsExcept(uint except)
        {
            var actionList = new List<IAction>(this.Agent.Actions.Values);
            actionList.RemoveAt((int) except);
            return actionList.Select(action => action.ID);
        }

        protected double GetStateTransitionProb(uint prevStateID, uint actionID, uint nextStateID)
        {
            return this.Agent.LongTermMemory.GetStateActionTransitionProbability(prevStateID, actionID, nextStateID);
        }

        protected uint GetRandomNextState(uint prevStateID, uint actionID)
        {
            return this.Agent.LongTermMemory.GetRandomStateActionTransition(prevStateID, actionID);
        }

        protected DenseVector GetRewardFeatures(uint prevStateID, uint actionID, uint nextStateID)
        {
            return this.Agent.MotivationManager.GetRewardFeatures(prevStateID, actionID, nextStateID);
        }

        protected double GetActionProb(uint stateID, uint actionID)
        {
            return this.Agent.BehaviorManager.GetActionProbability(stateID, actionID);
        }

        protected void SetGradientCache(
            uint prevStateID, uint actionID, uint nextStateID, uint depth, DenseVector gradient)
        {
            //verifies dictionaries
            if (!this.gradientsCache.ContainsKey(prevStateID))
                this.gradientsCache[prevStateID] =
                    new Dictionary<uint, Dictionary<uint, Dictionary<uint, DenseVector>>>();
            if (!this.gradientsCache[prevStateID].ContainsKey(actionID))
                this.gradientsCache[prevStateID][actionID] = new Dictionary<uint, Dictionary<uint, DenseVector>>();
            if (!this.gradientsCache[prevStateID][actionID].ContainsKey(nextStateID))
                this.gradientsCache[prevStateID][actionID][nextStateID] = new Dictionary<uint, DenseVector>();

            //adds gradient to cache
            this.gradientsCache[prevStateID][actionID][nextStateID][depth] = gradient;
        }

        protected DenseVector GetQGradientCache(uint prevStateID, uint actionID, uint nextStateID, uint depth)
        {
            //verifies dictionaries
            if (this.gradientsCache.ContainsKey(prevStateID) &&
                this.gradientsCache[prevStateID].ContainsKey(actionID) &&
                this.gradientsCache[prevStateID][actionID].ContainsKey(nextStateID) &&
                this.gradientsCache[prevStateID][actionID][nextStateID].ContainsKey(depth))
                return this.gradientsCache[prevStateID][actionID][nextStateID][depth];

            //return null if cache not available
            return null;
        }
    }
}