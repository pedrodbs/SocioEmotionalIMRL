// ------------------------------------------
// <copyright file="ECTestMeasureList.cs" company="Pedro Sequeira">
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

//    Last updated: 02/06/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using PS.Learning.Core.Testing;
using PS.Learning.Core.Testing.Config.Scenarios;

namespace PS.Learning.EvolutionaryComputation.Testing
{
    public class ECTestMeasureList : TestMeasureList
    {
        public ECTestMeasureList(IScenario scenario, ECTestMeasure baseMeasure) : base(scenario, baseMeasure)
        {
        }

        public void Add(IECChromosome chromosome, ECTestMeasure testMeasure)
        {
            //locks from outside access
            lock (this.locker)
            {
                //changes some parameters of the stored chromosome history
                if (!this.Contains(chromosome))
                    base.Add(chromosome, testMeasure);
                else
                    this.UpdateTestMeasure(chromosome, testMeasure);
            }
        }

        public void UpdateTestMeasure(IECChromosome chromosome)
        {
            this.UpdateTestMeasure(chromosome, null);
        }

        public virtual void UpdateTestMeasure(IECChromosome chromosome, ECTestMeasure testMeasure)
        {
            //checks and gets chromosome history
            if (!this.Contains(chromosome)) return;
            var prevTestMeasure = (ECTestMeasure) this.testMeasures[chromosome];

            //updates generation number (sets to the youngest between the two)
            if (chromosome.Population != null)
                prevTestMeasure.GenerationNumber =
                    Math.Min(prevTestMeasure.GenerationNumber, chromosome.Population.GenerationNumber);

            //averages chromosome fitness with new quantity info
            if (testMeasure != null)
                prevTestMeasure.Value = 0.5f*(prevTestMeasure.Value + testMeasure.Value);

            //increases generation counter
            prevTestMeasure.TimesGenerated++;
        }
    }
}