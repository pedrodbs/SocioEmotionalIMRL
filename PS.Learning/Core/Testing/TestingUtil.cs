// ------------------------------------------
// <copyright file="TestingUtil.cs" company="Pedro Sequeira">
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
//    Last updated: 06/26/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Utilities.IO.Serialization;

namespace PS.Learning.Core.Testing
{
    public static class TestingUtil
    {
        public static List<ITestsConfig> GenerateAllTestsConfigs(ITestsConfig baseTestsConfig)
        {
            var testsConfigList = new List<ITestsConfig>();

            var multipleTestTypes = baseTestsConfig.MultipleTestTypes;
            if ((multipleTestTypes == null) || (multipleTestTypes.Length == 0)) return testsConfigList;

            //gets all test parameters for each type of test
            foreach (var testType in multipleTestTypes)
                testsConfigList.AddRange(GetTestsConfig(baseTestsConfig, testType));

            return testsConfigList;
        }

        private static List<ITestsConfig> GetTestsConfig(ITestsConfig baseTestsConfig, uint testType)
        {
            //gets all test parameters for the type of test
            var testParamsSet = new HashSet<ITestParameters>(GetTestParameters(baseTestsConfig, testType));

            //removes possible unnecessary test parameters
            baseTestsConfig = baseTestsConfig.CloneJson();
            baseTestsConfig.Init();

            var testProfile = baseTestsConfig.ScenarioProfiles[testType];
            if (File.Exists(testProfile.TestMeasuresFilePath))
            {
                var testFactory = baseTestsConfig.CreateTestFactory(testProfile);
                var testMeasures = testFactory.CreateTestMeasureList();
                testMeasures.ReadFromFile(testProfile.TestMeasuresFilePath);
                testParamsSet.RemoveWhere(testMeasures.Contains);
            }

            //adds a new config for each specific test params
            return testParamsSet.Select(testParameters =>
                CreateNewTestsConfig(baseTestsConfig, testType, testParameters)).ToList();
        }

        private static List<ITestParameters> GetTestParameters(ITestsConfig baseTestsConfig, uint testType)
        {
            //gets sampled parameter list for given test type
            var testProfile = baseTestsConfig.ScenarioProfiles[testType];
            var testParameters = baseTestsConfig.GetOptimizationTestParameters();

            //also adds special tests
            testParameters.AddRange(baseTestsConfig.GetSpecialTestParameters(testProfile));

            return testParameters;
        }

        private static ITestsConfig CreateNewTestsConfig(
            ITestsConfig baseTestsConfig, uint testType, ITestParameters testParameters)
        {
            var testsConfig = baseTestsConfig.CloneJson();
            testsConfig.Init();
            testsConfig.SingleTestType = testType;
            testsConfig.SingleTestParameters = testParameters;
            return testsConfig;
        }
    }
}