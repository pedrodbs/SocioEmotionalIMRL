// ------------------------------------------
// <copyright file="ParallelTest.cs" company="Pedro Sequeira">
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

using System.Linq;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.Simulations;
using PS.Utilities.Core;

namespace PS.Learning.Core.Testing.SingleTests
{
    public abstract class ParallelTest : SingleTest
    {
        protected readonly object locker = new object();

        protected ParallelTest(IScenario scenario, ITestParameters testParameters)
            : base(scenario, testParameters)
        {
        }

        public bool StopAllSimulations { get; set; }

        public override double ProgressValue
        {
            get
            {
                lock (this.locker)
                {
                    var simulationSteps =
                        this.currentSimulations.Keys.Sum(
                            curSimulation => (int) curSimulation.Agent.LongTermMemory.TimeStep);
                    var curStep = ((this.curSimulationIDx - this.currentSimulations.Count)*this.TestsConfig.NumTimeSteps) +
                                  simulationSteps;
                    var maxSteps = this.TestsConfig.NumTimeSteps*this.TestsConfig.NumSimulations;

                    return (double) curStep/maxSteps;
                }
            }
        }

        public override void RunTest()
        {
            //resets simulation counter
            this.curSimulationIDx = 0;

            //runs run-simulation-method threads for each processor and waits for them to finish
            ProcessUtil.RunThreads(this.RunSingleSimulation, this.TestsConfig.MaxCPUsUsed);
        }

        protected virtual void RunSingleSimulation()
        {
            //loop until simulations end or external stop signal is given 
            while (!this.TestHasFinished())
            {
                Simulation nextSimulation;

                //lock on to take next simulation from list
                lock (this.locker)
                {
                    //gets next simulation if possible, or else returns (ends thread)
                    if (this.curSimulationIDx >= this.TestsConfig.NumSimulations) return;

                    //creates simulation, sets params and adds to current list
					nextSimulation = this.CreateAndSetupSimulation(this.curSimulationIDx);
                    //this.SetSimulationParams(nextSimulation);
                    this.currentSimulations.Add(nextSimulation, this.curSimulationIDx++);
                }

                //runs next simulation
                this.RunSimulation(nextSimulation);
            }
        }

        protected override bool TestHasFinished()
        {
            return this.StopAllSimulations;
        }

        protected override void AverageTestStatistics(Simulation simulation)
        {
            lock (this.locker)
                base.AverageTestStatistics(simulation);
        }
    }
}