// ------------------------------------------
// <copyright file="ParallelOptimizationTest.cs" company="Pedro Sequeira">
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
using System.IO;
using System.Threading;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Utilities.Core;
using PS.Utilities.IO;
using PS.Utilities.IO.Serialization;

namespace PS.Learning.Core.Testing.MultipleTests
{
    /// <summary>
    ///     Optimizes tests in parallel.
    ///     Test measures are shared between threads to store test results in a central place.
    /// </summary>
    public abstract class ParallelOptimizationTest : ITest, IProgressHandler
    {
        #region Constructors

        protected ParallelOptimizationTest(IOptimizationTestFactory optimizationTestFactory)
        {
            this.OptimizationTestFactory = optimizationTestFactory;
        }

        #endregion

        public bool LogStatistics { get; set; }

        #region IProgressHandler Members

        public abstract double ProgressValue { get; }

        #endregion

        #region ITest Members

        public virtual bool Run()
        {
            //runs tests one per CPU
            this.RunTestPerCPU(this.RunOptimizationTests);

            return true;
        }

        public virtual void Dispose()
        {
            this.TestMeasures.Dispose();
            if (this.LogWriter != null) this.LogWriter.Close();
        }

        public virtual void PrintResults()
        {
            this.PrintResults(false);
        }

        #endregion

        #region Constants

        public const string OPTIM_TESTS_ID = "Optim";
        public const string CUM_FIT_FILE_NAME = "CumFitness";
        public const string TEST_CFG_FILE_NAME = "Config";

        #endregion

        #region Fields

        protected readonly object locker = new object();
        protected string filePath;
        protected PerformanceMeasure performanceMeasure = new PerformanceMeasure();
        private IOptimizationTestFactory _optimizationTestFactory;

        #endregion

        #region Properties

        protected IScenario Scenario
        {
            get { return this.OptimizationTestFactory.Scenario; }
        }

        protected ITestsConfig TestsConfig
        {
            get { return this.Scenario.TestsConfig; }
        }

        public bool StopAllTests { get; set; }

        public virtual string TestID
        {
            get { return OPTIM_TESTS_ID; }
        }

        public TestMeasureList TestMeasures { get; protected set; }

        public IOptimizationTestFactory OptimizationTestFactory
        {
            get { return this._optimizationTestFactory; }
            set
            {
                this._optimizationTestFactory = value;
                if (value != null) 
                    this.filePath = this._optimizationTestFactory.Scenario.FilePath;
            }
        }

        public uint NumTimeSteps
        {
            get { return this.OptimizationTestFactory.Scenario.TestsConfig.NumTimeSteps; }
        }

        public uint NumSimulations
        {
            get { return this.OptimizationTestFactory.Scenario.TestsConfig.NumSimulations; }
        }

        public LogWriter LogWriter { get; protected set; }

        public string FilePath
        {
            get { return $"{this.filePath}{Path.DirectorySeparatorChar}{this.TestID}"; }
        }

        public long MemoryUsage
        {
            get { return this.performanceMeasure.MemoryUsage; }
        }

        public TimeSpan TestSpeed
        {
            get { return this.performanceMeasure.TimeElapsed; }
        }

        #endregion

        #region Protected Methods

        protected abstract void RunOptimizationTests();

        protected void PrintResults(bool printToCSV)
        {
            //writes tests' cumulative fitness to csv file
            this.TestMeasures.PrintToFile(this.GetLogFilePath(this.TestsConfig.TestMeasuresName, "csv"));

            //prints fitness quantity statistics
            this.TestMeasures.PrintFitnessStatistics(this.GetLogFilePath(CUM_FIT_FILE_NAME, "png"), printToCSV);

            //prints performance stats
            this.PrintTestPerformanceResults();
        }

        protected virtual string GetLogFilePath(string fileName, string extension)
        {
            return $"{this.FilePath}{Path.DirectorySeparatorChar}{fileName}{this.TestID}.{extension}";
        }

        protected virtual void PrintTestPerformanceResults()
        {
            this.WriteLine($"{this.TestID} has finished.");
            this.WriteLine($@"Time taken: {this.TestSpeed.ToString(@"hh\:mm\:ss")}");
            this.WriteLine($@"Memory used: {this.MemoryUsage} bytes");
        }

        public void InterruptProcessing(object sender, EventArgs e)
        {
            lock (this.locker) this.StopAllTests = true;
        }

        protected virtual void RunTestPerCPU(ThreadStart testRunner)
        {
            //reset test and starts performance meter
            this.ResetTest();

            //runs tests, one per processor
            ProcessUtil.RunThreads(testRunner, this.TestsConfig.MaxCPUsUsed);

            //stops performance meter and prints results
            this.performanceMeasure.Stop();
        }

        protected virtual void ResetTest()
        {
            if (this.LogStatistics)
            {
                //restarts test performances
                this.performanceMeasure.Start();
                this.StopAllTests = false;
                PathUtil.CreateOrClearDirectory(this.FilePath);

                //sets multiple factory path to tests' path
                this.OptimizationTestFactory.Scenario.FilePath = this.FilePath;
            }

            //creates and reads tests measures from file
            if (this.TestMeasures == null)
                this.TestMeasures = this.LogStatistics
                    ? this.OptimizationTestFactory.CreateAndInitTestMeasureList()
                    : this.OptimizationTestFactory.CreateTestMeasureList();
            this.TestMeasures.WriteTemp = this.LogStatistics;

            //verifies existence of test measures, use them to avoid unnecessary repetition
            if (File.Exists(this.OptimizationTestFactory.Scenario.TestMeasuresFilePath))
                this.TestMeasures.ReadFromFile(this.OptimizationTestFactory.Scenario.TestMeasuresFilePath);

            if (!this.LogStatistics) return;

            //writes test configuration
            var cfgFilePath = this.GetLogFilePath(TEST_CFG_FILE_NAME, "json");
            this.SerializeJsonFile(cfgFilePath, JsonUtil.ConfigSettings);

            //creates log
            this.LogWriter = new LogWriter(this.GetLogFilePath("", "log")) {WriteConsole = true};
            this.WriteLine(@"==========================================");
            this.WriteLine($"Running {this.TestID}...");
        }

        protected void WriteLine(string line)
        {
            if (this.LogWriter != null)
                this.LogWriter.WriteLine(line);
        }

        #endregion
    }
}