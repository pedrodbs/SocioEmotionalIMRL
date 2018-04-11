// ------------------------------------------
// <copyright file="Move.cs" company="Pedro Sequeira">
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
using MathNet.Numerics.Random;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;

namespace PS.Learning.Core.Domain.Actions
{
    public abstract class Move : CellAction
    {
        protected const double HIT_REWARD = -5;
        protected const double MOVE_REWARD = -1;
        protected static readonly Random Random = new WH2006();

        protected Move(string id, ICellAgent agent) : base(id, agent)
        {
        }

        public new ICellAgent Agent
        {
            get { return base.Agent as ICellAgent; }
        }

        public override double Execute()
        {
            var totalReward = MOVE_REWARD;

            //gets move probability
            var moveProb = Random.NextDouble();
            if (!(moveProb <= this.Agent.Scenario.TestsConfig.ActionAccuracy)) 
                return totalReward + base.Execute();

            //gets next cell and moves accordingly if possible
            var nextCell = this.GetNextCell();
            if ((nextCell == null) || !this.IsWalkable(nextCell))
                totalReward += HIT_REWARD;
            else
                this.Agent.Cell = nextCell;

            return totalReward + base.Execute(); 
        }

        protected bool IsWalkable(Cell cell)
        {
            return this.Agent.Environment.IsWalkable(cell);
        }

        protected abstract Cell GetNextCell();
    }
}