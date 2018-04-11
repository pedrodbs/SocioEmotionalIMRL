// ------------------------------------------
// <copyright file="ArrayChromosome.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.IMRL.EC

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using AForge.Genetic;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.EvolutionaryComputation;
using PS.Utilities.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PS.Learning.IMRL.EC.Chromosomes
{
    [Serializable]
    public class ArrayChromosome : ECChromosome, IArrayChromosome
    {
        #region Protected Fields

        [NonSerialized]
        protected static readonly Random Rand = new WH2006(true);

        protected ArrayParameter arrayParameter;
        protected IContinuousDistribution[] randomGenerators;

        #endregion Protected Fields

        #region Public Constructors

        public ArrayChromosome(ArrayChromosome source) : base(source.Population)
        {
            this.randomGenerators = source.randomGenerators;
            this.MutationBalancer = source.MutationBalancer;
            this.CrossoverBalancer = source.CrossoverBalancer;
            this.arrayParameter = (ArrayParameter)source.arrayParameter.Clone();
        }

        public ArrayChromosome(ArrayParameter arrayParameter)
            : this(null, arrayParameter)
        {
        }

        public ArrayChromosome(ECPopulation population, ArrayParameter arrayParameter)
            : base(population)
        {
            this.MutationBalancer = 0.5;
            this.CrossoverBalancer = 0.5;
            this.randomGenerators = this.CreateRandomGenerators(arrayParameter.Domains);
            this.arrayParameter = (ArrayParameter)arrayParameter.Clone();
            this.Generate();
        }

        #endregion Public Constructors

        #region Public Properties

        public double AbsoulteSum
        {
            get { return this.arrayParameter.AbsoulteSum; }
        }

        public double[] Array
        {
            get { return this.arrayParameter.Array; }
        }

        public Range<double>[] Domains
        {
            get { return this.arrayParameter.Domains; }
        }

        public override string[] Header
        {
            get
            {
                var length = this.Array.Length;
                var header = new string[length];
                for (var i = 0; i < length; i++)
                    header[i] = string.Format("param{0}", i);
                return header;
            }
        }

        public uint Length
        {
            get { return this.arrayParameter.Length; }
        }

        public uint NumDecimalPlaces
        {
            get { return this.arrayParameter.NumDecimalPlaces; }
            set { this.arrayParameter.NumDecimalPlaces = value; }
        }

        public double Sum
        {
            get { return this.arrayParameter.Sum; }
        }

        #endregion Public Properties

        #region Public Indexers

        public double this[int paramIdx]
        {
            get { return this.arrayParameter[paramIdx]; }
            set { this.arrayParameter[paramIdx] = value; }
        }

        #endregion Public Indexers

        #region Public Methods

        public override IChromosome Clone()
        {
            return new ArrayChromosome(this);
        }

        public override void Crossover(IChromosome pair)
        {
            if (!(pair is ArrayChromosome)) return;
            var p = (ArrayChromosome)pair;

            // check for correct pair
            var length = this.arrayParameter.Length;
            if (p.arrayParameter.Length != length) return;

            if (Rand.NextDouble() < this.CrossoverBalancer)
                this.PointCrossover(p);
            else
                this.ApproximateCrossover(p);

            //normalizes and rounds after crossover
            this.arrayParameter.NormalizeUnitSum();
            this.arrayParameter.Round();
            p.arrayParameter.NormalizeUnitSum();
            p.arrayParameter.Round();
        }

        public override bool FromValue(string[] value)
        {
            return this.arrayParameter.FromValue(value);
        }

        public override void Generate()
        {
            for (var i = 0; i < this.arrayParameter.Length; i++)
                this.arrayParameter[i] = randomGenerators[i].Sample();

            this.arrayParameter.NormalizeUnitSum();
            this.arrayParameter.Round();
        }

        public IEnumerator<double> GetEnumerator()
        {
            return this.arrayParameter.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override void Mutate()
        {
            var mutationGene = Rand.Next((int)this.arrayParameter.Length);

            if (Rand.NextDouble() < this.MutationBalancer)
            {
                var next = this.randomGenerators[mutationGene].Sample();
                this.arrayParameter[mutationGene] *= (Rand.NextDouble() < 0.5) ? next : 1 / next;
            }
            else
            {
                var next = this.randomGenerators[mutationGene].Sample();
                this.arrayParameter[mutationGene] += (Rand.NextDouble() < 0.5) ? next : -next;
            }

            this.arrayParameter.NormalizeUnitSum();
            this.arrayParameter.Round();
        }

        public void NormalizeSum()
        {
            this.arrayParameter.NormalizeSum();
        }

        public void NormalizeUnitSum()
        {
            this.arrayParameter.NormalizeUnitSum();
        }

        public void NormalizeVector()
        {
            this.arrayParameter.NormalizeVector();
        }

        public void Round()
        {
            this.arrayParameter.Round();
        }

        public void SetMidDomainValues()
        {
            this.arrayParameter.SetMidDomainValues();
        }

        public override string ToString()
        {
            return this.arrayParameter.ToString();
        }

        public override string ToScreenString()
        {
            return this.arrayParameter.ToScreenString();
        }

        public override string[] ToValue()
        {
            return this.arrayParameter.ToValue();
        }

        #endregion Public Methods

        #region Protected Methods

        protected void ApproximateCrossover(ArrayChromosome p)
        {
            var pairVal = p.arrayParameter;
            var factor = Rand.NextDouble();
            if (Rand.Next(2) == 0)
                factor = -factor;

            for (var i = 0; i < this.Length; i++)
            {
                var portion = (this.arrayParameter[i] - pairVal[i]) * factor;
                this.arrayParameter[i] -= portion;
                pairVal[i] += portion;
            }
        }

        protected IContinuousDistribution[] CreateRandomGenerators(Range<double>[] ranges)
        {
            var randomGens = new IContinuousDistribution[ranges.Length];
            for (var i = 0; i < ranges.Length; i++)
                randomGens[i] = new ContinuousUniform(ranges[i].min, ranges[i].max);
            return randomGens;
        }

        protected void PointCrossover(ArrayChromosome p)
        {
            // crossover point
            var crossOverPoint = Rand.Next((int)(this.Length - 1)) + 1;
            var crossOverLength = this.Length - crossOverPoint;

            // temporary array
            var temp = new double[crossOverLength];

            System.Array.Copy(this.arrayParameter, crossOverPoint, temp, 0, crossOverLength);
            System.Array.Copy(p.arrayParameter, crossOverPoint, this.arrayParameter, crossOverPoint,
                crossOverLength);
            System.Array.Copy(temp, 0, p.arrayParameter, crossOverPoint, crossOverLength);
        }

        #endregion Protected Methods
    }
}