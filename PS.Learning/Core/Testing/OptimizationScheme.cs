// ------------------------------------------
// <copyright file="OptimizationScheme.cs" company="Pedro Sequeira">
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
//    Last updated: 04/01/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;

namespace PS.Learning.Core.Testing
{
    public struct TopTestScheme
    {
        public TopTestScheme(uint numTests, uint numSimulations) : this()
        {
            this.NumTests = numTests;
            this.NumSimulations = numSimulations;
        }

        public uint NumTests { get; private set; }
        public uint NumSimulations { get; private set; }
    }

    public class OptimizationScheme : IDisposable
    {
        protected readonly ParallelOptimizationTest finalTest;
        protected readonly ParallelOptimizationTest mainTest;
        protected readonly List<TopTestScheme> topTestsScheme;

        public OptimizationScheme(ParallelOptimizationTest mainTest,
            IEnumerable<TopTestScheme> topTestsScheme = null, ParallelOptimizationTest finalTest = null)
        {
            if (mainTest == null) throw new ArgumentNullException("mainTest");

            this.mainTest = mainTest;
            this.finalTest = finalTest;
            if (topTestsScheme != null) this.topTestsScheme = topTestsScheme.ToList();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.topTestsScheme != null) this.topTestsScheme.Clear();
        }

        #endregion

        public IEnumerable<ParallelOptimizationTest> CreateFor(uint testType, ITestsConfig testsConfig)
        {
            var tests = new List<ParallelOptimizationTest>();
            var scenario = testsConfig.ScenarioProfiles[testType];
            if (scenario == null) return tests;

            //changes parameters of main optim test according to scenario
            this.AddMainTest(testsConfig, scenario, tests);

            //creates a top test according to scheme and scenario
            if (this.topTestsScheme == null) return tests;
            this.AddTopTests(testsConfig, scenario, tests);

            //changes parameters of final optim test according to scenario
            if (this.finalTest == null) return tests;
            this.AddFinalTest(testsConfig, scenario, tests);

            return tests;
        }

        protected virtual void AddFinalTest(
            ITestsConfig testsConfig, IScenario scenario, List<ParallelOptimizationTest> tests)
        {
            if (this.finalTest == null) return;
            var testFactory = testsConfig.CreateTestFactory(
                scenario, testsConfig.NumSimulations, testsConfig.NumTimeSteps);
            this.finalTest.OptimizationTestFactory = testFactory;
            tests.Add(this.finalTest);
        }

        protected virtual void AddMainTest(
            ITestsConfig testsConfig, IScenario scenario, List<ParallelOptimizationTest> tests)
        {
            var testFactory = testsConfig.CreateTestFactory(scenario, testsConfig.NumSimulations, testsConfig.NumSamples);
            this.mainTest.OptimizationTestFactory = testFactory;
            tests.Add(this.mainTest);
        }

        protected virtual void AddTopTests(
            ITestsConfig testsConfig, IScenario scenario, List<ParallelOptimizationTest> tests)
        {
            if (this.topTestsScheme == null) return;
            for (var i = 0; i < this.topTestsScheme.Count; i++)
            {
                var topTestScheme = this.topTestsScheme[i];

                //in last test always make 1 sample-per-step
                var testFactory = testsConfig.CreateTestFactory(
                    scenario, topTestScheme.NumSimulations,
                    i == this.topTestsScheme.Count - 1 ? testsConfig.NumTimeSteps : testsConfig.NumSamples);

                tests.Add(new SelectTopFitnessTest(testFactory, topTestScheme.NumTests, true));
            }
        }
    }
}