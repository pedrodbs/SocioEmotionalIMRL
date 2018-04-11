// ------------------------------------------
// <copyright file="SocialCommonGPChromosome.cs" company="Pedro Sequeira">
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

//    Last updated: 02/19/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AForge.Genetic;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.Core.Testing.Config.Parameters;

namespace PS.Learning.Social.IMRL.EC.Chromosomes
{
    [Serializable]
    public class SocialCommonGPChromosome : GPChromosome, ISocialECChromosome
    {
        public SocialCommonGPChromosome(uint numAgents, GPChromosome commomTestParameters)
            : base(commomTestParameters.BaseChromosome ?? commomTestParameters, commomTestParameters.Population)
        {
            this.NumAgents = numAgents;
        }

        public override HashSet<IGPChromosome> AllCombinations
        {
            get
            {
                return new HashSet<IGPChromosome>(
                    base.AllCombinations.Select(
                        comb => new SocialCommonGPChromosome(this.NumAgents, (GPChromosome) comb)));
            }
        }

        #region ISocialECChromosome Members

        public override IChromosome CreateNew()
        {
            return new SocialCommonGPChromosome(this.NumAgents, (GPChromosome) base.CreateNew());
        }

        public override IChromosome Clone()
        {
            return new SocialCommonGPChromosome(this.NumAgents, (GPChromosome) base.Clone());
        }

        #endregion

        #region Implementation of ISocialTestParameters

        public ITestParameters this[uint agentIdx]
        {
            get { return this; }
            set { this.BaseChromosome = (GPChromosome) value.Clone(); }
        }

        public uint NumAgents { get; private set; }

        #endregion
    }
}