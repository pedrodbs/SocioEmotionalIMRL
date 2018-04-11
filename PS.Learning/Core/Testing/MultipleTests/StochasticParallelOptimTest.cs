// ------------------------------------------
// <copyright file="StochasticParallelOptimTest.cs" company="Pedro Sequeira">
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
//    Last updated: 3/10/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.StochasticOptimization;
using PS.Utilities.Math;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PS.Utilities.Forms.Math;

namespace PS.Learning.Core.Testing.MultipleTests
{
    public abstract class StochasticParallelOptimTest : ParallelOptimizationTest
    {
        #region Protected Fields

        protected readonly List<IStochasticOptimizationTest> currentOptimTests = new List<IStochasticOptimizationTest>();
        protected uint curNumOptimTests;

        #endregion Protected Fields

        #region Protected Constructors

        protected StochasticParallelOptimTest(IOptimizationTestFactory optimizationTestFactory)
            : base(optimizationTestFactory)
        {
        }

        #endregion Protected Constructors

        #region Public Properties

        public StatisticalQuantity FitnessMaxProgressAvg { get; protected set; }

        public override double ProgressValue
        {
            get
            {
                lock (this.locker)
                {
                    if (this.currentOptimTests.Count == 0) return 0;

                    var testsConfig = TestsConfig;
                    var maxTotalIterations = (double)testsConfig.MaxIterations * testsConfig.NumParallelOptimTests;
                    var curNumIterations = this.currentOptimTests.Sum(population => population.IterationNumber);
                    var previousIterations = (this.curNumOptimTests - this.currentOptimTests.Count) *
                                             testsConfig.MaxIterations;
                    var curTotalIterations = (double)previousIterations + curNumIterations;

                    return curTotalIterations / maxTotalIterations;
                }
            }
        }

        #endregion Public Properties

        #region Protected Properties

        protected new IStochasticOptimTestsConfig TestsConfig
        {
            get { return base.TestsConfig as IStochasticOptimTestsConfig; }
        }

        #endregion Protected Properties

        #region Public Methods

        public override void Dispose()
        {
            base.Dispose();
            this.FitnessMaxProgressAvg.Dispose();
        }

        public override bool Run()
        {
            //runs genetic tests
            if (this.NumSimulations != 0)
                this.RunTestPerCPU(this.RunOptimizationTests);

            return true;
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract IStochasticOptimizationTest CreateStochasticOptimTest(uint optimTestNumber);

        protected override void PrintTestPerformanceResults()
        {
            base.PrintTestPerformanceResults();

            this.FitnessMaxProgressAvg.PrintStatisticsToCSV(
                $"{this.FilePath}{Path.DirectorySeparatorChar}FitnessMaxAvg.csv", false, true);
        }

        protected override void ResetTest()
        {
            base.ResetTest();
            this.currentOptimTests.Clear();
        }

        protected virtual void RunIteration(IStochasticOptimizationTest optimizationTest)
        {
            this.WriteLine(
                $"Running {optimizationTest.ID} iteration {optimizationTest.IterationNumber}...");

            // runs one iteration of the optim algorithm according to the tests results
            optimizationTest.Run();
        }

        protected override void RunOptimizationTests()
        {
            //tries to run a population while possible
            while (!this.StopAllTests)
            {
                //checks maximum number of populations run
                lock (this.locker)
                    if (this.curNumOptimTests >= this.TestsConfig.NumParallelOptimTests)
                        return;

                //gets new population number
                uint optimTestNumber;
                lock (this.locker) optimTestNumber = this.curNumOptimTests++;

                //creates base population members (chromosomes)
                var optimizationTest = this.CreateStochasticOptimTest(optimTestNumber);

                //adds population to cur pop list
                lock (this.locker) this.currentOptimTests.Add(optimizationTest);

                //runs every optimization iteration according to the number of iterations
                while (!this.StopAllTests && !optimizationTest.Terminated &&
                       (optimizationTest.IterationNumber < TestsConfig.MaxIterations))
                {
                    //runs and advances one generation
                    this.RunIteration(optimizationTest);
                }

                //removes optim test from cur test list
                lock (this.locker) this.currentOptimTests.Remove(optimizationTest);

                //stores test improvements
                this.StoreOptimTestStatistics(optimizationTest);

                this.WriteLine(
                    $"{optimizationTest.ID} finished in iteration {optimizationTest.IterationNumber} with improvement threshold of {optimizationTest.FitnessImprovementThreshold}.");
            }
        }

        protected virtual void StoreOptimTestStatistics(IStochasticOptimizationTest optimizationTest)
        {
            lock (this.locker)
            {
                //stores or averages optimization tests' statistics
                if (this.FitnessMaxProgressAvg == null)
                    this.FitnessMaxProgressAvg = optimizationTest.FitnessMaxProgress;
                else
                    this.FitnessMaxProgressAvg.AverageWith(optimizationTest.FitnessMaxProgress);
            }
        }

        #endregion Protected Methods
    }
}