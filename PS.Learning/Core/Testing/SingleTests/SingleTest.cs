// ------------------------------------------
// <copyright file="SingleTest.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.Core

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.Simulations;
using PS.Utilities.Core;
using PS.Utilities.Forms.Math;
using PS.Utilities.IO;
using PS.Utilities.IO.Serialization;
using PS.Utilities.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PS.Learning.Core.Testing.SingleTests
{
    public abstract class SingleTest : ITest, IProgressHandler, IResetable
    {
        #region Protected Fields

        protected readonly Dictionary<Simulation, uint> currentSimulations = new Dictionary<Simulation, uint>();

        protected uint curSimulationIDx;

        protected Simulation lastSimulation;

        protected PerformanceMeasure performanceMeasure = new PerformanceMeasure();

        protected StatisticsCollection testStatisticsAvg;

        #endregion Protected Fields

        #region Private Fields

        private const char FILE_PATH_REPLACER_CHAR = '_';

        private const string TEST_CONFIG_FILE = "TestConfig.json";

        #endregion Private Fields

        #region Protected Constructors

        protected SingleTest(IScenario scenario, ITestParameters testParameters)
        {
            //checks arguments
            if (scenario == null)
                throw new ArgumentNullException(nameof(scenario), @"testProfile can't be null");

            this.TestParameters = testParameters;
            this.Scenario = scenario;
            this.TestName = scenario.TestsConfig.GetTestName(scenario, testParameters);
        }

        #endregion Protected Constructors

        #region Public Properties

        public string FilePath
            =>
                $"{this.Scenario.FilePath}{Path.DirectorySeparatorChar}{PathUtil.ReplaceInvalidChars(this.TestName, FILE_PATH_REPLACER_CHAR)}"
            ;

        /// <summary>
        ///     Stores the cumulative fitness final values for each test run.
        /// </summary>
        public StatisticalQuantity FinalScores { get; protected set; }

        public bool LogStatistics { get; set; }
        public LogWriter LogWriter { get; set; }
        public long MemoryUsage => this.performanceMeasure.MemoryUsage;
        public abstract double ProgressValue { get; }

        public IScenario Scenario { get; }

        public StatisticalQuantity SimulationScoreAvg => this.testStatisticsAvg?[Simulation.SCORE_ID];

        public string TestName { get; }

        public ITestParameters TestParameters { get; private set; }

        protected ITestsConfig TestsConfig => this.Scenario.TestsConfig;

        public TimeSpan TestSpeed => this.performanceMeasure.TimeElapsed;

        #endregion Public Properties

        #region Public Methods

        public abstract Simulation CreateAndSetupSimulation(uint simulationIDx);

        public virtual void Dispose()
        {
            this.LogWriter?.Close();
            this.testStatisticsAvg?.Dispose();
            this.testStatisticsAvg = null;
            this.FinalScores?.Dispose();
        }

        public virtual void FinishSimulation(Simulation simulation)
        {
            //writes score to screen
            if (this.currentSimulations.ContainsKey(simulation))
                this.WriteLine($"Simulation {this.currentSimulations[simulation]}: {simulation.Score.Value:0.00}");

            //averages test stats
            this.AverageTestStatistics(simulation);
        }

        public virtual void FinishTest()
        {
            //stops performance measures
            this.performanceMeasure.Stop();

            this.WriteTestResults();
        }

        public virtual void PrintResults()
        {
            this.PrintPerformanceResults();
            this.PrintAgent();
        }

        public virtual void Reset()
        {
            this.LogWriter?.Close();
            this.testStatisticsAvg?.Dispose();

            this.FinalScores = new StatisticalQuantity(this.TestsConfig.NumSimulations);
        }

        public virtual bool Run()
        {
            this.StartTest();
            this.RunTest();
            this.FinishTest();
            return true;
        }

        public virtual void RunSimulation(Simulation simulation)
        {
            simulation.Run();
            this.FinishSimulation(simulation);
        }

        public abstract void RunTest();

        public virtual void StartTest()
        {
            //start performance measures
            this.performanceMeasure.Start();
        }

        public void WriteProperties()
        {
            var filePath = $"{this.FilePath}{Path.DirectorySeparatorChar}{TEST_CONFIG_FILE}";
            this.SerializeJsonFile(filePath, JsonUtil.ConfigSettings);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void AverageTestStatistics(Simulation simulation)
        {
            //stores and replaces ref to last simulation
            if (this.lastSimulation != null)
                if (this.lastSimulation.Equals(simulation)) return;
                else this.lastSimulation.Dispose();
            this.lastSimulation = simulation;

            //adds cumulative (final) value to statistical variable
            this.FinalScores.Value = simulation.Score.Value;
            if (!this.LogStatistics) return;

            //stores or averages tests' statistics
            if (this.testStatisticsAvg == null)
                this.testStatisticsAvg = this.GetTestStatistics(simulation);
            else
                this.testStatisticsAvg.AverageWith(this.GetTestStatistics(simulation));

            //removes simulation from list
            this.currentSimulations.Remove(simulation);
        }

        protected virtual StatisticsCollection GetTestStatistics(Simulation simulation)
        {
            //adds all statistics relevant for the test from the given simulation
            // for single agent tests, this includes all the agent's and simulation's stats
            var statistics = simulation.Agent.StatisticsCollection.Clone();
            statistics.AddRange(simulation.StatisticsCollection.Clone());

            return statistics;
        }

        protected void PrintActionsStatistics()
        {
            //prints action info
            var actionQuantitiesList = new StatisticsCollection();
            actionQuantitiesList.AddRange(this.lastSimulation.Agent.Actions.Keys.ToDictionary(
                actionID => actionID,
                quantityName => this.testStatisticsAvg[quantityName]));
            actionQuantitiesList.PrintAllQuantitiesToCSV($"{this.FilePath}/Behavior/ActionsAvg.csv");
            actionQuantitiesList.PrintAllQuantitiesToImage($"{this.FilePath}/Behavior/ActionsAvg.png");
        }

        protected virtual void PrintAgent()
        {
            if (!this.LogStatistics) return;

            //prints agent statistics
            this.WriteLine(@"Printing agent statistical quantities...");
            this.PrintStatistic("StateActionValue", "/STM/StateActionValueAvg.csv");
            this.PrintStatistic("Epsilon", "/Learning/EpsilonAvg.csv");
            this.PrintStatistic("NumBackups", "/Learning/NumBackups.csv");

            //creates/cleans output directory
            var path = $"{this.FilePath}{Path.DirectorySeparatorChar}LTM";
            PathUtil.CreateOrClearDirectory(path);
            this.lastSimulation.Agent.LongTermMemory.PrintResults(path);
            //this.PrintAgentQuantity("PredictionError", "/Learning/PredictionErrorAvg.csv");

            this.PrintActionsStatistics();
        }

        protected virtual void PrintPerformanceResults()
        {
            //prints simulation cumulative score
            this.WriteLine(@"Printing agent performance...");
            this.SimulationScoreAvg.PrintStatisticsToCSV(
                $"{this.FilePath}{Path.DirectorySeparatorChar}CumulativeFitnessAvg.csv", false, true);
            this.FinalScores.PrintStatisticsToCSV(
                $"{this.FilePath}{Path.DirectorySeparatorChar}CumulativeFitnessValues.csv", false, true);
        }

        protected void PrintStatistic(string quantityName, string quantityFilePath)
        {
            this.testStatisticsAvg[quantityName].PrintStatisticsToCSV($"{this.FilePath}{quantityFilePath}.csv", false, true);
        }

        protected abstract bool TestHasFinished();

        protected void WriteLine(string line)
        {
            this.LogWriter?.WriteLine(line);
        }

        protected virtual void WriteTestResults()
        {
            this.WriteLine($"{this.TestName}{(" has finished.")}");
            this.WriteLine($"Time taken: {this.TestSpeed.ToString(@"hh\:mm\:ss")}");
            this.WriteLine($"Memory used: {this.MemoryUsage} bytes");
            this.WriteLine($"Test score: {this.FinalScores.Mean:0.00} ± {this.FinalScores.StdDev:0.00}");
        }

        #endregion Protected Methods
    }
}