// ------------------------------------------
// <copyright file="TestsConfig.cs" company="Pedro Sequeira">
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
//    Last updated: 03/10/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Utilities.Collections;
using PS.Utilities.IO.Serialization;

namespace PS.Learning.Core.Testing.Config
{
    [Serializable]
    //[JsonObject(MemberSerialization.OptIn)]
    public abstract class TestsConfig : ITestsConfig
    {
        //directories and files config
        protected string PreviousPhaseID { get; set; }
        protected string TestIDPrefix { get; set; }
        protected Dictionary<uint, string> TestIDs { get; set; }
        protected string DestinationTestsDir { get; set; }
        public HashSet<uint> CompletedTests { get; set; }

        #region ITestsConfig Members

        public double ActionAccuracy { get; set; }
        public string BaseFilePath { get; set; }
        public string TestListFilePath { get; set; }
        public string CondorServerBaseEnvPath { get; set; }
        public string CondorScriptPath { get; set; }
        public string MemoryBaseFilePath { get; set; }
        public string TestMeasuresName { get; set; }
        //test-related variables
        public bool RandStart { get; set; }
        //learning-related variables
        public double Epsilon { get; set; }
        public double Temperature { get; set; }
        public double Discount { get; set; }
        public double LearningRate { get; set; }
        public double MaximalChangeThreshold { get; set; }
        public double ExploratoryDecay { get; set; }
        public double InitialActionValue { get; set; }
        public abstract List<ITestParameters> GetSpecialTestParameters(IScenario scenario);
        public abstract List<ITestParameters> GetOptimizationTestParameters();
        public Dictionary<uint, IScenario> ScenarioProfiles { get; set; }
        public int CellSize { get; set; }

        public virtual object Clone()
        {
            return this.CloneJson();
        }

        public virtual void Init()
        {
            this.CreateTestsProfiles();
        }

        public virtual void SetDefaultConstants()
        {
            //sets custom TestMeasures name according to machine id
            this.TestMeasuresName = "TestMeasures";
            this.BaseFilePath = "./";
            this.EnvBaseFilePath = "../../../../bin/config/";
            this.CondorScriptPath = "./submit";
            this.CondorServerBaseEnvPath = "../config/";

            this.ActionAccuracy = 1;
            this.InitialActionValue = 0;
        }

        public abstract string GetTestName(IScenario scenario, ITestParameters testParameters);

        public abstract IOptimizationTestFactory CreateTestFactory(
            IScenario scenario, uint numSimulations, uint numSamples);

        #endregion

        public virtual IOptimizationTestFactory CreateTestFactory(IScenario scenario)
        {
            var numSimulations = scenario.TestsConfig.NumSimulations;
            var numSamples = scenario.TestsConfig.NumSamples;
            return this.CreateTestFactory(scenario, numSimulations, numSamples);
        }

        protected abstract void CreateTestsProfiles();

        #region Utility file naming methods

        protected virtual string GetFilePath(uint testType)
        {
            return Path.GetFullPath(
                $"{this.BaseFilePath}{Path.DirectorySeparatorChar}{this.GetPrefixedTestID(testType)}");
        }

        protected virtual string GetEnvironmentFilePath(string envFileName)
        {
            return Path.GetFullPath(
                $"{this.EnvBaseFilePath}{Path.DirectorySeparatorChar}{envFileName}");
        }

        protected virtual string GetTestMeasuresFilePath(uint testType)
        {
            return Path.GetFullPath(
                string.Format("{0}{4}{1}{4}{2}{4}{3}{2}.csv",
                    this.DestinationTestsDir, this.GetPrefixedTestID(testType), this.PreviousPhaseID,
                    this.TestMeasuresName, Path.DirectorySeparatorChar));
        }

        protected virtual string GetPrefixedTestID(uint testType)
        {
            return $"{this.TestIDPrefix}{this.GetTestID(testType)}";
        }

        protected virtual string GetTestID(uint testType)
        {
            return testType.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Json and Parsing

        #region Properties / args short and long names

        public const string NUM_TIME_STEPS_ARG = "timeSteps";
        public const char NUM_TIME_STEPS_CHAR = 's';
        private const string NUM_TIME_STEPS_MSG = "Number of time-steps to simulate.";

        public const string ENV_BASE_PATH_ARG = "envPath";
        public const char ENV_BASE_PATH_CHAR = 'e';
        private const string ENV_BASE_PATH_MSG = "Environment files path.";

        public const string ENABLE_GRAPHICS_ARG = "graphics";
        public const char ENABLE_GRAPHICS_CHAR = 'g';
        private const string ENABLE_GRAPHICS_MSG = "Whether to enable graphics / user interface.";

        public const string NUM_CPUS_ARG = "cpus";
        public const char NUM_CPUS_CHAR = 'c';
        private const string NUM_CPUS_MSG = "The number of CPUs to use for processing.";

        public const string NUM_SIMS_ARG = "numSims";
        public const char NUM_SIMS_CHAR = 'i';
        private const string NUM_SIMS_MSG = "The number of simulations to run.";

        public const string NUM_SAMPLES_ARG = "numSamples";
        public const char NUM_SAMPLES_CHAR = 'a';
        private const string NUM_SAMPLES_MSG = "The number of samples to get from quantities during simulations.";

        private const string TEST_TYPE_META_VALUE = "UInt32";
        public const string TEST_TYPE_ARG = "testType";
        public const char TEST_TYPE_CHAR = 't';
        private const string TEST_TYPE_MSG = "The default type of test to run.";

        public const string SINGLE_TEST_PARAMS_ARG = "params";
        public const char SINGLE_TEST_PARAMS_CHAR = 'p';
        private const string SINGLE_TEST_PARAMS_MSG = "The parameters of the single test.";

        public const string TEST_TYPES_ARG = "testTypes";
        public const char TEST_TYPES_CHAR = 'l';
        private const string TEST_TYPES_MSG = "A list of test types to run, separated by white spaces.";

        #endregion

        #region Properties

        [JsonProperty(NUM_TIME_STEPS_ARG)]
        [Option(NUM_TIME_STEPS_CHAR, NUM_TIME_STEPS_ARG, Required = false, HelpText = NUM_TIME_STEPS_MSG)]
        public uint NumTimeSteps { get; set; }

        [JsonProperty(ENV_BASE_PATH_ARG)]
        [Option(ENV_BASE_PATH_CHAR, ENV_BASE_PATH_ARG, Required = true, HelpText = ENV_BASE_PATH_MSG)]
        public string EnvBaseFilePath { get; set; }

        [JsonProperty(ENABLE_GRAPHICS_ARG)]
        [Option(ENABLE_GRAPHICS_CHAR, ENABLE_GRAPHICS_ARG, Required = false, DefaultValue = true,
            HelpText = ENABLE_GRAPHICS_MSG)]
        public bool GraphicsEnabled { get; set; } = true;

        [JsonProperty(NUM_CPUS_ARG)]
        [Option(NUM_CPUS_CHAR, NUM_CPUS_ARG, Required = false, DefaultValue = 1u, HelpText = NUM_CPUS_MSG)]
        public uint MaxCPUsUsed { get; set; } = 1u;

        [JsonProperty(NUM_SIMS_ARG)]
        [Option(NUM_SIMS_CHAR, NUM_SIMS_ARG, Required = true, HelpText = NUM_SIMS_MSG)]
        public uint NumSimulations { get; set; }

        [JsonProperty(NUM_SAMPLES_ARG)]
        [Option(NUM_SAMPLES_CHAR, NUM_SAMPLES_ARG, Required = true, HelpText = NUM_SAMPLES_MSG)]
        public uint NumSamples { get; set; }

        [JsonProperty(TEST_TYPE_ARG)]
        [Option(TEST_TYPE_CHAR, TEST_TYPE_ARG, MetaValue = TEST_TYPE_META_VALUE, Required = false, DefaultValue = 0u,
            HelpText = TEST_TYPE_MSG)]
        public uint SingleTestType { get; set; }

        [JsonProperty(SINGLE_TEST_PARAMS_ARG)]
        [OptionArray(SINGLE_TEST_PARAMS_CHAR, SINGLE_TEST_PARAMS_ARG, HelpText = SINGLE_TEST_PARAMS_MSG)]
        public string[] SingleTestParametersArgs { get; set; }

        public ITestParameters SingleTestParameters { get; set; }

        [JsonProperty(TEST_TYPES_ARG)]
        [OptionArray(TEST_TYPES_CHAR, TEST_TYPES_ARG, MetaValue = TEST_TYPE_META_VALUE + "[]",
            Required = false, HelpText = TEST_TYPES_MSG)]
        public uint[] MultipleTestTypes { get; set; }

        #endregion

        #region Serialization methods

        public void SerializeJsonFile(string fileName)
        {
            SerializeJsonFile(new List<ITestsConfig> {this}, fileName);
        }

        public static void SerializeJsonFile(IEnumerable<ITestsConfig> configList, string fileName)
        {
            if ((configList == null) || (fileName == null)) return;
            new List<ITestsConfig>(configList).SerializeJsonFile(fileName, JsonUtil.ArgsSettings);
        }

        public static List<ITestsConfig> DeserializeJsonFile(string fileName)
        {
            return JsonUtil.DeserializeJsonFile<List<ITestsConfig>>(fileName, JsonUtil.ArgsSettings);
        }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, this.FormatOptionHelpText);
        }

        private void FormatOptionHelpText(HelpText obj)
        {
            obj.FormatOptionHelpText += this.OnFormatOptionHelpText;
        }

        protected virtual void OnFormatOptionHelpText(object sender, FormatOptionHelpTextEventArgs e)
        {
            if (e.Option.LongName.Equals(SINGLE_TEST_PARAMS_ARG) && this.SingleTestParameters != null)
                e.Option.HelpText += this.SingleTestParameters.Header.ToString(' ');
        }

        public void OnParsingArguments()
        {
            this.OnDeserializing();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.OnDeserializing();
        }

        protected virtual void OnDeserializing()
        {
            //before deserialize first set default values
            this.SetDefaultConstants();
        }

        public void OnArgumentsParsed()
        {
            this.OnDeserialized();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.OnDeserialized();
        }

        protected virtual void OnDeserialized()
        {
            //gets test parameters from string[] args
            if ((this.SingleTestParametersArgs != null) && (this.SingleTestParameters != null))
                this.SingleTestParameters.FromValue(this.SingleTestParametersArgs);
        }

        public void OnGetArguments()
        {
            this.OnSerializing();
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            this.OnSerializing();
        }

        protected virtual void OnSerializing()
        {
            //gets string[] args from test parameters 
            if (this.SingleTestParameters != null)
                this.SingleTestParametersArgs = this.SingleTestParameters.ToValue();
        }

        #endregion

        #endregion
    }
}