// ------------------------------------------
// <copyright file="KeeperGhost.cs" company="Pedro Sequeira">
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
//    Project: Learning.Tests.EmotionalOptimization

//    Last updated: 10/9/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using MathNet.Numerics.Random;
using PS.Learning.Core.Domain.Cells;

namespace PS.Learning.Tests.EmotionalOptimization.Domain.Ghosts
{
    [Serializable]
    public class KeeperGhost : Ghost
    {
        private const int GHOST_KEEP_PROB = 5; // in 10

        [NonSerialized] protected static readonly Random Rand = new WH2006(true);
        protected readonly ICellElement otherElement;

        public KeeperGhost(ICellElement otherElement)
        {
            this.otherElement = otherElement;
        }

        protected override void UpdateMovement()
        {
            //checks for same cell, nothing to do
            if ((this.Cell == null) || (this.Cell.Environment == null)) return;

            //if greedy prob, advance to bottom, else advance to other element
            var prob = Rand.Next(10);
            if (prob < GHOST_KEEP_PROB)
                this.AdvanceToBottom();
            else
                this.Cell.Environment.MoveToElement(this, this.otherElement);
        }

        protected virtual void AdvanceToBottom()
        {
            var downCell = this.Cell.YCoord < this.Cell.Environment.Rows - 1
                               ? this.Cell.Environment.Cells[this.Cell.XCoord, this.Cell.YCoord + 1]
                               : null;
            if ((downCell != null) && this.Cell.Environment.IsWalkable(downCell))
                this.Cell = downCell;
        }
    }
}