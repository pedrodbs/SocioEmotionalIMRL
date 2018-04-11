// ------------------------------------------
// <copyright file="FitnessFunction.cs" company="Pedro Sequeira">
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

//    Last updated: 02/06/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using AForge.Genetic;
using PS.Learning.EvolutionaryComputation.Testing;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Utilities.Core;

namespace PS.Learning.EvolutionaryComputation
{
    public class FitnessFunction : IFitnessFunction
    {
        #region Constructors

        public FitnessFunction(
            IOptimizationTestFactory optimizationTestFactory, ECTestMeasureList testMeasureList)
        {
            this.optimizationTestFactory = optimizationTestFactory;
            this.testMeasureList = testMeasureList;
        }

        #endregion

        #region IFitnessFunction Members

        #region Public Methods

        public virtual double Evaluate(IChromosome chromosome)
        {
            //checks argument
            if (!(chromosome is IECChromosome))
                throw new ArgumentException(@"Parameter is not IECChromosome", nameof(chromosome));

            var chromosomeParam = (IECChromosome) chromosome;

            //checks whether test has been already executed, updating information
            if (this.testMeasureList.Contains(chromosomeParam))
                this.testMeasureList.UpdateTestMeasure(chromosomeParam);
            else
            {
                //create new test with given chromosome params
                var test = this.optimizationTestFactory.CreateTest(chromosomeParam);

                //runs test
                test.Run();

                //prints test results
                if (this.PrintResults) test.PrintResults();

                this.testMeasureList.Add(chromosomeParam, this.optimizationTestFactory.CreateTestMeasure(test));
            }

            var testMeasure = (ECTestMeasure) this.testMeasureList.GetTestMeasure(chromosomeParam);

            this.WriteLine($"\"{testMeasure.ID}\": {testMeasure.Value:0.00}±{testMeasure.StdDev:0.00}");
            return testMeasure.Value;
        }

        #endregion

        #endregion

        #region Protected Methods

        protected void WriteLine(string line)
        {
            this.LogWriter?.WriteLine(line);
        }

        #endregion

        #region Fields

        protected readonly IOptimizationTestFactory optimizationTestFactory;
        protected readonly ECTestMeasureList testMeasureList;

        #endregion

        #region Properties

        public LogWriter LogWriter { get; set; }

        public bool PrintResults { get; set; }

        #endregion
    }
}