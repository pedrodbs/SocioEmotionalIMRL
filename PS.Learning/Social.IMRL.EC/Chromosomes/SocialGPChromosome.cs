// ------------------------------------------
// <copyright file="SocialGPChromosome.cs" company="Pedro Sequeira">
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
//    Project: Learning.Social.IMRL.EC

//    Last updated: 03/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AForge.Genetic;
using MathNet.Numerics.Random;
using PS.Learning.EvolutionaryComputation;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.Social.Testing;
using PS.Learning.Core.Testing.Config.Parameters;

namespace PS.Learning.Social.IMRL.EC.Chromosomes
{
    [Serializable]
    public class SocialGPChromosome : IGPChromosome, ISocialECChromosome
    {
        private const double SYMMETRY_FACTOR_DEFAULT = 0.7d;
        [NonSerialized] private static readonly Random Rand = new WH2006(true);
        private readonly GPChromosome[] _chromosomes;

        public SocialGPChromosome(uint numAgents, string baseChromosomeExpr)
            : this(numAgents, new GPExpressionChromosome(baseChromosomeExpr))
        {
        }

        public SocialGPChromosome(string[] chromosomeExpressions)
        {
            this.NumAgents = (uint) chromosomeExpressions.Length;
            this._chromosomes = new GPChromosome[this.NumAgents];
            for (var i = 0; i < this.NumAgents; i++)
                this._chromosomes[i] = new GPExpressionChromosome(chromosomeExpressions[i]);
            this.Init();
        }

        public SocialGPChromosome(GPChromosome[] chromosomes)
        {
            this.NumAgents = (uint) chromosomes.Length;
            this._chromosomes = new GPChromosome[this.NumAgents];
            for (var i = 0; i < this.NumAgents; i++)
                this._chromosomes[i] = (GPChromosome) chromosomes[i].Clone();
            this.Population = chromosomes[0].Population;
            this.Init();
        }

        public SocialGPChromosome(uint numAgents, GPChromosome baseChromosome)
        {
            this.NumAgents = numAgents;
            this._chromosomes = new GPChromosome[numAgents];
            for (var i = 0; i < numAgents; i++)
                this._chromosomes[i] = (GPChromosome) baseChromosome.Clone();
            this.Population = baseChromosome.Population;
            this.Init();
        }

        public double SymmetryFactor { get; set; }

        public ITestParameters this[int agentIdx]
        {
            get { return this._chromosomes[agentIdx]; }
            set { if (value is GPChromosome) this._chromosomes[agentIdx] = (GPChromosome) value; }
        }

        public int Index { get; private set; }

        #region IGPChromosome Members

        public uint Length
        {
            get { return (uint) this._chromosomes.Sum(chromosome => chromosome.Length); }
        }

        public uint Depth
        {
            get { return this._chromosomes.Max(chromosome => chromosome.Depth); }
        }

        public virtual HashSet<IGPChromosome> AllCombinations
        {
            get { return Util.GenerateAllCombinations(this); }
        }

        public double Fitness { get; set; }

        public int CompareTo(object obj)
        {
            if (!(obj is SocialGPChromosome)) return -1;
            return this.CompareTo((SocialGPChromosome) obj);
        }

        public int CompareTo(IECChromosome chromosome)
        {
            if (!(chromosome is SocialGPChromosome)) return -1;
            return this.Population == chromosome.Population
                ? -this.Fitness.CompareTo(chromosome.Fitness)
                : -1;
        }

        public void Generate()
        {
            foreach (var chromosome in this._chromosomes)
                chromosome.Generate();
        }

        public IChromosome CreateNew()
        {
            var newChromosomes = new GPChromosome[this.NumAgents];

            //gets symmetry prob
            var symmetricProb = Rand.NextDouble();

            //creates first (base) individual chromosome
            newChromosomes[0] = (GPChromosome) this._chromosomes[0].CreateNew();

            //creates new chromosomes or copies base one based on symmetry prob.
            for (var i = 1; i < this.NumAgents; i++)
                newChromosomes[i] = (GPChromosome) (symmetricProb <= this.SymmetryFactor
                    ? newChromosomes[0].Clone()
                    : this._chromosomes[i].CreateNew());

            return new SocialGPChromosome(newChromosomes)
                   {
                       Population = this.Population,
                       SymmetryFactor = this.SymmetryFactor
                   };
        }

        public IChromosome Clone()
        {
            return new SocialGPChromosome(this._chromosomes)
                   {
                       Population = this.Population,
                       Fitness = this.Fitness,
                       SymmetryFactor = this.SymmetryFactor
                   };
        }

        public void Mutate()
        {
            //gets symmetry prob
            var symmetricProb = Rand.NextDouble();

            //makes the chromosome symmetric or randomly mutates (50% chance) each indiv. chromosome
            if (symmetricProb < this.SymmetryFactor)
                this.MakeSymmetric();
            else
                for (var i = 0; i < this.NumAgents; i++)
                    if (Rand.Next(2) == 0)
                        this._chromosomes[i].Mutate();

            this.Init();
        }

        public void Crossover(IChromosome pair)
        {
            if (!(pair is SocialGPChromosome)) return;
            var socialChromosome = (SocialGPChromosome) pair;

            //for each individual chromosome  
            for (var i = 0; i < this.NumAgents; i++)
            {
                //randomly chooses pairs idxs for crossover
                var sourceIdx = Rand.Next((int) this.NumAgents);
                var pairIdx = Rand.Next((int) this.NumAgents);
                if (Rand.Next(2) == 0)
                {
                    //exchanges (50% chance) one chromosome with the pair
                    var temp = this._chromosomes[sourceIdx];
                    this._chromosomes[sourceIdx] = socialChromosome._chromosomes[pairIdx];
                    socialChromosome._chromosomes[pairIdx] = temp;
                }
                else
                {
                    //crossover (50% chance) one chromosome with the pair
                    this._chromosomes[sourceIdx].Crossover(socialChromosome._chromosomes[pairIdx]);
                }
            }

            this.Init();
        }

        public void Evaluate(IFitnessFunction function)
        {
            this.Fitness = function.Evaluate(this);
        }

        public bool Equals(ITestParameters other)
        {
            return (other is SocialGPChromosome) &&
                   this.ToString().Equals(other.ToString());
        }

        public ECPopulation Population
        {
            get { return this._chromosomes[0].Population; }
            set { Array.ForEach(this._chromosomes, chromosome => chromosome.Population = value); }
        }

        public double CrossoverBalancer
        {
            get { return this._chromosomes[0].CrossoverBalancer; }
            set { Array.ForEach(this._chromosomes, chromosome => chromosome.CrossoverBalancer = value); }
        }

        public double MutationBalancer
        {
            get { return this._chromosomes[0].MutationBalancer; }
            set { Array.ForEach(this._chromosomes, chromosome => chromosome.MutationBalancer = value); }
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public string[] ToValue()
        {
            return this._chromosomes.Select(chromosome => chromosome.ToValue().First()).ToArray();
        }

        public bool FromValue(string[] value)
        {
            if ((value == null) || (value.Length < this.NumAgents)) return false;
            for (var i = 0; i < this._chromosomes.Length; i++)
                this._chromosomes[i] = new GPExpressionChromosome(value[i]);
            return true;
        }

        public string[] Header
        {
            get
            {
                var header = new string[this.NumAgents];
                for (var i = 0; i < this.NumAgents; i++)
                    header[i] = string.Format("chromAg{0}", i);
                return header;
            }
        }

        #endregion

        #region ISocialECChromosome Members

        ITestParameters ISocialTestParameters.this[uint agentIdx]
        {
            get { return this._chromosomes[agentIdx]; }
            set { this._chromosomes[agentIdx] = (GPChromosome) value; }
        }

        public uint NumAgents { get; private set; }

        #endregion

        protected virtual void Init()
        {
            this.Index = (int) (this.Fitness = -1);
            this.SortChromosomes();
            this.SymmetryFactor = SYMMETRY_FACTOR_DEFAULT;
        }

        protected virtual void MakeSymmetric()
        {
            //chooses random indiv. chromosome to be copied
            var baseChromosome = this._chromosomes[Rand.Next((int) this.NumAgents)];

            //makes all chromosomes equal to the chosen one
            for (var i = 0; i < this.NumAgents; i++)
                this._chromosomes[i] = (GPChromosome) baseChromosome.Clone();
        }

        public override string ToString()
        {
            var chromList = this._chromosomes.Select(chromosome => chromosome.ToString()).ToList();
            chromList.Sort();
            var expression = chromList.Aggregate(
                string.Empty, (current, value) => string.Format("{0}{1}_", current, value));
            expression = expression.Remove(expression.Length - 1, 1);
            return expression;
        }

        public string ToScreenString()
        {
            var chromList = this._chromosomes.Select(chromosome => chromosome.ToScreenString()).ToList();
            chromList.Sort();
            var expression = chromList.Aggregate(
                string.Empty, (current, value) => string.Format("{0}{1}|", current, value));
            expression = expression.Remove(expression.Length - 1, 1);
            return expression;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is SocialGPChromosome) && this.Equals((SocialGPChromosome) obj);
        }

        protected virtual void SortChromosomes()
        {
            //rearranges chromosomes to be sorted by text
            var sortedChromosomes = new List<GPChromosome>();
            foreach (var chromosome in this._chromosomes)
            {
                var index = 0;
                for (; index < sortedChromosomes.Count; index++)
                    if (String.Compare(chromosome.ToString(), sortedChromosomes[index].ToString(),
                        StringComparison.Ordinal) < 0)
                        break;
                sortedChromosomes.Insert(index, chromosome);
            }
            sortedChromosomes.CopyTo(this._chromosomes);
        }
    }
}