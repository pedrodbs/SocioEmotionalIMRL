// ------------------------------------------
// <copyright file="LongTermMemory.cs" company="Pedro Sequeira">
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
using System.Globalization;
using System.IO;
using System.Linq;
using MathNet.Numerics.Random;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Managers;
using PS.Learning.Core.Domain.States;
using PS.Utilities.Collections;
using PS.Utilities.Math;

namespace PS.Learning.Core.Domain.Memories
{
    [Serializable]
    public abstract class LongTermMemory : Manager, ILongTermMemory
    {
        #region Constructor

        protected LongTermMemory(IAgent agent, ShortTermMemory shortTermMemory)
            : base(agent)
        {
            this.ShortTermMemory = shortTermMemory;
            this.MaxStates = this.Scenario.MaxStates;
            this.NumTasks = new StatisticalQuantity();
        }

        #endregion

        #region Constants

        protected const uint DEFAULT_MAX_STATES = 10;
        protected const string ST_ACT_TRANS_STATS_FILE = "StateActionTransitionStats.csv";
        protected const string ST_ACT_STATS_FILE = "StateActionStats.csv";
        protected const string ST_STATS_FILE = "StateStats.csv";
        protected const string DIRECTORY_NAME = "LTM";

        #endregion

        #region Fields

        [NonSerialized] protected static readonly Random Rand = new WH2006(true);
        protected IAction[] actions;
        protected ulong[][] lastStateActionTimeStep;
        protected ulong[] lastStateTimeStep;
        protected double[][] stateActionCount;
        protected double[][] stateActionPredErrorAvg;
        protected double[][] stateActionRewardAvg;
        protected double[][][] stateActionTransitionCount;
        protected double[][] stateActionValue;
        protected ulong[][] stateActionValueUpdates;
        protected double[] stateCount;
        protected List<KeyValuePair<uint, uint>>[] statePredecessors;
        protected IState[] states;

        #endregion

        #region Properties

        public string ImgFormat { get; set; }

        public StatisticalQuantity NumTasks { get; protected set; }

        public uint NumStates { get; protected set; }

        public uint NumActions { get; protected set; }

        public uint MaxStates { get; set; }

        public ulong TimeStep { get; protected set; }

        public IShortTermMemory ShortTermMemory { get; protected set; }

        public double MaxStateValue { get; protected set; }

        public double MinStateValue { get; protected set; }

        #endregion

        #region Public Methods

        public abstract IState FromString(string stateStr);

        public abstract string ToString(IState state);

        public abstract IState GetUpdatedCurrentState();

        public override void Update()
        {
            //checks for new state
            this.CheckNewState(this.ShortTermMemory.CurrentState);

            //updates state and action statistics
            this.UpdateCounts();
        }

        public override void Reset()
        {
            this.InitMemory();
            this.ResetAllStateActionValues();
            this.TimeStep = 0;
            this.NumStates = 0;
        }

        public override void Dispose()
        {
            this.stateActionValue = null;
            this.stateActionCount = null;
            this.stateCount = null;
            this.stateActionValueUpdates = null;
            this.stateActionPredErrorAvg = null;
            this.lastStateTimeStep = null;
            this.lastStateActionTimeStep = null;
            this.stateActionTransitionCount = null;
            this.statePredecessors = null;
            this.stateActionRewardAvg = null;
        }

        #region Get Methods

        public IState GetState(uint stateID)
        {
            //checks state arg
            return stateID >= this.NumStates ? null : this.states[stateID];
        }

        public IAction GetAction(uint actionID)
        {
            //checks state arg
            return actionID >= this.NumActions ? null : this.actions[actionID];
        }

        public virtual uint GetMaxStateAction(uint stateID)
        {
            //checks state arg
            if (stateID >= this.NumStates) return uint.MaxValue;

            //builds list with max-valued actions for the given state
            var maxActionValue = double.MinValue;
            var maxStateActions = new List<uint>();
            for (var actionID = 0u; actionID < this.NumActions; actionID++)
            {
                var actionValue = this.stateActionValue[stateID][actionID];
                if (actionValue.Equals(maxActionValue))
                {
                    //if equal to max value, just add action to list
                    maxStateActions.Add(actionID);
                }
                else if (actionValue > maxActionValue)
                {
                    //if higher than max value, clear list and add action
                    maxActionValue = actionValue;
                    maxStateActions.Clear();
                    maxStateActions.Add(actionID);
                }
            }

            //returns random action from maximal set
            return maxStateActions.Count == 0
                ? uint.MaxValue
                : maxStateActions[Rand.Next(maxStateActions.Count)];
        }

        public virtual double GetMaxStateActionValue(uint stateID)
        {
            //returns max action value for given state
            return stateID >= this.NumStates ? 0 : this.stateActionValue[stateID].Max();
        }

        public virtual double GetMinStateActionValue(uint stateID)
        {
            //returns min action value for given state
            return stateID >= this.NumStates ? 0 : this.stateActionValue[stateID].Min();
        }

        public virtual double GetStateActionValue(uint stateID, uint actionID)
        {
            //checks arguments
            if ((stateID >= this.NumStates) || (actionID >= this.NumActions))
                return 0;

            //returns action value
            return this.stateActionValue[stateID][actionID];
        }

        public virtual double GetStateActionReward(uint stateID, uint actionID)
        {
            //checks arguments
            if ((stateID >= this.NumStates) || (actionID >= this.NumActions))
                return 0;

            //returns state-action reward average
            return this.stateActionRewardAvg[stateID][actionID];
        }

        public uint GetStateActionNumber(uint stateID)
        {
            //checks args
            if (stateID >= this.NumStates) return 0;

            //counts number of actions executed in this state
            var stateActionNum = 0u;
            for (var actionID = 0u; actionID < this.NumActions; actionID++)
                if (!this.GetStateActionCount(stateID, actionID).Equals(0))
                    stateActionNum++;

            return stateActionNum;
        }

        public virtual double GetStateCount(uint stateID)
        {
            //returns the state count
            return stateID >= this.NumStates ? 0 : this.stateCount[stateID];
        }

        public virtual double GetStateActionCount(uint stateID, uint actionID)
        {
            //returns the state-action pair count
            return (stateID >= this.NumStates) || (actionID >= this.NumActions)
                ? 0
                : this.stateActionCount[stateID][actionID];
        }

        public virtual double GetStateActionTransictionCount(uint initialStateID, uint actionID, uint finalStateID)
        {
            //returns state-action-transition count, if it exists, 0 otherwise
            return (initialStateID >= this.NumStates) || (actionID >= this.NumActions) ||
                   (finalStateID >= this.NumStates)
                ? 0
                : this.stateActionTransitionCount[initialStateID][actionID][finalStateID];
        }

        public virtual IEnumerable<KeyValuePair<uint, uint>> GetStatePredecessors(uint stateID)
        {
            return (stateID >= this.NumStates) ? null : this.statePredecessors[stateID];
        }

        public virtual double GetStateActionTransitionProbability(uint initialStateID, uint actionID, uint finalStateID)
        {
            //checks params
            if ((initialStateID >= this.NumStates) || (actionID >= this.NumActions) || (finalStateID >= this.NumStates))
                return 0;

            //gets state-action and state transition count
            var stActionCount = this.GetStateActionCount(initialStateID, actionID);
            var stateActionTransictionCount = this.GetStateActionTransictionCount(initialStateID, actionID, finalStateID);

            //return probability of state transtition
            return stActionCount.Equals(0)
                ? 0 //initialState == finalState ? 1 : 0
                : stateActionTransictionCount/stActionCount;
        }

        public virtual uint GetRandomStateActionTransition(uint stateID, uint actionID)
        {
            //verifies parameters
            if ((stateID >= this.NumStates) || (actionID >= this.NumActions))
                return uint.MaxValue;

            //gets a random transition based on the observation counts (greater prob for most observed transition states)
            var totalTransitionCount = (int) this.stateActionCount[stateID][actionID];
            var randomCount = Rand.Next(totalTransitionCount) + 1;
            var count = 0d;

            //gets all counts of transitions observed from given state and action
            for (var finalStateID = 0u; finalStateID < this.NumStates; finalStateID++)
            {
                count += this.stateActionTransitionCount[stateID][actionID][finalStateID];
                if (count >= randomCount)
                    return finalStateID;
            }

            //should not return unless there is no transition known
            return uint.MaxValue;
        }

        public virtual int GetMaxStateActionTransition(uint initialStateID, uint actionID)
        {
            //verifies parameters
            if ((initialStateID >= this.NumStates) || (actionID >= this.NumActions))
                return -1;

            //returns the state with the greatest number of transitions made to it
            var maxTransitionCount = double.MinValue;
            var maxState = -1;
            for (var finalStateID = 0; finalStateID < this.NumStates; finalStateID++)
            {
                var transitionCount = this.stateActionTransitionCount[initialStateID][actionID][finalStateID];
                if ((transitionCount.Equals(0)) || (transitionCount <= maxTransitionCount)) continue;

                maxTransitionCount = transitionCount;
                maxState = finalStateID;
            }
            return maxState;
        }

        public virtual ulong GetTimeStepsLastStateAction(uint stateID, uint actionID)
        {
            //checks arguments
            if ((stateID >= this.NumStates) || (actionID >= this.NumActions))
                return 0;

            //gets timestep difference of action execution
            var lastStActionTimeStep = this.lastStateActionTimeStep[stateID][actionID];
            return lastStActionTimeStep == 0 ? 0 : this.TimeStep - lastStActionTimeStep;
        }

        public virtual ulong GetTimeStepsLastState(uint stateID)
        {
            //checks arguments
            if (stateID >= this.NumStates) return 0;

            //gets timestep difference of state visit
            var lastStTimeStep = this.lastStateTimeStep[stateID];
            return lastStTimeStep == 0 ? 0 : this.TimeStep - lastStTimeStep;
        }

        public virtual IEnumerable<uint> GetAllStateActionTransitions(uint initialStateID, uint actionID)
        {
            //verifies parameters
            if ((initialStateID >= this.NumStates) || (actionID >= this.NumActions))
                return null;

            //returns list of all states that the agent transitioned from initial state taking the given action
            var finalStateIDs = new List<uint>();
            for (var finalStateID = 0u; finalStateID < this.NumStates; finalStateID++)
                if (!this.stateActionTransitionCount[initialStateID][actionID][finalStateID].Equals(0))
                    finalStateIDs.Add(finalStateID);
            return finalStateIDs;
        }

        public virtual double GetStateActionSupport(uint stateID, uint actionID)
        {
            //returns action support
            return (stateID >= this.NumStates) || (actionID >= this.NumActions)
                ? 0
                : this.stateActionCount[stateID][actionID]/this.stateCount[stateID];
        }

        public virtual double GetStateActionPredErrAvg(uint stateID, uint actionID)
        {
            //checks parameters
            if ((stateID >= this.NumStates) || (actionID >= this.NumActions))
                return 0;

            return this.stateActionPredErrorAvg[stateID][actionID];
        }

        #endregion

        #region Update Methods

        public virtual void UpdateStateActionValue(uint stateID, uint actionID, double value)
        {
            //checks null arguments
            if ((stateID >= this.NumStates) || (actionID >= this.NumActions)) return;

            //gets previous state value
            var prevStateActionValue = this.GetStateActionValue(stateID, actionID);

            //updates action value function
            this.stateActionValue[stateID][actionID] = value;

            //gets previous and current pred error
            var previousPredError = this.stateActionPredErrorAvg[stateID][actionID];
            var predError = Math.Abs(prevStateActionValue - value);

            //updates prediction error avg
            var stActionValueUpdates = ++this.stateActionValueUpdates[stateID][actionID];
            this.stateActionPredErrorAvg[stateID][actionID] =
                ((previousPredError*(stActionValueUpdates - 1)) + predError)/stActionValueUpdates;
        }

        public virtual void UpdateStateActionReward(uint stateID, uint actionID, double reward)
        {
            //checks parameters
            if ((stateID >= this.NumStates) || (actionID >= this.NumActions)) return;

            //updates action reward average
            var stActionCount = this.stateActionCount[stateID][actionID];
            var previousReward = this.stateActionRewardAvg[stateID][actionID];

            this.stateActionRewardAvg[stateID][actionID] = ((previousReward*(stActionCount - 1)) + reward)/stActionCount;
        }

        public virtual void ResetAllStateActionValues()
        {
            //sets all action-values to zero
            for (var stateID = 0u; stateID < this.MaxStates; stateID++)
                for (var actionID = 0u; actionID < this.stateActionValue[stateID].Length; actionID++)
                    this.stateActionValue[stateID][actionID] = this.TestsConfig.InitialActionValue;
        }

        public virtual void UpdateMinMaxValues()
        {
            //gets new min and max state values
            this.MaxStateValue = double.MinValue;
            for (var stateID = 0u; stateID < this.NumStates; stateID++)
                this.MaxStateValue = Math.Max(this.MaxStateValue, this.GetMaxStateActionValue(stateID));

            this.MinStateValue = double.MaxValue;
            for (var stateID = 0u; stateID < this.NumStates; stateID++)
                this.MinStateValue = Math.Min(this.MinStateValue, this.GetMinStateActionValue(stateID));
        }

        #endregion

        public virtual void InitMemory()
        {
            this.NumActions = (uint) this.Agent.Actions.Count;
            this.states = new IState[this.MaxStates];

            //inits actions array
            this.actions = new IAction[this.NumActions];
            var actionID = 0u;
            foreach (var action in this.Agent.Actions.Values)
            {
                this.actions[actionID] = action;
                action.ID = actionID;
                actionID++;
            }

            //initializes all arrays (allocates memory)
            this.lastStateTimeStep = new ulong[this.MaxStates];
            this.stateCount = new double[this.MaxStates];
            this.statePredecessors = new List<KeyValuePair<uint, uint>>[this.MaxStates];
            this.lastStateActionTimeStep = ArrayUtil.Create2DArray<ulong>(this.MaxStates, this.NumActions);
            this.stateActionValueUpdates = ArrayUtil.Create2DArray<ulong>(this.MaxStates, this.NumActions);
            this.stateActionCount = ArrayUtil.Create2DArray<double>(this.MaxStates, this.NumActions);
            this.stateActionPredErrorAvg = ArrayUtil.Create2DArray<double>(this.MaxStates, this.NumActions);
            this.stateActionRewardAvg = ArrayUtil.Create2DArray<double>(this.MaxStates, this.NumActions);
            this.stateActionValue = ArrayUtil.Create2DArray<double>(this.MaxStates, this.NumActions);
            this.stateActionTransitionCount = ArrayUtil.Create3DArray<double>(
                this.MaxStates, this.NumActions, this.MaxStates);
        }

        public override void PrintResults(string path)
        {
            //prints all results/stats
            this.WriteStateStats(path);
            this.WriteStateActionStats(path);
            this.WriteStateActionTransitionStats(path);
        }

        #endregion

        #region Protected Methods

        protected virtual void UpdateCounts()
        {
            //gets arguments
            var previousState = this.ShortTermMemory.PreviousState;
            var currentAction = this.ShortTermMemory.CurrentAction;
            var currentReward = this.ShortTermMemory.CurrentReward.Value;
            var currentState = this.ShortTermMemory.CurrentState;

            //checks arguments
            if ((previousState == null) || (currentAction == null) || (currentState == null)) return;

            //updates counts
            this.UpdateStateCounts(previousState.ID);
            this.UpdateStateActionCounts(previousState.ID, currentAction.ID);
            this.UpdateStateActionTransitions(previousState.ID, currentAction.ID, currentState.ID);
            this.UpdateStateActionReward(previousState.ID, currentAction.ID, currentReward);

            //updates num tasks
            this.UpdateNumTasks(previousState, currentAction);

            //update time step
            this.TimeStep++;
        }

        protected virtual void UpdateNumTasks(IState previousState, IAction currentAction)
        {
            if ((this.Agent is ICellAgent) && ((ICellAgent) this.Agent).Environment.AgentFinishedTask(
                this.Agent, previousState, currentAction))
                ++this.NumTasks.Value;
        }

        protected virtual void UpdateStateCounts(uint stateID)
        {
            //updates state count
            this.stateCount[stateID] = this.GetStateCount(stateID) + 1;

            //updates state count time step;
            this.lastStateTimeStep[stateID] = this.TimeStep;
        }

        protected virtual void UpdateStateActionCounts(uint stateID, uint actionID)
        {
            //updates state-action pair count
            this.stateActionCount[stateID][actionID] = this.GetStateActionCount(stateID, actionID) + 1;

            //updates state-action count time step;
            this.lastStateActionTimeStep[stateID][actionID] = this.TimeStep;
        }

        protected virtual void UpdateStateActionTransitions(
            uint previousStateID, uint currentActionID, uint currentStateID)
        {
            //checks arguments
            if ((this.NumStates == 0) || (previousStateID == uint.MaxValue) || (currentActionID == uint.MaxValue) ||
                (currentStateID == uint.MaxValue)) return;

            var stateActionTransictionCount =
                this.GetStateActionTransictionCount(previousStateID, currentActionID, currentStateID);

            //updates predecessor list
            if (stateActionTransictionCount.Equals(0))
                this.statePredecessors[currentStateID].Add(new KeyValuePair<uint, uint>(previousStateID, currentActionID));

            //updates state-action-state count
            this.stateActionTransitionCount[previousStateID][currentActionID][currentStateID] =
                stateActionTransictionCount + 1;
        }

        protected virtual void CheckNewState(IState state)
        {
            //checks for new state (ID == NEW_STATE_ID)
            if ((state == null) || (state.ID != State.NEW_STATE_ID) && (this.states[state.ID] != null))
                return;

            //increments state count and stores new state
            this.states[state.ID = this.NumStates++] = state;

            //initializes elements
            this.statePredecessors[state.ID] = new List<KeyValuePair<uint, uint>>();
        }

        #endregion

        #region Serialization methods

        public virtual void ReadAllStats(string path)
        {
            //verifies path
            if (!Directory.Exists(path)) return;

            //tries to read all stats from within
            this.ReadStateStats(path);
            this.ReadStateActionStats(path);
            this.ReadStateActionTransitionStats(path);
        }

        public virtual bool ReadStateStats(string path)
        {
            return this.ReadStats(path, ST_STATS_FILE, this.ReadStateFileLine);
        }

        public virtual bool ReadStateActionStats(string path)
        {
            return this.ReadStats(path, ST_ACT_STATS_FILE, this.ReadStateActionFileLine);
        }

        public virtual bool ReadStateActionTransitionStats(string path)
        {
            return this.ReadStats(path, ST_ACT_TRANS_STATS_FILE, this.ReadStateActionTransitionFileLine);
        }

        public virtual void WriteStateStats(string path)
        {
            //checks file
            var fileName = $"{path}{Path.DirectorySeparatorChar}{ST_STATS_FILE}";
            if (File.Exists(fileName)) File.Delete(fileName);

            var sw = new StreamWriter(fileName);

            //writes header
            sw.WriteLine(this.GetStateFileHeader());

            //writes stats for each state
            for (var stateID = 0u; stateID < this.NumStates; stateID++)
                sw.WriteLine(this.GetStateFileLine(stateID));

            sw.Close();
            sw.Dispose();
        }

        public virtual void WriteStateActionStats(string path)
        {
            //checks file
            var fileName = $"{path}{Path.DirectorySeparatorChar}{ST_ACT_STATS_FILE}";
            if (File.Exists(fileName)) File.Delete(fileName);

            var sw = new StreamWriter(fileName);

            //writes header
            sw.WriteLine(this.GetStateActionFileHeader());

            //writes stats for each state-action pair
            for (var stateID = 0u; stateID < this.NumStates; stateID++)
                for (var actionID = 0u; actionID < this.NumActions; actionID++)
                    sw.WriteLine(this.GetStateActionFileLine(stateID, actionID));

            sw.Close();
            sw.Dispose();
        }

        public virtual void WriteStateActionTransitionStats(string path)
        {
            //checks file
            var fileName = $"{path}{Path.DirectorySeparatorChar}{ST_ACT_TRANS_STATS_FILE}";
            if (File.Exists(fileName)) File.Delete(fileName);

            var sw = new StreamWriter(fileName);

            //writes header
            sw.WriteLine(this.GetStateActionTransitionFileHeader());

            //writes stats for each state-action pair
            for (var initialStateID = 0u; initialStateID < this.NumStates; initialStateID++)
                for (var actionID = 0u; actionID < this.NumActions; actionID++)
                    for (var finalStateID = 0u; finalStateID < this.NumStates; finalStateID++)
                        sw.WriteLine(this.GetStateActionTransitionFileLine(initialStateID, actionID, finalStateID));

            sw.Close();
            sw.Dispose();
        }

        protected virtual bool ReadStats(string path, string fileTypeName, LineHandler lineHandler)
        {
            //checks file
            var fileName = $"{path}{Path.DirectorySeparatorChar}{fileTypeName}";
            if (!File.Exists(fileName)) return false;

            var sr = new StreamReader(fileName);

            //ignore header
            sr.ReadLine();

            //reads stats from each line
            string line;
            while ((line = sr.ReadLine()) != null)
                lineHandler(line);

            sr.Close();
            sr.Dispose();

            return true;
        }

        protected virtual string GetStateFileHeader()
        {
            return "State,Count,Max Action,Max Action Value,Max Action Policy,Last Time Step";
        }

        protected virtual string GetStateFileLine(uint stateID)
        {
            var maxStateAction = this.GetMaxStateAction(stateID);
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5}",
                this.ToString(this.states[stateID]),
                this.GetStateCount(stateID),
                this.actions[maxStateAction],
                this.GetStateActionValue(stateID, maxStateAction),
                this.Agent.BehaviorManager.GetPolicy(stateID)[maxStateAction],
                this.lastStateTimeStep[stateID]);
        }

        protected virtual void ReadStateFileLine(string line)
        {
            //gets cs values
            var fields = line.Split(',');
            if (fields.Length < 6) return;

            //tries to get state
            var idx = 0;
            var state = this.FromString(fields[idx]);
            if (state == null) return;
            this.CheckNewState(state);

            //tries to get other fields
            double.TryParse(fields[++idx], out this.stateCount[state.ID]);
            ++idx;
            ++idx;
            ++idx;
            ulong.TryParse(fields[++idx], out this.lastStateTimeStep[state.ID]);
        }

        protected virtual string GetStateActionFileHeader()
        {
            return
                "State,Action,Action Val,Policy,Avg Reward,Count,Support,Last State Step," +
                "Last State-Action Step,Avg Pred Error,Action Val Updates;";
        }

        protected virtual string GetStateActionFileLine(uint stateID, uint actionID)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                this.ToString(this.states[stateID]),
                this.actions[actionID],
                this.GetStateActionValue(stateID, actionID),
                this.Agent.BehaviorManager.GetPolicy(stateID)[actionID],
                this.GetStateActionReward(stateID, actionID),
                this.GetStateActionCount(stateID, actionID),
                this.GetStateActionSupport(stateID, actionID),
                this.lastStateTimeStep[stateID],
                this.lastStateActionTimeStep[stateID][actionID],
                this.stateActionPredErrorAvg[stateID][actionID],
                this.stateActionValueUpdates[stateID][actionID]);
        }

        protected virtual void ReadStateActionFileLine(string line)
        {
            //gets cs values
            var fields = line.Split(',');
            if (fields.Length < 11) return;

            //tries to get state
            var idx = 0;
            var state = this.FromString(fields[idx]);
            if (state == null) return;
            this.CheckNewState(state);

            //tries to get action
            var actionName = fields[++idx];
            if (!this.Agent.Actions.ContainsKey(actionName)) return;
            var action = this.Agent.Actions[actionName];

            //tries to get other fields
            double.TryParse(fields[++idx], out this.stateActionValue[state.ID][action.ID]);
            ++idx;
            double.TryParse(fields[++idx], out this.stateActionRewardAvg[state.ID][action.ID]);
            double.TryParse(fields[++idx], out this.stateActionCount[state.ID][action.ID]);
            ++idx;
            ulong.TryParse(fields[++idx], out this.lastStateTimeStep[state.ID]);
            ulong.TryParse(fields[++idx], out this.lastStateActionTimeStep[state.ID][action.ID]);
            double.TryParse(fields[++idx], out this.stateActionPredErrorAvg[state.ID][action.ID]);
            ulong.TryParse(fields[++idx], out this.stateActionValueUpdates[state.ID][action.ID]);
        }

        protected virtual string GetStateActionTransitionFileHeader()
        {
            return "Initial State,Action,Final State,Num Transitions,State-Action Count,Trans Prob";
        }

        protected virtual string GetStateActionTransitionFileLine(
            uint initialStateID, uint actionID, uint finalStateID)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5}",
                this.ToString(this.states[initialStateID]),
                this.actions[actionID],
                this.ToString(this.states[finalStateID]),
                this.GetStateActionTransictionCount(initialStateID, actionID, finalStateID),
                this.GetStateActionCount(initialStateID, actionID),
                this.GetStateActionTransitionProbability(initialStateID, actionID, finalStateID));
        }

        protected virtual void ReadStateActionTransitionFileLine(string line)
        {
            //gets cs values
            var fields = line.Split(',');
            if (fields.Length < 6) return;

            //tries to get initial state
            var idx = 0;
            var initialState = this.FromString(fields[idx]);
            if (initialState == null) return;
            this.CheckNewState(initialState);

            //tries to get action
            var actionName = fields[++idx];
            if (!this.Agent.Actions.ContainsKey(actionName)) return;
            var action = this.Agent.Actions[actionName];

            //tries to get final state
            var finalState = this.FromString(fields[++idx]);
            if (finalState == null) return;
            this.CheckNewState(finalState);

            //tries to get state action transition count
            double stActionTransCount;
            if (double.TryParse(fields[++idx], out stActionTransCount))
            {
                this.UpdateStateActionTransitions(initialState.ID, action.ID, finalState.ID);
                this.stateActionTransitionCount[initialState.ID][action.ID][finalState.ID] = stActionTransCount;
            }

            //tries to get other fields
            double.TryParse(fields[++idx], out this.stateActionCount[initialState.ID][action.ID]);
        }

        protected delegate void LineHandler(string line);

        #endregion
    }
}