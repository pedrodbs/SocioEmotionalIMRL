// ------------------------------------------
// <copyright file="SingleTestRunner.cs" company="Pedro Sequeira">
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
using PS.Learning.Core.Testing.SingleTests;
using PS.Utilities.Core;
using PS.Utilities.Collections;
using PS.Utilities.IO;

namespace PS.Learning.Core.Testing.Runners
{
    public class SingleTestRunner : TestRunner
    {
        protected SingleTestRunner(ITestsConfig testsConfig)
            : base(testsConfig)
        {
        }

        public override void RunTest()
        {
            //creates test of default TestType and with default parameters 
            var test = this.DefaultTestFactory.CreateTest(this.TestsConfig.SingleTestParameters);
            this.PrepareTest(test);

            //executes test
            this.RunSimulation(test);

            //creates and prints test measure
            this.PrintTestMeasure(test);

            test.Dispose();
        }

        private void PrepareTest(FitnessTest test)
        {
            test.LogStatistics = this.TestsConfig.GraphicsEnabled;

            if (!this.TestsConfig.GraphicsEnabled)
                return;

            //initiates log and writes all properties to file
            PathUtil.CreateOrClearDirectory(test.FilePath);
            test.LogWriter =  new LogWriter($"{test.FilePath}{Path.DirectorySeparatorChar}test.log");
            test.WriteProperties();
        }

        private void PrintTestMeasure(FitnessTest test)
        {
            //creates and prints test measure
            var testMeasure = this.DefaultTestFactory.CreateTestMeasure(test);
            var testMeasuresFilePath = Path.GetFullPath(
                string.Format("{0}{1}..{1}{2}.csv", test.FilePath, Path.DirectorySeparatorChar,
                    this.TestsConfig.TestMeasuresName));

            if (!File.Exists(testMeasuresFilePath))
                ExclusiveFileWriter.AppendLine(testMeasuresFilePath, ArrayUtil.ToString(testMeasure.Header));
            ExclusiveFileWriter.AppendLine(testMeasuresFilePath, ArrayUtil.ToString(testMeasure.ToValue()));
        }

        protected virtual void RunSimulation(FitnessTest test)
        {
            //just run test on console
            this.RunConsoleApplication(test);
        }

        protected virtual void RunConsoleApplication(FitnessTest test)
        {
            //runs test
            Console.WriteLine(@"----------------------------------------------");
            Console.WriteLine(@"Starting test {0}...", test.TestName);

            test.Run();

            if (this.TestsConfig.GraphicsEnabled)
            {
                //prints results
                Console.WriteLine(@"----------------------------------------------");
                Console.WriteLine(@"Printng test results...");
                test.PrintResults();
            }

            Console.WriteLine(@"----------------------------------------------");
            Console.WriteLine(@"Test has finished!");
        }
    }
}