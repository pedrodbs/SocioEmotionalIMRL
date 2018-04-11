// ------------------------------------------
// <copyright file="Manager.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.Core

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Utilities.IO.Serialization;
using System;

namespace PS.Learning.Core.Domain.Managers
{
    [Serializable]
    public abstract class Manager : XmlResource, IManager
    {
        #region Protected Constructors

        protected Manager(IAgent agent)
        {
            this.Agent = agent;
        }

        #endregion Protected Constructors

        #region Public Properties

        public IAgent Agent { get; }

        #endregion Public Properties

        #region Protected Properties

        protected IScenario Scenario
        {
            get { return this.Agent.Scenario; }
        }

        protected ITestsConfig TestsConfig
        {
            get { return this.Agent.Scenario.TestsConfig; }
        }

        #endregion Protected Properties

        #region Public Methods

        public override void InitElements()
        {
        }

        public abstract void PrintResults(string path);

        public abstract void Reset();

        public abstract void Update();

        #endregion Public Methods
    }
}