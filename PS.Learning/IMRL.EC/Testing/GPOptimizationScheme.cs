// ------------------------------------------
// <copyright file="GPOptimizationScheme.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL.EC

//    Last updated: 4/2/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using PS.Learning.Core.Testing;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;

namespace PS.Learning.IMRL.EC.Testing
{
    public class GPOptimizationScheme : OptimizationScheme
    {
        private readonly uint _numSelectBestSimulations;

        public GPOptimizationScheme(ParallelOptimizationTest mainTest, uint numSelectBestSimulations = 0,
            IEnumerable<TopTestScheme> topTestsScheme = null, ParallelOptimizationTest finalTest = null)
            : base(mainTest, topTestsScheme, finalTest)
        {
            this._numSelectBestSimulations = numSelectBestSimulations;
        }

        protected override void AddMainTest(ITestsConfig testsConfig, IScenario scenario,
            List<ParallelOptimizationTest> tests)
        {
            base.AddMainTest(testsConfig, scenario, tests);
            if (!(testsConfig is IGPTestsConfig)) return;

            //adds a select best test
            var multipleTestFactory = 
                testsConfig.CreateTestFactory(scenario, this._numSelectBestSimulations, testsConfig.NumSamples);
            tests.Add(new SelectBestFitnessTest(multipleTestFactory, ((IGPTestsConfig) testsConfig).StdDevTimes));
        }

        protected override void AddFinalTest(ITestsConfig testsConfig, IScenario scenario, List<ParallelOptimizationTest> tests)
        {
            if (this.finalTest == null) return;
            if (!(testsConfig is IGPTestsConfig gpTestsConfig))
            {
                base.AddFinalTest(testsConfig, scenario, tests);
                return;
            }
            var testFactory = gpTestsConfig.CreateSimplifierTestFactory(
                scenario, testsConfig.NumSimulations, testsConfig.NumTimeSteps);
            this.finalTest.OptimizationTestFactory = testFactory;
            tests.Add(this.finalTest);
        }
    }
}