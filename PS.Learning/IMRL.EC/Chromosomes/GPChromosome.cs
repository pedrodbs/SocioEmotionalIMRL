// ------------------------------------------
// <copyright file="GPChromosome.cs" company="Pedro Sequeira">
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

//    Last updated: 03/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using AForge.Genetic;
using PS.Learning.EvolutionaryComputation;
using PS.Utilities.Core.Conversion;

namespace PS.Learning.IMRL.EC.Chromosomes
{
    [Serializable]
    public class GPChromosome : ECChromosome, IGPChromosome
    {
        protected GPChromosome(IChromosome baseChromosome, ECPopulation population) : base(population)
        {
            this.BaseChromosome = baseChromosome;
        }

        public GPChromosome(IGPGene ancestor) : base(null)
        {
            this.BaseChromosome = new GPTreeChromosome(ancestor);
        }

        #region IGPChromosome Members

        public IChromosome BaseChromosome { get; set; }

        public virtual HashSet<IGPChromosome> AllCombinations
        {
            get { return Util.GenerateAllCombinations(this); }
        }

        public uint Length
        {
            get
            {
                var originalExpression = this.ToString();
                return string.IsNullOrWhiteSpace(originalExpression)
                    ? 0
                    : (uint) Util.GetNodeStack(originalExpression).Count;
            }
        }

        public uint Depth
        {
            get { return Util.GetDepth(this); }
        }

        public override int CompareTo(object obj)
        {
            if (!(obj is GPChromosome)) return -1;
            return this.CompareTo((GPChromosome) obj);
        }

        public override int CompareTo(IECChromosome chromosome)
        {
            if (!(chromosome is GPChromosome)) return -1;
            return
                this.Population == chromosome.Population
                    ? this.Length.CompareTo(((GPChromosome) chromosome).Length)
                    : -1;
        }

        public override void Generate()
        {
            this.BaseChromosome.Generate();
        }

        public override IChromosome CreateNew()
        {
            var newChromosome = this.BaseChromosome.CreateNew();
            return new GPChromosome(newChromosome, this.Population);
        }

        public override IChromosome Clone()
        {
            return new GPChromosome(this.BaseChromosome.Clone(), this.Population);
        }

        public override bool FromValue(string[] value)
        {
            IChromosome baseChromosome;
            if (!ValueConverter.Convert(out baseChromosome, value[0])) return false;
            this.BaseChromosome = baseChromosome;
            return true;
        }

        public override string ToScreenString()
        {
            return this.BaseChromosome?.ToString();
        }

        public override void Mutate()
        {
            this.BaseChromosome.Mutate();
        }

        public override void Crossover(IChromosome pair)
        {
            if (pair is GPChromosome)
                this.BaseChromosome.Crossover(((GPChromosome) pair).BaseChromosome);
        }

        #endregion

        public override string ToString()
        {
            return this.BaseChromosome.ToString();
        }

    }
}