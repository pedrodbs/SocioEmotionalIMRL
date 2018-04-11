// ------------------------------------------
// <copyright file="StochasticTransitionLTM.cs" company="Pedro Sequeira">
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
//    Last updated: 12/18/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.States;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;
using Action = PS.Learning.Core.Domain.Actions.Action;

namespace PS.Learning.Core.Domain.Memories
{
    [Serializable]
    public class StochasticTransitionLTM : LongTermMemory
    {
        #region Constructor

        public StochasticTransitionLTM(IAgent agent, ShortTermMemory shortTermMemory)
            : base(agent, shortTermMemory)
        {
        }

        #endregion

        #region Constants

        private const string OBSERVABLE_STATE_TAG = "observable-state";
        private const string NON_OBSERVABLE_STATE_TAG = "non-observable-state";
        private const string INITIAL_STATE_TAG = "initial-state";
        private const string FINAL_STATE_TAG = "final-state";
        private const string STATES_TAG = "states";
        private const string STATE_TAG = "state";
        private const string ACTIONS_TAG = "actions";
        private const string ACTION_TAG = "action";
        private const string REWARDS_TAG = "rewards";
        private const string REWARD_TAG = "reward";
        private const string TRANSITIONS_TAG = "transitions";
        private const string TRANSITION_TAG = "transition";
        private const string VALUE_TAG = "value";

        #endregion

        #region Fields

        protected PartiallyObservableState currentState;

        protected List<PartiallyObservableState.NonObservableState> nonObservableStates =
            new List<PartiallyObservableState.NonObservableState>();

        protected List<PartiallyObservableState.ObservableState> observableStates =
            new List<PartiallyObservableState.ObservableState>();

        protected List<PartiallyObservableState> poStates = new List<PartiallyObservableState>();

        protected Dictionary<PartiallyObservableState, Dictionary<IAction, double>> rewardTable =
            new Dictionary<PartiallyObservableState, Dictionary<IAction, double>>();

        protected
            Dictionary<PartiallyObservableState, Dictionary<IAction, Dictionary<PartiallyObservableState, double>>>
            stateTransitionProbabilities =
                new Dictionary
                    <PartiallyObservableState, Dictionary<IAction, Dictionary<PartiallyObservableState, double>>>();

        #endregion

        #region Properties

        public Dictionary<PartiallyObservableState, Dictionary<IAction, Dictionary<PartiallyObservableState, double>>>
            StateTransitionProbabilities
        {
            get { return this.stateTransitionProbabilities; }
        }

        protected new IStochasticTestsConfig TestsConfig
        {
            get { return base.TestsConfig as IStochasticTestsConfig; }
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            base.Dispose();
            this.rewardTable.Clear();
            this.poStates.Clear();
            this.stateTransitionProbabilities.Clear();
            this.observableStates.Clear();
            this.nonObservableStates.Clear();
            this.currentState = null;
        }

        public override void Reset()
        {
            base.Reset();

            //only reset current state, state transitions and reward table maintains
            this.currentState = null;
        }

        public virtual void GenerateRandomMemories()
        {
            //creates and fills state, action, reward and transition tables
            this.CreateStates();
            this.CreateActions();
            this.CreateRewardTable();
            this.CreateStateTransitions();
        }

        public virtual double GetCurrentStateActionReward()
        {
            //returns the current state-current action reward from the reward table if exists, 0 otherwise
            return this.GetStateActionReward(this.currentState, this.ShortTermMemory.CurrentAction);
        }

        public virtual double GetStateActionReward(PartiallyObservableState state, IAction action)
        {
            //returns the state-action reward from the reward table if exists, 0 otherwise
            return (state == null) || (action == null) ||
                   !this.rewardTable.ContainsKey(state) || !this.rewardTable[state].ContainsKey(action)
                ? 0
                : this.rewardTable[state][action];
        }

        public override string ToString(IState state)
        {
            return state.ToString();
        }

        public override IState GetUpdatedCurrentState()
        {
            //gets random next state according to transition table, previous state and chosen action
            var action = this.ShortTermMemory.CurrentAction;
            this.currentState = (this.currentState == null)
                ? this.GetRandomState()
                : this.GetRandomNextState(this.currentState, action);

            //only the observable part of the state remains "visible" to the system
            return this.currentState.ObservablePart;
        }

        public override IState FromString(string stateStr)
        {
            return null;
        }

        public override string ToString()
        {
            return
                $"{this.observableStates.Count}oStates-{this.nonObservableStates.Count}noStates-{this.Agent.Actions.Count}actions";
        }

        public override void InitMemory()
        {
            //base.InitMemory();

            //loads memory from XML file or creates random one
            var filePath = ((IStochasticScenario) this.Agent.Scenario).LtmXmlFilePath;
            if (filePath == null)
                this.GenerateRandomMemories();
            else
                this.LoadFromXmlFile(filePath);
        }

        #endregion

        #region Protected Methods

        protected virtual void CreateStates()
        {
            //creates observable states
            var numObservableStates = Rand.Next((int) this.TestsConfig.MinObservableStates,
                (int) this.TestsConfig.MaxObservableStates + 1);
            for (var i = 0; i < numObservableStates; i++)
                this.observableStates.Add(new PartiallyObservableState.ObservableState {IdToken = "observableState" + i});

            //creates non-observable states
            var numNonObservableStates = Rand.Next((int) this.TestsConfig.MaxNonObservableStates) + 1;
            for (var i = 0; i < numNonObservableStates; i++)
                this.nonObservableStates.Add(new PartiallyObservableState.NonObservableState
                                             {IdToken = "nonObservableState" + i});

            //creates partially-observable states from combinations between them
            foreach (var observableState in this.observableStates)
                foreach (var nonObservableState in this.nonObservableStates)
                    this.poStates.Add(new PartiallyObservableState(observableState, nonObservableState));
        }

        protected virtual void CreateActions()
        {
            //creates x numaber of simple actions
            var numActions = Rand.Next((int) this.TestsConfig.MinActions, (int) this.TestsConfig.MaxActions + 1);
            for (var i = 0; i < numActions; i++)
            {
                var actionID = "action" + i;
                this.Agent.Actions.Add(actionID, new Actions.Action(actionID, this.Agent));
            }
        }

        protected virtual void CreateRewardTable()
        {
            var numStateActionRewards = 0;
            var maxStateActionRewards = this.TestsConfig.StateActionRewardPercent*
                                        this.poStates.Count*this.Agent.Actions.Count;

            //creates rewards table for each state-action pair
            foreach (var state in this.poStates)
            {
                this.rewardTable.Add(state, new Dictionary<IAction, double>());
                foreach (var action in this.Agent.Actions.Values)
                {
                    //determines the state-action probability of getting a reward (at least one has reward)
                    var rewardProbability = this.GetSmallValue();
                    if ((numStateActionRewards > 0) && (rewardProbability > this.TestsConfig.StateActionRewardPercent))
                        continue;

                    //calculates random reward [0-1]
                    numStateActionRewards++;
                    var reward = this.GetSmallValue();
                    this.rewardTable[state][action] = reward;

                    //checks num state-action rewards limit
                    if (numStateActionRewards >= maxStateActionRewards)
                        return;
                }
            }
        }

        protected virtual void CreateStateTransitions()
        {
            //checks max transitions per state
            if (this.TestsConfig.MaxTransitionsPerStateAction == 0)
                this.TestsConfig.MaxTransitionsPerStateAction = (uint) Math.Max(1, this.poStates.Count/10);

            var actions = new List<IAction>(this.Agent.Actions.Values);

            //creates transitions for all states (as initial states)
            foreach (var initialState in this.poStates)
            {
                this.stateTransitionProbabilities[initialState] =
                    new Dictionary<IAction, Dictionary<PartiallyObservableState, double>>();

                //chooses an action that will lead to the same state
                var sameStateAction = actions[Rand.Next(actions.Count)];

                //adds state transitions for each action
                foreach (var action in actions)
                {
                    this.stateTransitionProbabilities[initialState][action] =
                        new Dictionary<PartiallyObservableState, double>();

                    var transitionProbabilityTotal = 0d;
                    var numTransitions = 0;

                    //if action leads to same state, add such transition
                    if (action == sameStateAction)
                    {
                        this.stateTransitionProbabilities[initialState][action][initialState] =
                            transitionProbabilityTotal = this.TestsConfig.MaxTransitionsPerStateAction == 1
                                ? 1
                                : this.GetSmallValue();
                        numTransitions = 1;
                    }

                    //adds other state transitions for the initial state and action
                    for (;
                        (numTransitions < this.TestsConfig.MaxTransitionsPerStateAction) &&
                        (transitionProbabilityTotal < 1f);
                        numTransitions++)
                    {
                        //gets final state
                        var finalState = this.GetNewTransition(initialState, action);

                        //gets next transition probability (small value)
                        var transitionProbability = this.GetSmallValue()*0.5f;

                        //checks last state transition or transition probability limit (1) reached
                        this.stateTransitionProbabilities[initialState][action][finalState] =
                            (((transitionProbability + transitionProbabilityTotal) > 1f) ||
                             (numTransitions == (this.TestsConfig.MaxTransitionsPerStateAction - 1)))
                                ? 1f - transitionProbabilityTotal
                                : transitionProbability;

                        transitionProbabilityTotal += transitionProbability;
                    }
                }
            }
        }

        protected virtual PartiallyObservableState GetNewTransition(PartiallyObservableState initialState,
            IAction action)
        {
            //gets a new final state for the given state 
            //which is different from other initial state transitions and from initial state itself
            PartiallyObservableState finalState = null;
            while ((finalState == null) || (finalState == initialState) ||
                   (this.stateTransitionProbabilities[initialState][action].ContainsKey(finalState)))
            {
                finalState = this.poStates[Rand.Next(this.poStates.Count)];
            }
            return finalState;
        }

        protected virtual PartiallyObservableState GetRandomNextState(PartiallyObservableState initialState,
            IAction action)
        {
            var transitionProbabilityTotal = 0d;
            var possibleTransitions = this.stateTransitionProbabilities[initialState][action];

            //gets a transition probability
            var transitionProbability = this.GetSmallValue();

            //goes through the possible state transitions until one probability fits the accumulated probability
            var numTransitions = 0;
            foreach (var transition in possibleTransitions)
            {
                transitionProbabilityTotal += transition.Value;
                if ((++numTransitions == possibleTransitions.Count) ||
                    (transitionProbability <= transitionProbabilityTotal))
                    return transition.Key;
            }

            //should not return null
            return null;
        }

        protected PartiallyObservableState GetRandomState()
        {
            return this.poStates[Rand.Next(this.poStates.Count)];
        }

        protected double GetSmallValue()
        {
            //returns small value between 0 and 1 (both inclusive)
            return Rand.Next(1001)/1000f;
        }

        #endregion

        #region Implementation of IXmlSerializable

        public override void InitElements()
        {
            this.Dispose();
        }

        #region Deserialization Methods

        public override void DeserializeXML(XmlElement element)
        {
            this.InitElements();

            this.DeserializeStatesXML(element);
            this.DeserializeActionsXML(element);
            this.DeserializeRewardsXML(element);
            this.DeserializeTransitionsXML(element);
        }

        protected virtual void DeserializeStatesXML(XmlElement element)
        {
            //creates dictionaries for state indexation
            var observableStateDictionary = new Dictionary<string, PartiallyObservableState.ObservableState>();
            var nonObservableStateDictionary = new Dictionary<string, PartiallyObservableState.NonObservableState>();

            foreach (XmlElement childElement in element.SelectNodes(STATES_TAG + "/" + STATE_TAG))
            {
                var nonObservableStateID = childElement.GetAttribute(NON_OBSERVABLE_STATE_TAG).ToLower();
                var observableStateID = childElement.GetAttribute(OBSERVABLE_STATE_TAG).ToLower();

                //checks partial states
                if (!observableStateDictionary.ContainsKey(observableStateID))
                    observableStateDictionary.Add(
                        observableStateID, new PartiallyObservableState.ObservableState {IdToken = observableStateID});

                if (!nonObservableStateDictionary.ContainsKey(nonObservableStateID))
                    nonObservableStateDictionary.Add(
                        nonObservableStateID,
                        new PartiallyObservableState.NonObservableState {IdToken = nonObservableStateID});

                //creates and adds full state
                this.poStates.Add(new PartiallyObservableState(
                    observableStateDictionary[observableStateID],
                    nonObservableStateDictionary[nonObservableStateID]));
            }

            //adds states to member list
            this.observableStates.AddRange(observableStateDictionary.Values);
            this.nonObservableStates.AddRange(nonObservableStateDictionary.Values);
        }

        protected virtual void DeserializeActionsXML(XmlElement element)
        {
            //reads, creates and adds actions
            foreach (XmlElement childElement in element.SelectNodes(ACTIONS_TAG + "/" + ACTION_TAG))
            {
                var action = new Actions.Action("action", null);
                action.DeserializeXML(childElement);
                if (!this.Agent.Actions.ContainsKey(action.IdToken))
                    this.Agent.Actions.Add(action.IdToken, action);
            }
        }

        protected virtual void DeserializeRewardsXML(XmlElement element)
        {
            //creates state index table
            var stateDictionary = this.poStates.ToDictionary(state => state.IdToken);

            //reads rewards and stores in table);
            foreach (XmlElement childElement in element.SelectNodes(REWARDS_TAG + "/" + REWARD_TAG))
            {
                //gets ids
                var stateID = childElement.GetAttribute(STATE_TAG).ToLower();
                var actionID = childElement.GetAttribute(ACTION_TAG).ToLower();
                var reward = double.Parse(childElement.GetAttribute(VALUE_TAG), CultureInfo.InvariantCulture);

                //checks state and action
                if (!stateDictionary.ContainsKey(stateID) || !this.Agent.Actions.ContainsKey(actionID)) continue;

                //gets state and action
                var state = stateDictionary[stateID];
                var action = this.Agent.Actions[actionID];
                if (!this.rewardTable.ContainsKey(state))
                    this.rewardTable[state] = new Dictionary<IAction, double>();

                //adds state-action reward to table
                this.rewardTable[state][action] = reward;
            }
        }

        protected virtual void DeserializeTransitionsXML(XmlElement element)
        {
            //creates state index table
            var stateDictionary = this.poStates.ToDictionary(state => state.IdToken);

            //reads state transitions and stores in table);
            foreach (XmlElement childElement in element.SelectNodes(TRANSITIONS_TAG + "/" + TRANSITION_TAG))
            {
                //gets ids
                var initialStateID = childElement.GetAttribute(INITIAL_STATE_TAG).ToLower();
                var actionID = childElement.GetAttribute(ACTION_TAG).ToLower();
                var finalStateID = childElement.GetAttribute(FINAL_STATE_TAG).ToLower();
                var probability = double.Parse(childElement.GetAttribute(VALUE_TAG), CultureInfo.InvariantCulture);

                //checks states and action
                if (!stateDictionary.ContainsKey(initialStateID) || !stateDictionary.ContainsKey(finalStateID) ||
                    !this.Agent.Actions.ContainsKey(actionID))
                    continue;

                //gets states and action
                var initialState = stateDictionary[initialStateID];
                var action = this.Agent.Actions[actionID];
                var finalState = stateDictionary[finalStateID];
                if (!this.stateTransitionProbabilities.ContainsKey(initialState))
                    this.stateTransitionProbabilities[initialState] =
                        new Dictionary<IAction, Dictionary<PartiallyObservableState, double>>();
                if (!this.stateTransitionProbabilities[initialState].ContainsKey(action))
                    this.stateTransitionProbabilities[initialState][action] =
                        new Dictionary<PartiallyObservableState, double>();

                //adds state transition probability to table
                this.stateTransitionProbabilities[initialState][action][finalState] = probability;
            }
        }

        #endregion

        #region Serialization Methods

        public override void SerializeXML(XmlElement element)
        {
            if (string.IsNullOrEmpty(this.IdToken)) this.IdToken = this.ToString();
            this.SerializeStatesXML(element);
            this.SerializeActionsXML(element);
            this.SerializeRewardsXML(element);
            this.SerializeTransitionsXML(element);
        }

        protected virtual void SerializeStatesXML(XmlElement element)
        {
            //serializes all states
            var childElement = element.OwnerDocument.CreateElement(STATES_TAG);
            element.AppendChild(childElement);
            foreach (var state in this.poStates)
            {
                var newChildElement = element.OwnerDocument.CreateElement(STATE_TAG);
                newChildElement.SetAttribute(NON_OBSERVABLE_STATE_TAG, state.NonObservablePart.IdToken);
                newChildElement.SetAttribute(OBSERVABLE_STATE_TAG, state.ObservablePart.IdToken);
                childElement.AppendChild(newChildElement);
            }
        }

        protected virtual void SerializeActionsXML(XmlElement element)
        {
            //serializes all actions
            var childElement = element.OwnerDocument.CreateElement(ACTIONS_TAG);
            element.AppendChild(childElement);
            foreach (Actions.Action action in this.Agent.Actions.Values)
            {
                var newChild = element.OwnerDocument.CreateElement(ACTION_TAG);
                action.SerializeXML(newChild);
                childElement.AppendChild(newChild);
            }
        }

        protected virtual void SerializeRewardsXML(XmlElement element)
        {
            //serializes all state-action rewards
            var childElement = element.OwnerDocument.CreateElement(REWARDS_TAG);
            element.AppendChild(childElement);
            foreach (var state in this.rewardTable.Keys)
                foreach (Actions.Action action in this.rewardTable[state].Keys)
                {
                    var newChildElement = element.OwnerDocument.CreateElement(REWARD_TAG);

                    var reward = this.rewardTable[state][action];

                    newChildElement.SetAttribute(STATE_TAG, state.IdToken);
                    newChildElement.SetAttribute(ACTION_TAG, action.IdToken);
                    newChildElement.SetAttribute(VALUE_TAG, reward.ToString(CultureInfo.InvariantCulture));
                    childElement.AppendChild(newChildElement);
                }
        }

        protected virtual void SerializeTransitionsXML(XmlElement element)
        {
            //serializes all state-action-transition-probability elements
            var childElement = element.OwnerDocument.CreateElement(TRANSITIONS_TAG);
            element.AppendChild(childElement);
            foreach (var initialState in this.stateTransitionProbabilities.Keys)
                foreach (Actions.Action action in this.stateTransitionProbabilities[initialState].Keys)
                    foreach (var finalState in this.stateTransitionProbabilities[initialState][action].Keys)
                    {
                        var newChildElement = element.OwnerDocument.CreateElement(TRANSITION_TAG);
                        var probability = this.stateTransitionProbabilities[initialState][action][finalState];
                        newChildElement.SetAttribute(INITIAL_STATE_TAG, initialState.IdToken);
                        newChildElement.SetAttribute(ACTION_TAG, action.IdToken);
                        newChildElement.SetAttribute(FINAL_STATE_TAG, finalState.IdToken);
                        newChildElement.SetAttribute(VALUE_TAG, probability.ToString(CultureInfo.InvariantCulture));
                        childElement.AppendChild(newChildElement);
                    }
        }

        #endregion

        #endregion
    }
}