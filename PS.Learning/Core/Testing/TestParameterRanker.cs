// ------------------------------------------
// <copyright file="TestParameterRanker.cs" company="Pedro Sequeira">
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Utilities.Math;

namespace PS.Learning.Core.Testing
{
    public class TestParameterRanker
    {
        public TestParameterRanker(ITestsConfig testsConfig, IOptimizationTestFactory testFactory)
        {
            this.TestsConfig = testsConfig;
            this.TestFactory = testFactory;
            this.TestMeasures = testFactory.CreateTestMeasureList();
            this.TestMeasures.WriteTemp = false;
            this.SortedTestParameters = new Dictionary<uint, List<ITestParameters>>();
        }

        public ITestsConfig TestsConfig { get; private set; }
        public IOptimizationTestFactory TestFactory { get; private set; }
        public TestMeasureList TestMeasures { get; private set; }
        protected Dictionary<uint, List<ITestParameters>> SortedTestParameters { get; private set; }

        public void RankTests()
        {
            this.TestMeasures.Clear();

            //processes all tests types to read test results (measures)
            var multipleTestTypes = this.TestsConfig.MultipleTestTypes;
            if (multipleTestTypes == null) return;
            foreach (var testType in multipleTestTypes)
                this.ProcessTestType(testType);

            //creates a test measure list based on the average test parameters ranking
            this.CreateTestMeasureList();
        }

        protected void CreateTestMeasureList()
        {
            //goes through all the parameters 
            var testParameters = this.SortedTestParameters.Values.First();
            foreach (var testParameter in testParameters)
            {
                //gets average of rank for each test type
                var rankQuantity = new StatisticalQuantity((uint) this.TestsConfig.MultipleTestTypes.Length);
                foreach (var testType in TestsConfig.MultipleTestTypes)
                    rankQuantity.Value = this.SortedTestParameters[testType].IndexOf(testParameter);

                //creates measure based on this rank avg and adds to list
                var rankMeasure = new TestMeasure
                                  {
                                      Value = rankQuantity.Mean,
                                      StdDev = rankQuantity.StdDev,
                                      Parameters = testParameter,
                                      ID = testParameter.ToString()
                                  };
                this.TestMeasures.Add(testParameter, rankMeasure);
            }
        }

        protected void ProcessTestType(uint testType)
        {
            //gets profile and tests for measures list existence
            var testProfile = this.TestsConfig.ScenarioProfiles[testType];
            if (!File.Exists(testProfile.TestMeasuresFilePath)) return;

            //reads measure list from file, sorts measures and stores list for the type
            var testMeasures = this.TestFactory.CreateTestMeasureList();
            testMeasures.ReadFromFile(testProfile.TestMeasuresFilePath);
            this.SortedTestParameters[testType] = testMeasures.Sort();
        }
    }
}