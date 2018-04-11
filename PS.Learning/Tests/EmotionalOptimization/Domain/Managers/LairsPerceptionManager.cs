// ------------------------------------------
// <copyright file="LairsPerceptionManager.cs" company="Pedro Sequeira">
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

//    Last updated: 12/3/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.Managers.Perception;
using PS.Learning.Tests.EmotionalOptimization.Domain.Agents;

namespace PS.Learning.Tests.EmotionalOptimization.Domain.Managers
{
	public class LairsPerceptionManager : CellPerceptionManager
	{
		public LairsPerceptionManager (ILairsAgent agent)
            : base (agent)
		{
		}

		public new ILairsAgent Agent {
			get { return base.Agent as ILairsAgent; }
		}

		protected override void AddExternalSensations ()
		{
			//adds cell contents (including cell location), but doesn't see neighbour cells
			foreach (var cellElement in this.Agent.Cell.Elements)
				if (!cellElement.Equals (this.Agent) && !(cellElement is InfoCellElement) && !(cellElement is RabbitLair))
					this.CurrentSensations.Add (cellElement.IdToken);
		}

		protected override void AddInternalSensations ()
		{
			//adds states of both lairs
			this.AddLairSensations (this.Agent.Environment.Lair1);
			this.AddLairSensations (this.Agent.Environment.Lair2);
		}

		protected void AddLairSensations (RabbitLair lair)
		{
			//adds lair state
			this.CurrentSensations.Add ((lair.State != LairState.Rabbit ? lair.IdToken + "-" : string.Empty) + lair.State);
		}
	}
}