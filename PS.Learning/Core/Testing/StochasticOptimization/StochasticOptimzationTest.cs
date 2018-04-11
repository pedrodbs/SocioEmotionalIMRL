// ------------------------------------------
// <copyright file="StochasticOptimzationTest.cs" company="Pedro Sequeira">
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
//    Last updated: 2/14/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using PS.Learning.Core.Testing.Config;
using PS.Utilities.Math;
using PS.Utilities.Core;

namespace PS.Learning.Core.Testing.StochasticOptimization
{
    public abstract class StochasticOptimzationTest : IStochasticOptimizationTest
    {
        protected const uint MINIMUM_IMPROV_THRESHOLD = 2;
        protected const uint DEFAULT_IMPROV_THRESHOLD = 10;
        protected const double IMPROV_THRESHOLD_PENALTY_FACTOR = 4d;
        protected readonly IStochasticOptimTestsConfig testsConfig;
        protected double curFitImprovThreshold;
        protected double lastFitnessMax = double.MinValue;
        protected uint numGenWithoutImprov;

        protected StochasticOptimzationTest(string id, IStochasticOptimTestsConfig testsConfig)
        {
            this.ID = id;
            this.testsConfig = testsConfig;
            this.FitnessMaxProgress = new StatisticalQuantity(testsConfig.MaxIterations);
            this.curFitImprovThreshold = testsConfig.FitnessImprovementThreshold;
        }

        #region IStochasticOptimizationTest Members

        public double FitnessImprovementThreshold
        {
            get { return (uint) System.Math.Max(this.curFitImprovThreshold, MINIMUM_IMPROV_THRESHOLD); }
        }

        public StatisticalQuantity FitnessMaxProgress { get; private set; }

        public string ID { get; protected set; }

        public int IterationNumber { get; set; }

        public double MaxFitness { get; protected set; }

        public bool Terminated { get; set; }

        public long MemoryUsage
        {
            get { return 0; }
        }

        public TimeSpan TestSpeed
        {
            get { return new TimeSpan(); }
        }

        public string FilePath { get; set; }

        public LogWriter LogWriter { get; set; }

        public bool Run()
        {
            //checks terminated
            if (this.Terminated) return true;

            //optimizes fitness (one step)
            this.RunIteration();

            //checks fit improv
            this.CheckFitnessImprovement();

            //increments generation and checks improvements on fitness
            this.IterationNumber++;

            return false;
        }

        public void PrintResults()
        {
        }

        public virtual void Dispose()
        {
        }

        #endregion

        protected abstract void RunIteration();

        protected virtual void CheckFitnessImprovement()
        {
            //compares with last max fitness
            if (this.MaxFitness > this.lastFitnessMax)
            {
                //updates improv threshold accordingly (greater interval, even smaller threshold)
                this.curFitImprovThreshold -= this.numGenWithoutImprov/IMPROV_THRESHOLD_PENALTY_FACTOR;

                //resets counter and updates max fitness
                this.numGenWithoutImprov = 0;
                this.lastFitnessMax = this.MaxFitness;
            }
            else if (++this.numGenWithoutImprov >= this.FitnessImprovementThreshold)
            {
                //threshold passed
                this.Terminated = true;
            }

            //updates max fitness quantity
            this.FitnessMaxProgress.Value = this.MaxFitness;
        }
    }
}