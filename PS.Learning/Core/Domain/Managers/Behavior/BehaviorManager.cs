// ------------------------------------------
// <copyright file="BehaviorManager.cs" company="Pedro Sequeira">
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
//    Project: Learning
//    Last updated: 10/10/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Utilities.Math;

namespace PS.Learning.Core.Domain.Managers.Behavior
{
    [Serializable]
    public abstract class BehaviorManager : Manager
    {
        #region Fields

        [NonSerialized] protected static readonly WH2006 Rand = new WH2006(true);

        #endregion

        #region Constructors

        protected BehaviorManager(IAgent agent) : base(agent)
        {
            this.Controlable = false;
            this.ActionStatistics = new StatisticsCollection();
            this.ActionList = new List<IAction>(this.Agent.Actions.Values);
        }

        #endregion

        #region Properties

        public bool Controlable { get; set; }

        public IAction NextAction { get; set; }

        protected Dictionary<string, IAction> Actions
        {
            get { return this.Agent.Actions; }
        }

        public List<IAction> ActionList { get; private set; }

        public StatisticsCollection ActionStatistics { get; protected set; }

        public double LastReward { get; protected set; }

        #endregion

        #region Public Methods

        public virtual void Init()
        {
            //adds one quantity per action available
            foreach (var action in this.Actions)
                this.ActionStatistics.Add(action.Key, new StatisticalQuantity());
        }

        public override void Update()
        {
            //chooses and executes next action, updates epsilon
            this.DeliberateNextAction();
            this.ExecuteChosenAction();
        }

        public virtual void ExecuteChosenAction()
        {
            //executes previously chosen action
            this.LastReward = this.ExecuteNextAction();
        }

        public virtual void DeliberateNextAction()
        {
            //chooses next action to be executed
            var chosenAction = this.Agent.ShortTermMemory.CurrentAction = this.ChooseNextAction();
            if (chosenAction == null) return;

            //updates statistics about chosen actions
            foreach (var actionStatistic in this.ActionStatistics)
                actionStatistic.Value.Value = actionStatistic.Key.Equals(chosenAction.IdToken) ? 1 : 0;
        }

        public Policy GetPolicy(uint stateID)
        {
            var policy = this.GetStatePolicy(stateID);
            policy.Normalize();
            return policy;
        }

        protected abstract Policy GetStatePolicy(uint stateID);

        public double GetActionProbability(uint stateID, uint actionID)
        {
            return this.GetPolicy(stateID).GetActionProbability(actionID);
        }

        public virtual IAction SelectAction(uint stateID)
        {
            //get current policy and normalize
            var policy = this.GetPolicy(stateID);

            //choose an action randomly based on policy/distribution for state
            var prob = Rand.NextDouble();
            var sum = 0d;
            for (var i = 0; i < this.Actions.Count; i++)
            {
                sum += policy[(uint) i];
                if (sum >= prob) return this.ActionList[i];
            }

            //should never get here if policy is non-zero
            return this.SelectGreedyAction(stateID);
        }

        public IAction SelectRandomAction()
        {
            //picks random action from action set
            return this.ActionList[Rand.Next(this.Actions.Count)];
        }

        #endregion

        #region Protected Methods

        protected virtual double ExecuteNextAction()
        {
            //executes the action in memory (chosen to be executed)
            return this.Agent.ShortTermMemory.CurrentAction == null
                ? 0
                : this.Agent.ShortTermMemory.CurrentAction.Execute();
        }

        protected IAction ChooseNextAction()
        {
            //checks for null state
            var currentState = this.Agent.ShortTermMemory.CurrentState;
            if (currentState == null) return this.SelectRandomAction();

            //if agent is externally controlable, just return action, otherwise pick accord to policy
            return this.Controlable
                ? (this.NextAction ?? this.SelectRandomAction())
                : this.SelectAction(currentState.ID);
        }


        protected IAction SelectGreedyAction(uint stateID)
        {
            return this.ActionList[(int) this.Agent.LongTermMemory.GetMaxStateAction(stateID)];
        }

        protected IAction SelectExploratoryAction(uint stateID)
        {
            IAction minCountAction = null;
            var minCount = double.MaxValue;
            for (var i = 0u; i < this.ActionList.Count; i++)
                if ((this.Agent.LongTermMemory.GetStateActionCount(stateID, i) < minCount)
                    )
                {
                    minCount = this.Agent.LongTermMemory.GetStateActionCount(stateID, i);
                    minCountAction = this.ActionList[(int) i];
                }
            return minCountAction ?? this.SelectRandomAction();
        }

        #endregion
    }
}