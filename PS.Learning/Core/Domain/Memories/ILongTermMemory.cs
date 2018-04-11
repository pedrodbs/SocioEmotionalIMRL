// ------------------------------------------
// <copyright file="ILongTermMemory.cs" company="Pedro Sequeira">
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
using System.Collections.Generic;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.States;
using PS.Utilities.Math;

namespace PS.Learning.Core.Domain.Memories
{
    public interface ILongTermMemory : IMemory
    {
        uint NumStates { get; }
        uint NumActions { get; }
        uint MaxStates { get; set; }
        ulong TimeStep { get; }
        IShortTermMemory ShortTermMemory { get; }
        double MaxStateValue { get; }
        double MinStateValue { get; }
        StatisticalQuantity NumTasks { get; }
        IState GetUpdatedCurrentState();
        IState FromString(string stateStr);
        string ToString(IState state);
        IState GetState(uint stateID);
        IAction GetAction(uint actionID);
        uint GetMaxStateAction(uint stateID);
        double GetMaxStateActionValue(uint stateID);
        double GetMinStateActionValue(uint stateID);
        double GetStateActionValue(uint stateID, uint actionID);
        double GetStateActionReward(uint stateID, uint actionID);
        uint GetStateActionNumber(uint stateID);
        double GetStateCount(uint stateID);
        double GetStateActionCount(uint stateID, uint actionID);
        double GetStateActionTransictionCount(uint initialStateID, uint actionID, uint finalStateID);
        double GetStateActionTransitionProbability(uint initialStateID, uint actionID, uint finalStateID);
        int GetMaxStateActionTransition(uint initialStateID, uint actionID);
        ulong GetTimeStepsLastStateAction(uint stateID, uint actionID);
        ulong GetTimeStepsLastState(uint stateID);
        void UpdateStateActionValue(uint stateID, uint actionID, double value);
        void UpdateStateActionReward(uint stateID, uint actionID, double reward);
        IEnumerable<KeyValuePair<uint, uint>> GetStatePredecessors(uint stateID);
        uint GetRandomStateActionTransition(uint initialStateID, uint actionID);
        void ResetAllStateActionValues();
        void UpdateMinMaxValues();
        bool ReadStateStats(string path);
        bool ReadStateActionStats(string path);
        bool ReadStateActionTransitionStats(string path);
        void WriteStateStats(string path);
        void WriteStateActionStats(string path);
        void WriteStateActionTransitionStats(string path);
        void ReadAllStats(string path);
    }
}