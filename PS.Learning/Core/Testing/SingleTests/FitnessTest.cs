// ------------------------------------------
// <copyright file="FitnessTest.cs" company="Pedro Sequeira">
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

using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.Simulations;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Environments;

namespace PS.Learning.Core.Testing.SingleTests
{
    public class FitnessTest : ParallelTest
    {
        #region Constructor

        public FitnessTest(IFitnessScenario scenario, ITestParameters testParameters)
            : base(scenario, testParameters)
        {
        }

        #endregion

        #region Properties

        protected new IFitnessScenario Scenario
        {
            get { return base.Scenario as IFitnessScenario; }
        }

        #endregion

        #region Creation Methods

        protected virtual IAgent CreateAgent()
        {
            //gets new agent from factory
            return this.Scenario.CreateAgent();
        }

        protected virtual IEnvironment CreateEnvironment()
        {
            //gets new environment from factory
            return this.Scenario.CreateEnvironment();
        }

		protected virtual Simulation CreateSimulation(uint simulationIDx, IAgent agent)
        {
			return new FitnessSimulation(simulationIDx, agent, this.Scenario);
        }

		public override Simulation CreateAndSetupSimulation(uint simulationIDx)
        {
            //sets up and initiates agent
            var agent = this.CreateAgent();
            agent.StatisticsCollection.SampleSteps = this.TestsConfig.NumTimeSteps/this.TestsConfig.NumSamples;
            agent.StatisticsCollection.MaxNumSamples = this.TestsConfig.NumSamples;

            //creates new environment and cells
            var environment = this.CreateEnvironment();
            if (environment != null)
            {
                environment.CreateCells(3, 3, this.Scenario.EnvironmentConfigFile);
                ((SingleAgentEnvironment) environment).Agent = (CellAgent) agent;
                environment.Scenario = this.Scenario;
                environment.Init();
                environment.Reset();
            }

            this.SetAgentParameters(agent);

			return this.CreateSimulation(simulationIDx, agent);
        }

        protected virtual void SetAgentParameters(IAgent agent)
        {
            //inits agent
            agent.Scenario = this.Scenario;
            agent.TestParameters = this.TestParameters;
            agent.Init();
        }

        #endregion
    }
}