// ------------------------------------------
// <copyright file="GPSimplifierFitnessTest.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL.EC

//    Last updated: 03/10/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using System.IO;
using PS.Learning.EvolutionaryComputation.Testing;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Core.Testing.SingleTests;

namespace PS.Learning.IMRL.EC.Testing.MultipleTests
{
    public class GPSimplifierFitnessTest : ListFitnessTest
    {
        private const string SIMPLIFY_TEST_ID = "ExpressionSimplify";
        private const double MIN_P_VALUE = 0.2;

        #region Constructors

        public GPSimplifierFitnessTest(IGPSimplifierOptimizationTestFactory optimizationTestFactory)
            : base(optimizationTestFactory)
        {
            this.AddSpecialTestParams = false;
        }

        #endregion

        #region Protected Methods

        protected override void RunOptimizationTests()
        {
            //loop until chromosomes end or external stop request
            while (!this.StopAllTests)
            {
                IGPChromosome nextTestParams;
                ECTestMeasure originalGPTestMeasure;

                //lock on to take next chromosome from list
                lock (this.locker)
                {
                    //gets next test if possible, or else returns (ends thread)
                    if (this.currentTestIdx >= this.testParameters.Count) return;
                    if (this.testParameters.Count == 0) continue;
                    nextTestParams = (IGPChromosome) this.testParameters[this.currentTestIdx++];

                    //gets history associated with this simplified chromosome and compares lengths
                    originalGPTestMeasure =
                        this.OptimizationTestFactory.SimplifiedParamMeasures[nextTestParams];
                    var minHistLength = this.OptimizationTestFactory.MinLengthChromosome[originalGPTestMeasure];

                    //if new length is higher, ignore new chromosome as we have found a simpler version
                    if (nextTestParams.Length > minHistLength) continue;
                }

                //runs test
                this.RunSingleTest(nextTestParams);

                lock (this.locker)
                {
                    //calculates chromosomes' fitness diff. statistical signif.
                    var nextSimpChromosomeMeasure = (GPTestMeasure) this.TestMeasures.GetTestMeasure(nextTestParams);
                    var statisticallyDifferent = nextSimpChromosomeMeasure.IsStatiscallyDifferentFrom(
                        originalGPTestMeasure, (int) this.NumSimulations, MIN_P_VALUE);

                    //checks whether results are better than original expression or statistically insignificant
                    if ((nextSimpChromosomeMeasure.Value < originalGPTestMeasure.Value) ||
                        !statisticallyDifferent) continue;

                    //tests whether the length is minimal in relation to the original history's simplifications
                    if (this.OptimizationTestFactory.MinLengthChromosome[originalGPTestMeasure] >
                        ((IGPChromosome) nextSimpChromosomeMeasure.Parameters).Length)
                        this.OptimizationTestFactory.SimplifiedChromosomesMeasures[originalGPTestMeasure].Clear();

                    //sets minimal length and adds history to minimal list
                    this.OptimizationTestFactory.MinLengthChromosome[originalGPTestMeasure] =
                        ((IGPChromosome) nextSimpChromosomeMeasure.Parameters).Length;
                    this.OptimizationTestFactory.SimplifiedChromosomesMeasures[originalGPTestMeasure].Add(
                        nextSimpChromosomeMeasure);
                }
            }
        }

        protected override void PrepareSingleTest(FitnessTest test)
        {
            base.PrepareSingleTest(test);
            lock (this.locker)
                test.LogStatistics = this.TestsConfig.GraphicsEnabled;
        }

        #endregion

        #region Properties

        public override string TestID
        {
            get { return SIMPLIFY_TEST_ID; }
        }

        public new IGPSimplifierOptimizationTestFactory OptimizationTestFactory
        {
            get { return base.OptimizationTestFactory as IGPSimplifierOptimizationTestFactory; }
        }

        #endregion

        #region Public Methods

        protected override void DetermineParametersList(bool readFromFile = false)
        {
            this.testParameters = this.OptimizationTestFactory.DetermineChromosomeParameters(
                (ECTestMeasureList) this.TestMeasures);
        }

        public override void PrintResults()
        {
            base.PrintResults();

            //add histories of simplified versions of chromosomes if possible
            var simplifiedChromosomeParams = new HashSet<ITestParameters>();
            foreach (var simplifiedChromosomeHistories in this.OptimizationTestFactory.SimplifiedChromosomesMeasures)
            {
                //if no simplified version exists, adds original chromosome history
                if (simplifiedChromosomeHistories.Value.Count == 0)
                    simplifiedChromosomeParams.Add(simplifiedChromosomeHistories.Key.Parameters);
                else
                    foreach (var simplifiedChromosomeHistory in simplifiedChromosomeHistories.Value)
                        simplifiedChromosomeParams.Add(simplifiedChromosomeHistory.Parameters);
            }

            //only keep simplified chromosome histories
            this.TestMeasures.ClearExcept(simplifiedChromosomeParams);
            this.TestMeasures.PrintToFile(string.Format("{0}{1}{2}{3}.csv",
                this.FilePath, Path.DirectorySeparatorChar, SIMPLIFY_TEST_ID, TestsConfig.TestMeasuresName));
        }

        #endregion
    }
}