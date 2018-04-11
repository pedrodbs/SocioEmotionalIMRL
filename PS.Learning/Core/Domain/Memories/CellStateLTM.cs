// ------------------------------------------
// <copyright file="CellStateLTM.cs" company="Pedro Sequeira">
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
using System.Globalization;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.States;

namespace PS.Learning.Core.Domain.Memories
{
    [Serializable]
    public class CellStateLTM : LongTermMemory
    {
        public const char STATE_SEPARATOR = ':';

        protected Dictionary<Cell, CellState> cellStates = new Dictionary<Cell, CellState>();

        public CellStateLTM(CellAgent agent, ShortTermMemory shortTermMemory)
            : base(agent, shortTermMemory)
        {
        }

        public new CellAgent Agent
        {
            get { return base.Agent as CellAgent; }
        }

        public override void Reset()
        {
            base.Reset();
            this.cellStates.Clear();
        }

        public override string ToString(IState state)
        {
            if (!(state is CellState)) return string.Empty;
            var cell = ((CellState) state).Cell;
            return string.Format(CultureInfo.InvariantCulture,
                                 "{0}{1}{2}", cell.XCoord, STATE_SEPARATOR, cell.YCoord);
        }

        public override IState GetUpdatedCurrentState()
        {
            return this.GetState(this.Agent.Cell);
        }

        public override IState FromString(string stateStr)
        {
            if (string.IsNullOrWhiteSpace(stateStr)) return null;

            var coordStrs = stateStr.Split(STATE_SEPARATOR);
            if (coordStrs.Length != 2) return null;
            var xCoord = uint.Parse(coordStrs[0]);
            var yCoord = uint.Parse(coordStrs[1]);

            return this.GetState(this.Agent.Environment.Cells[xCoord, yCoord]);
        }

        protected virtual IState GetState(Cell cell)
        {
            if (cell == null) return null;

            //checks if state already exists, or create new one
            var cellState = this.cellStates.ContainsKey(cell)
                                ? this.cellStates[cell]
                                : new CellState(cell);

            //stores new/existing state in current state
            this.cellStates[cell] = cellState;
            return cellState;
        }
    }
}