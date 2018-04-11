// ------------------------------------------
// <copyright file="ECChromosome.cs" company="Pedro Sequeira">
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

//    Last updated: 01/11/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using AForge.Genetic;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Utilities.Core;

namespace PS.Learning.EvolutionaryComputation
{
    [Serializable]
    public abstract class ECChromosome : IECChromosome
    {
        [NonSerialized] private ECPopulation _population;

        protected ECChromosome(ECPopulation population)
        {
            this.Population = population;
        }

        #region IECChromosome Members

        public double Fitness { get; set; }

        public ECPopulation Population
        {
            get { return this._population; }
            set { this._population = value; }
        }

        public double CrossoverBalancer { get; set; }
        public double MutationBalancer { get; set; }
        public abstract void Generate();

        public virtual IChromosome CreateNew()
        {
            var chromosome = this.Clone();
            chromosome.Generate();
            return chromosome;
        }

        public abstract IChromosome Clone();
        public abstract void Mutate();
        public abstract void Crossover(IChromosome pair);

        public void Evaluate(IFitnessFunction function)
        {
            this.Fitness = function.Evaluate(this);
        }

        public bool Equals(ITestParameters other)
        {
            return (other is ECChromosome) &&
                   this.ToString().Equals(other.ToString());
        }

        public virtual int CompareTo(object obj)
        {
            if (!(obj is ECChromosome)) return -1;
            return CompareTo((ECChromosome) obj);
        }

        public virtual int CompareTo(IECChromosome chromosome)
        {
            if (!(chromosome is ECChromosome)) return -1;
            return this.Population == chromosome.Population ? 0 : -1;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public virtual string[] ToValue()
        {
            return new[] {this.ToString().ToLiteral()};
        }

        public abstract bool FromValue(string[] value);

        public virtual string[] Header
        {
            get { return new[] {"Chromosome"}; }
        }

        public abstract string ToScreenString();

        #endregion

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is ECChromosome) && this.Equals((ECChromosome) obj);
        }
    }
}