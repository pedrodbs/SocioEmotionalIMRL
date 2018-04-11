// ------------------------------------------
// <copyright file="SocialArrayChromosome.cs" company="Pedro Sequeira">
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

//    Last updated: 02/12/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Linq;
using AForge.Genetic;
using PS.Learning.EvolutionaryComputation;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.Social.Testing;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Utilities.Collections;

namespace PS.Learning.Social.IMRL.EC.Chromosomes
{
    public class SocialArrayChromosome : ECChromosome, ISocialECChromosome, ISocialArrayParameter
    {
        private readonly ArrayChromosome[] _chromosomes;

        public SocialArrayChromosome(ArrayParameter[] parameters)
            : this(null, parameters)
        {
        }

        public SocialArrayChromosome(ECPopulation population, ArrayParameter[] parameters)
            : base(population)
        {
            this.NumAgents = (uint) parameters.Length;
            this._chromosomes = new ArrayChromosome[this.NumAgents];
            for (var i = 0; i < this.NumAgents; i++)
                this._chromosomes[i] = new ArrayChromosome(population, parameters[i]);
        }

        public SocialArrayChromosome(uint numAgents, ArrayParameter baseParameters)
            : this(null, numAgents, baseParameters)
        {
        }

        public SocialArrayChromosome(ECPopulation population, uint numAgents, ArrayParameter baseParameters)
            : base(population)
        {
            this.NumAgents = numAgents;
            this._chromosomes = new ArrayChromosome[this.NumAgents];
            for (var i = 0; i < this.NumAgents; i++)
                this._chromosomes[i] = new ArrayChromosome(population, baseParameters);
        }

        private SocialArrayChromosome(SocialArrayChromosome baseChromosome) : base(baseChromosome.Population)
        {
            this.NumAgents = baseChromosome.NumAgents;
            this._chromosomes = new ArrayChromosome[this.NumAgents];
            for (var i = 0; i < this.NumAgents; i++)
                this._chromosomes[i] = new ArrayChromosome(baseChromosome._chromosomes[i]);
        }

        #region ISocialArrayParameter Members

        public uint NumDecimalPlaces
        {
            get { return this._chromosomes[0].NumDecimalPlaces; }
            set
            {
                foreach (var chromosome in this._chromosomes)
                    chromosome.NumDecimalPlaces = value;
            }
        }

        public uint Length
        {
            get { return this._chromosomes[0].Length; }
        }

        public void NormalizeVector()
        {
            foreach (var chromosome in this._chromosomes)
                chromosome.NormalizeVector();
        }

        public void NormalizeSum()
        {
            foreach (var chromosome in this._chromosomes)
                chromosome.NormalizeSum();
        }

        public void NormalizeUnitSum()
        {
            foreach (var chromosome in this._chromosomes)
                chromosome.NormalizeUnitSum();
        }

        public void Round()
        {
            foreach (var chromosome in this._chromosomes)
                chromosome.Round();
        }

        public override string ToScreenString()
        {
            return this._chromosomes.Aggregate(
                string.Empty, (current, value) => string.Format("{0}{1}|", current, value.ToScreenString()));
        }

        #endregion

        #region ISocialECChromosome Members

        public ITestParameters this[uint agentIdx]
        {
            get { return this._chromosomes[agentIdx]; }
            set { this._chromosomes[agentIdx] = value as ArrayChromosome; }
        }

        public uint NumAgents { get; private set; }

        public override void Generate()
        {
            foreach (var chromosome in this._chromosomes)
                chromosome.Generate();
        }

        public override IChromosome Clone()
        {
            return new SocialArrayChromosome(this);
        }

        public override string[] ToValue()
        {
            var value = new string[this.NumAgents*this.Length];
            for (var i = 0; i < this._chromosomes.Length; i++)
            {
                var parameterValue = this._chromosomes[i].ToValue();
                Array.Copy(parameterValue, 0, value, i*parameterValue.Length, parameterValue.Length);
            }
            return value;
        }

        public override bool FromValue(string[] value)
        {
            if ((value == null) || (value.Length < this.NumAgents))
                return false;

            var valueSplit = value.Split(this.NumAgents);
            for (var i = 0; i < this.NumAgents; i++)
            {
                var arrayParameter = new ArrayParameter();
                if (!arrayParameter.FromValue(valueSplit[i])) return false;
                this._chromosomes[i] = new ArrayChromosome(this.Population, arrayParameter);
            }
            return true;
        }

        public override string[] Header
        {
            get
            {
                var header = new string[this.NumAgents*this.Length];
                for (var i = 0; i < this._chromosomes.Length; i++)
                {
                    var parameterValue = this._chromosomes[i].Header;
                    for (var j = 0; j < parameterValue.Length; j++)
                        parameterValue[j] += string.Format("Ag{0}", i);
                    Array.Copy(parameterValue, 0, header, i*parameterValue.Length, parameterValue.Length);
                }
                return header;
            }
        }

        public override void Mutate()
        {
            foreach (var chromosome in this._chromosomes)
                chromosome.Mutate();
        }

        public override void Crossover(IChromosome pair)
        {
            var otherChromosome = (SocialArrayChromosome) pair;
            for (var i = 0; i < this.NumAgents; i++)
                this._chromosomes[i].Crossover(otherChromosome._chromosomes[i]);
        }

        #endregion
    }
}