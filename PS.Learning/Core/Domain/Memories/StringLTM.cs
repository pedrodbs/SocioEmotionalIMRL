// ------------------------------------------
// <copyright file="StringLTM.cs" company="Pedro Sequeira">
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
//    Last updated: 10/9/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.States;

namespace PS.Learning.Core.Domain.Memories
{
    [Serializable]
    public class StringLTM : CellAgentLTM
    {
        public const char STATE_SEPARATOR = ':';

        protected Dictionary<string, StringState> stringStates = new Dictionary<string, StringState>();

        public StringLTM(CellAgent agent, ShortTermMemory shortTermMemory)
            : base(agent, shortTermMemory)
        {
        }

        public override void Dispose()
        {
            this.stringStates.Clear();
            base.Dispose();
        }

        public override string ToString(IState state)
        {
            return state.ToString();
        }

        public override IState GetUpdatedCurrentState()
        {
            return this.GetState(this.Agent.PerceptionManager.CurrentSensations);
        }

        public override IState FromString(string stateStr)
        {
            //verifies if state already created
            if (this.stringStates.ContainsKey(stateStr))
                return this.stringStates[stateStr];

            //gets sensations from string
            var sensations = stateStr.Split(new[] {STATE_SEPARATOR});

            //returns new state
            return this.GetState(new HashSet<string>(sensations));
        }

        protected virtual IState GetState(HashSet<string> sensations)
        {
            //creates list of stimuli sorted alphabetically
            var stimuliList = new List<string>(sensations);
            stimuliList.Sort();

            var stateStr = (stimuliList.Count == 0)
                               ? string.Empty
                               : stimuliList.Aggregate(
                                   (current, stimulus) => $"{current}{STATE_SEPARATOR}{stimulus}");

            //checks for new state
            if (!this.stringStates.ContainsKey(stateStr))
            {
                var stringState = new StringState(stateStr);
                this.stringStates[stateStr] = stringState;
            }

            return this.stringStates[stateStr];
        }
    }
}