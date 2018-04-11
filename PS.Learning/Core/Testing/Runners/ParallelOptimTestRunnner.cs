// ------------------------------------------
// <copyright file="ParallelOptimTestRunnner.cs" company="Pedro Sequeira">
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
using System.IO;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.MultipleTests;

namespace PS.Learning.Core.Testing.Runners
{
    public class ParallelOptimTestRunnner : TestRunner
    {
        protected readonly OptimizationScheme optimizationScheme;
        protected string prevTestMeasuresPath;

        public ParallelOptimTestRunnner(ITestsConfig testsConfig, OptimizationScheme optimizationScheme)
            : base(testsConfig)
        {
            this.optimizationScheme = optimizationScheme;
        }

        public override void RunTest()
        {
            //checks multiple test list 
            var multipleTestTypes = this.TestsConfig.MultipleTestTypes;
            if (multipleTestTypes == null) return;

            //runs all tests from list
            foreach (var testType in multipleTestTypes)
            {
                //resets variables
                this.prevTestMeasuresPath = null;

                //runs all tests for selected type
                this.RunTestsForType(testType);
            }
        }

        private void RunTestsForType(uint testType)
        {
            //creates optimization tests for a test type
            var optimizationTests = this.optimizationScheme.CreateFor(testType, this.TestsConfig);

            //runs tests in sequence
            foreach (var test in optimizationTests)
                this.RunTest(test);
        }

        protected virtual void RunTest(ParallelOptimizationTest test)
        {
            //tries to recover previous tests measures
            if (this.prevTestMeasuresPath != null)
                test.OptimizationTestFactory.Scenario.TestMeasuresFilePath = this.prevTestMeasuresPath;

            //runs test
            Console.WriteLine(@"----------------------------------------------");
            Console.WriteLine(@"Starting {0} test...", test.TestID);
            test.LogStatistics = this.TestsConfig.GraphicsEnabled;
            test.Run();

            //prints results and disposes
            Console.WriteLine(@"----------------------------------------------");
            Console.WriteLine(@"Printing test results...");

            if (this.TestsConfig.GraphicsEnabled)
                test.PrintResults();
            else
                this.PrintTestMeasures(test);

            Console.WriteLine(@"{0} test has finished.", test.TestID);

            this.prevTestMeasuresPath = test.TestMeasures.LastFilePath;

            test.Dispose();
        }

        private void PrintTestMeasures(ParallelOptimizationTest test)
        {
            //verifies path
            var filePath = Path.GetFullPath($"{test.FilePath}{Path.DirectorySeparatorChar}");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            //prints only test measures
            var testMeasuresFilePath =
                $"{filePath}{this.TestsConfig.TestMeasuresName}{test.TestID}.csv";
            test.TestMeasures.PrintToFile(testMeasuresFilePath);
        }
    }
}