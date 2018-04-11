// ------------------------------------------
// <copyright file="SingleScenario.cs" company="Pedro Sequeira">
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
//    Last updated: 03/30/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using PS.Learning.Core.Testing.Simulations;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Environments;
using PS.Utilities.IO.Serialization;

namespace PS.Learning.Core.Testing.Config.Scenarios
{
    [Serializable]
    public class SingleScenario : IFitnessScenario
    {
        private readonly IAgent _baseAgent;
        private readonly IEnvironment _baseEnvironment;

        public SingleScenario(IAgent baseAgent, IEnvironment baseEnvironment, ITestsConfig testsConfig)
        {
            this._baseAgent = baseAgent;
            this._baseEnvironment = baseEnvironment;
            this.TestsConfig = testsConfig;

            //defaults, can be change in TestsConfig
            this.AgentFitnessFunction = new CumulativeFitnessFunction();
            this.FitnessText = "Fitness";
        }

        #region IFitnessScenario Members

        public ITestsConfig TestsConfig { get; set; }
        public string TestMeasuresFilePath { get; set; }
        public string EnvironmentConfigFile { get; set; }
        public string FilePath { get; set; }
        public uint MaxStates { get; set; }
        public string FitnessText { get; set; }

        public IAgent CreateAgent()
        {
            return this._baseAgent.CreateNew();
        }

        public IEnvironment CreateEnvironment()
        {
            return this._baseEnvironment.CreateNew();
        }

        public IScenario Clone()
        {
            return this.CloneJson();
        }

        public IScenario Clone(uint numSimulations, uint numSamples)
        {
            var profiles = this.TestsConfig.ScenarioProfiles;
            this.TestsConfig.ScenarioProfiles = null;
            var profile = this.Clone();
            profile.TestsConfig.NumSimulations = numSimulations;
            profile.TestsConfig.NumSamples = numSamples;
            this.TestsConfig.ScenarioProfiles = profile.TestsConfig.ScenarioProfiles = profiles;
            return profile;
        }

        public IAgentFitnessFunction AgentFitnessFunction { get; set; }

        #endregion
    }
}