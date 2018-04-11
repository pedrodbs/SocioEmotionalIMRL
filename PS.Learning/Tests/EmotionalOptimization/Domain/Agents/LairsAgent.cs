// ------------------------------------------
// <copyright file="LairsAgent.cs" company="Pedro Sequeira">
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

//    Last updated: 10/10/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Domain.Managers.Perception;
using PS.Learning.IMRL.Emotions.Domain.Agents;
using PS.Learning.Tests.EmotionalOptimization.Domain.Actions;
using PS.Learning.Tests.EmotionalOptimization.Domain.Environments;
using PS.Learning.Tests.EmotionalOptimization.Domain.Managers;

namespace PS.Learning.Tests.EmotionalOptimization.Domain.Agents
{
    [Serializable]
    public class LairsAgent : EmotionalAgent, ILairsAgent
    {
        private const string OPEN_ACTION_ID = "Open Lair";

        public LairsAgent()
        {
            //mandatory eat action here
            this.AutoEat = false;
        }

        #region ILairsAgent Members

        public new LairsEnvironment Environment
        {
            get { return base.Environment as LairsEnvironment; }
        }

        #endregion

        protected override PerceptionManager CreatePerceptionManager()
        {
            return new LairsPerceptionManager(this);
        }

        protected override void CreateActions()
        {
            base.CreateActions();

            //add action to open lair
            this.Actions.Add(OPEN_ACTION_ID, new OpenLair(OPEN_ACTION_ID, this));
        }
    }
}