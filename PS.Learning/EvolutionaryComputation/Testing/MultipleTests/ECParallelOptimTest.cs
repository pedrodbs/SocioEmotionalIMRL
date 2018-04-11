// ------------------------------------------
// <copyright file="ECParallelOptimTest.cs" company="Pedro Sequeira">
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
//    Project: Learning.EvolutionaryComputation

//    Last updated: 03/10/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Core.Testing.StochasticOptimization;
using PS.Utilities.Math;
using System.IO;

namespace PS.Learning.EvolutionaryComputation.Testing.MultipleTests
{
    public class ECParallelOptimTest : StochasticParallelOptimTest
    {
        #region Private Fields

        private const string TEST_ID = "EvoOptimization";

        #endregion Private Fields

        #region Public Constructors

        public ECParallelOptimTest(IOptimizationTestFactory optimizationTestFactory)
            : base(optimizationTestFactory)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public StatisticalQuantity RandomProportionProgressAvg { get; protected set; }

        public override string TestID
        {
            get { return TEST_ID; }
        }

        #endregion Public Properties

        #region Protected Properties

        protected new IECTestsConfig TestsConfig
        {
            get { return base.TestsConfig as IECTestsConfig; }
        }

        #endregion Protected Properties

        #region Public Methods

        public override void Dispose()
        {
            base.Dispose();
            this.RandomProportionProgressAvg.Dispose();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override IStochasticOptimizationTest CreateStochasticOptimTest(uint optimTestNumber)
        {
            this.WriteLine(@"__________________________________________");
            this.WriteLine(string.Format("Creating population {0}...", optimTestNumber));

            return new ECOptimizationTest(string.Format("Population {0}", optimTestNumber),
                this.TestsConfig, this.TestsConfig.CreateBaseChromosome(), this.CreateFitnessFunction());
        }

        protected override void PrintTestPerformanceResults()
        {
            base.PrintTestPerformanceResults();
            this.RandomProportionProgressAvg.PrintStatisticsToCSV(
                string.Format("{0}{1}RandomProportionAvg.csv", this.FilePath, Path.DirectorySeparatorChar));
        }

        protected override void StoreOptimTestStatistics(IStochasticOptimizationTest optimizationTest)
        {
            base.StoreOptimTestStatistics(optimizationTest);

            lock (this.locker)
            {
                var ecOptimizationTest = (ECOptimizationTest)optimizationTest;

                //stores or averages populations' statistics
                if (this.RandomProportionProgressAvg == null)
                    this.RandomProportionProgressAvg = ecOptimizationTest.RandomProportionProgress;
                else
                    this.RandomProportionProgressAvg.AverageWith(ecOptimizationTest.RandomProportionProgress);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private FitnessFunction CreateFitnessFunction()
        {
            //create fitness function
            return new FitnessFunction(this.OptimizationTestFactory, (ECTestMeasureList)this.TestMeasures)
            {
                LogWriter = this.LogWriter
            };
        }

        #endregion Private Methods
    }
}