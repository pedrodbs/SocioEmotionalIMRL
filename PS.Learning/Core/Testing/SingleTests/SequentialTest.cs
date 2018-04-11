// ------------------------------------------
// <copyright file="SequentialTest.cs" company="Pedro Sequeira">
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

namespace PS.Learning.Core.Testing.SingleTests
{
    public abstract class SequentialTest : SingleTest
    {
        protected Simulation currentSimulation;

        protected SequentialTest(SingleScenario scenario, ITestParameters testParameters)
            : base(scenario, testParameters)
        {
        }

        public override double ProgressValue
        {
            get
            {
                var simulationSteps = (long) this.currentSimulation.Agent.LongTermMemory.TimeStep;
                var curStep = (this.curSimulationIDx*this.TestsConfig.NumTimeSteps) + simulationSteps;
                var maxSteps = this.TestsConfig.NumTimeSteps*this.TestsConfig.NumSimulations;

                return (double) curStep/maxSteps;
            }
        }

        public override void RunTest()
        {
            //runs episodes sequentially
            for (this.curSimulationIDx = 0;
                (this.curSimulationIDx < this.TestsConfig.NumSimulations) && !this.TestHasFinished();
                this.curSimulationIDx++)
            {
                //runs next simulation
				this.currentSimulation = this.CreateAndSetupSimulation(this.curSimulationIDx);
                //this.SetSimulationParams(this.currentSimulation);
                this.RunSimulation(this.currentSimulation);
            }
        }
    }
}