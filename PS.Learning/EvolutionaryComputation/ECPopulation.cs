// ------------------------------------------
// <copyright file="ECPopulation.cs" company="Pedro Sequeira">
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

//    Last updated: 2/6/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Collections.Generic;
using AForge.Genetic;
using MathNet.Numerics.Random;
using PS.Learning.EvolutionaryComputation.Testing;

namespace PS.Learning.EvolutionaryComputation
{
    [Serializable]
    public class ECPopulation : Population
    {
        protected const double DEFAULT_CROSSOVER_RATE = 1; //all parents are paired
        protected const double DEFAULT_MUTATION_RATE = 0.1;
        protected readonly IECTestsConfig testsConfig;
        protected static Random rand = new WH2006(RandomSeed.Robust());

        public ECPopulation(
            IECTestsConfig testsConfig, IECChromosome ancestor, FitnessFunction fitnessFunction) :
                base(testsConfig.NumTestsPerIteration, ancestor, fitnessFunction, testsConfig.SelectionMethod)
        {
            this.testsConfig = testsConfig;

            //sets population ref to all chromosomes
            ancestor.Population = this;
            for (var i = 0; i < this.Size; i++)
                ((IECChromosome) this[i]).Population = this;

            //default values
            this.CrossoverRate = DEFAULT_CROSSOVER_RATE;
            this.MutationRate = DEFAULT_MUTATION_RATE;
            this.RandomSelectionPortion = testsConfig.RandomSelectionPortion;
        }

        //public List<IChromosome> Chromosomes
        //{
        //    get { return this.population; }
        //}

        public int GenerationNumber { get; set; }

        public override void Selection()
        {
            //amount of random chromosomes in the new population
            var randomAmount = (int) (this.RandomSelectionPortion*this.Size);

            //amount of top chromosomes in the new population to keep
            var steadyStateAmount = (int) (this.testsConfig.SteadyStatePortion*this.Size);

            //do selection
            this.SelectionMethod.ApplySelection(this.Chromosomes, this.Size - randomAmount - steadyStateAmount);

            //add random chromosomes
            this.AddRandomChromosomes(randomAmount);
        }

        public virtual IEnumerable<IChromosome> GetTopChromosomes(int steadyStateAmount)
        {
            // sort chromosomes
            this.Chromosomes.Sort();

            // returns top chromosomes
            var topChromosomes = new IChromosome[steadyStateAmount];
            this.Chromosomes.CopyTo(0, topChromosomes, 0, steadyStateAmount);
            return topChromosomes;
        }

        protected virtual void AddRandomChromosomes(int randomAmount)
        {
            if (randomAmount <= 0) return;

            var ancestor = this.Chromosomes[0];

            for (var i = 0; i < randomAmount; i++)
            {
                // create new chromosome
                var c = ancestor.CreateNew();
                // calculate it's fitness
                c.Evaluate(this.FitnessFunction);
                // add it to population
                this.Chromosomes.Add(c);
            }
        }

        public void ShuffleSort()
        {
            this.Chromosomes.Sort((a, b) => rand.Next(-1, 2));
        }

        public void UpdateBestChromosome()
        {
            this.FindBestChromosome();
        }
    }
}