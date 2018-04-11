// ------------------------------------------
// <copyright file="OptimizationTestFactory.cs" company="Pedro Sequeira">
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
using System.IO;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.SingleTests;

namespace PS.Learning.Core.Testing.MultipleTests
{
    [Serializable]
    public abstract class OptimizationTestFactory : IDisposable, IOptimizationTestFactory
    {
        protected const string TEMP_FILE_PREFIX = "Temp-";

        protected OptimizationTestFactory(IFitnessScenario scenario)
        {
            this.Scenario = scenario;
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        #region IOptimizationTestFactory Members

        public virtual TestMeasureList CreateAndInitTestMeasureList()
        {
            var testMeasures = this.CreateTestMeasureList();
            var tempFilePath =
                $"{this.Scenario.FilePath}{Path.DirectorySeparatorChar}{TEMP_FILE_PREFIX}{this.TestsConfig.TestMeasuresName}.csv";
            testMeasures.CreateTempWriter(tempFilePath);
            return testMeasures;
        }

        public virtual FitnessTest CreateTest(ITestParameters parameters)
        {
            var singleTest = new FitnessTest(this.Scenario, parameters);
            singleTest.Reset();
            return singleTest;
        }

        public virtual TestMeasureList CreateTestMeasureList()
        {
            //use as base parameters the single test params
            var baseMeasure = this.CreateTestMeasure(this.CreateTest(this.TestsConfig.SingleTestParameters));
            return new TestMeasureList(this.Scenario, baseMeasure);
        }

        public virtual TestMeasure CreateTestMeasure(FitnessTest test)
        {
            return new TestMeasure
                   {
                       ID = test.TestName,
                       Parameters = test.TestParameters,
                       Quantity = test.SimulationScoreAvg?.Clone(),
                       Value = test.FinalScores.Mean,
                       StdDev = test.FinalScores.StdDev
                   };
        }

        #endregion

        #region Properties

        protected ITestsConfig TestsConfig => this.Scenario.TestsConfig;

        public IFitnessScenario Scenario { get; }

        #endregion
    }
}