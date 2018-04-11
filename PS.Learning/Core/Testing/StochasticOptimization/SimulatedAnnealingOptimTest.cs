// ------------------------------------------
// <copyright file="SimulatedAnnealingOptimTest.cs" company="Pedro Sequeira">
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
//    Last updated: 02/14/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using MathNet.Numerics.Random;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.MultipleTests;

namespace PS.Learning.Core.Testing.StochasticOptimization
{
    public abstract class SimulatedAnnealingOptimTest : StochasticOptimzationTest
    {
        protected static readonly Random Random = new WH2006();
        protected readonly IOptimizationTestFactory optimizationTestFactory;
        protected readonly TestMeasureList testMeasures;
        protected double temperature;

        protected SimulatedAnnealingOptimTest(
            string id, IOptimizationTestFactory optimizationTestFactory, TestMeasureList testMeasures) :
                base(id, (ISimAnnTestConfig) optimizationTestFactory.Scenario.TestsConfig)
        {
            this.optimizationTestFactory = optimizationTestFactory;
            this.testMeasures = testMeasures;
        }

        protected ISimAnnTestConfig TestsConfig
        {
            get { return (ISimAnnTestConfig) this.testsConfig; }
        }

        protected override void RunIteration()
        {
            this.IterationNumber = 0;
            ITestParameters currentParam = null, bestParam = null;
            this.temperature = this.TestsConfig.InitialTemperature;
            this.MaxFitness = double.MinValue;

            //while the temperature did not reach threshold
            while (!this.Terminated && (this.temperature > this.TestsConfig.TempThreshold))
            {
                this.IterationNumber++;

                //get the next random permutation of values and compute the best 
                ITestParameters bestNeighbour;
                var bestNeighbourValue = this.ComputeBestNeighbour(currentParam, out bestNeighbour);

                if (bestNeighbourValue > this.MaxFitness)
                {
                    //accept and assign the new args if the value is max 
                    this.MaxFitness = bestNeighbourValue;
                    bestParam = currentParam = bestNeighbour;
                }
                else if (Random.NextDouble() < this.ChangeProbability(bestNeighbourValue))
                {
                    //if the new value is worse accept it but with a probability level
                    //less than E to the power -delta/temperature.
                    currentParam = bestNeighbour;
                }
                else
                {
                    currentParam = bestParam;
                }

                //cooling process on every iteration
                temperature *= this.TestsConfig.Alpha;
            }
        }

        protected double ComputeBestNeighbour(ITestParameters current, out ITestParameters bestNeighbour)
        {
            bestNeighbour = null;
            var bestNeighbourValue = double.MinValue;

            for (var i = 0; i < this.TestsConfig.NumTestsPerIteration; i++)
            {
                var neighbour = this.ComputeNeighbour(current);
                var neighbourValue = this.ComputeFitness(neighbour);

                if (!(neighbourValue > bestNeighbourValue)) continue;
                bestNeighbourValue = neighbourValue;
                bestNeighbour = neighbour;
            }
            return bestNeighbourValue;
        }

        protected abstract ITestParameters ComputeNeighbour(ITestParameters currentParam);

        protected virtual double ChangeProbability(double nextValue)
        {
            var diff = nextValue - this.MaxFitness;
            //return Math.Exp((diff * Math.Log(iteration + 1)) / 10);
            return Math.Exp(diff/this.temperature);
        }

        protected virtual double ComputeFitness(ITestParameters testParameters)
        {
            //checks whether test has been already executed, updating information
            if (!this.testMeasures.Contains(testParameters))
            {
                //create new test with given test params
                var test = this.optimizationTestFactory.CreateTest(testParameters);

                //runs test
                test.Run();

                //adds test results to measures list
                this.testMeasures.Add(testParameters, this.optimizationTestFactory.CreateTestMeasure(test));
            }

            //returns test results
            return this.testMeasures.GetTestMeasure(testParameters).Value;
        }
    }
}