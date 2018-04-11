// ------------------------------------------
// <copyright file="MultipleScenarioProfile.cs" company="Pedro Sequeira">
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
//    Last updated: 12/09/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Environments;
using PS.Utilities.IO.Serialization;

namespace PS.Learning.Core.Testing
{
    [Serializable]
    public class MultipleScenario : IScenario
    {
        private int _curProfileIdx;

        public MultipleScenario(IList<IScenario> testProfiles)
        {
            this.TestProfiles = testProfiles;
        }

        public IList<IScenario> TestProfiles { get; private set; }

        public IScenario CurrentScenario
        {
            get { return this.TestProfiles[this._curProfileIdx]; }
            set { this._curProfileIdx = this.TestProfiles.IndexOf(value); }
        }

        public bool AutoUpdate { get; set; }

        #region IScenarioProfile Members

        public ITestsConfig TestsConfig
        {
            get { return this.CurrentScenario.TestsConfig; }
            set { }
        }

        public string TestMeasuresFilePath { get; set; }
        public string FilePath { get; set; }

        public string EnvironmentConfigFile
        {
            get { return this.CurrentScenario.EnvironmentConfigFile; }
            set { }
        }

        public uint MaxStates
        {
            get { return this.CurrentScenario.MaxStates; }
            set { }
        }

        public IAgent CreateAgent()
        {
            return this.CurrentScenario.CreateAgent();
        }

        public IEnvironment CreateEnvironment()
        {
            return this.CurrentScenario.CreateEnvironment();
        }

        public IScenario Clone()
        {
            return this.CloneJson();
        }

        public IScenario Clone(uint numSimulations, uint numSamples)
        {
            var clone = this.Clone();
            clone.TestsConfig.NumSimulations = numSimulations;
            clone.TestsConfig.NumSamples = numSamples;
            return clone;
        }

        #endregion

        public virtual void Update()
        {
            if (this.AutoUpdate)
                this._curProfileIdx = this._curProfileIdx == this.TestProfiles.Count - 1 ? 0 : this._curProfileIdx + 1;
        }
    }
}