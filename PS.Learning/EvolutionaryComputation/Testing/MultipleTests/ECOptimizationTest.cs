// ------------------------------------------
// <copyright file="ECOptimizationTest.cs" company="Pedro Sequeira">
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

//    Last updated: 02/14/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using AForge.Genetic;
using PS.Learning.Core.Testing.StochasticOptimization;
using PS.Utilities.Math;

namespace PS.Learning.EvolutionaryComputation.Testing.MultipleTests
{
    public class ECOptimizationTest : StochasticOptimzationTest
    {
        protected readonly ECPopulation population;

        public ECOptimizationTest(
            string id, IECTestsConfig testsConfig, IECChromosome ancestor, FitnessFunction fitnessFunction)
            : base(id, testsConfig)
        {
            this.population = new ECPopulation(testsConfig, ancestor, fitnessFunction);
            this.RandomProportionProgress = new StatisticalQuantity(testsConfig.MaxIterations);
        }

        public StatisticalQuantity RandomProportionProgress { get; private set; }

        protected override void RunIteration()
        {
            //updates generation number
            this.population.GenerationNumber = this.IterationNumber;

            //amount of top chromosomes in the new population to keep
            var steadyStateAmount = (int) (((IECTestsConfig) this.testsConfig).SteadyStatePortion*this.population.Size);

            //keep top chromosomes before running epoch
            var topChromosomes = this.population.GetTopChromosomes(steadyStateAmount);

            //shuffles elements
            this.population.ShuffleSort();

            //gets random proportion
            this.UpdateRandomProportion();

            //run epoch
            this.population.Crossover();
            this.population.Mutate();
            this.population.Selection();

            //add top chromosomes
            this.population.Chromosomes.AddRange(topChromosomes);
        }

        protected override void CheckFitnessImprovement()
        {
            //updates max fitness based on population
            this.population.UpdateBestChromosome();
            this.MaxFitness = this.population.FitnessMax;

            base.CheckFitnessImprovement();
        }

        protected virtual void UpdateRandomProportion()
        {
            //calculates num of distinct chromosomes
            var distinctChromosomes = new HashSet<IChromosome>();
            foreach (var chromosome in this.population.Chromosomes)
                if (!distinctChromosomes.Contains(chromosome))
                    distinctChromosomes.Add(chromosome);

            //random amount proportional to the number of diff chromosomes
            var genFactor = (double) this.IterationNumber/this.testsConfig.MaxIterations;
            var chromFactor = (double) distinctChromosomes.Count/this.population.Size;
            this.RandomProportionProgress.Value =
                (this.population.MutationRate = this.population.RandomSelectionPortion =
                    (1d - chromFactor)*(1d - (genFactor*genFactor)));
        }
    }
}