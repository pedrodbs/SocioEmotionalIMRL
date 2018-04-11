// ------------------------------------------
// <copyright file="SocialGPSimplifierOptimizationTestFactory.cs" company="Pedro Sequeira">
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

//    Last updated: 3/10/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System.Collections.Generic;
using PS.Learning.Core.Testing.Config;
using PS.Learning.EvolutionaryComputation.Testing;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.IMRL.EC.Testing.MultipleTests;
using PS.Learning.Social.Testing;
using PS.Learning.Core.Testing.Config.Parameters;

namespace PS.Learning.Social.IMRL.EC.Testing.MultipleTests
{
    public class SocialGPSimplifierOptimizationTestFactory
        : SocialGPOptimizationTestFactory, IGPSimplifierOptimizationTestFactory
    {
        public SocialGPSimplifierOptimizationTestFactory(ISocialFitnessScenario scenario)
            : base(scenario)
        {
            this.SimplifiedParamMeasures = new Dictionary<IGPChromosome, ECTestMeasure>();
            this.SimplifiedChromosomesMeasures = new Dictionary<ECTestMeasure, List<ECTestMeasure>>();
            this.MinLengthChromosome = new Dictionary<ECTestMeasure, uint>();
        }

        #region IGPSimplifierOptimizationTestFactory Members

        public Dictionary<ECTestMeasure, uint> MinLengthChromosome { get; private set; }

        public Dictionary<ECTestMeasure, List<ECTestMeasure>> SimplifiedChromosomesMeasures { get; private set; }

        public Dictionary<IGPChromosome, ECTestMeasure> SimplifiedParamMeasures { get; private set; }

        public virtual List<ITestParameters> DetermineChromosomeParameters(ECTestMeasureList testMeasureList)
        {
            var testParameters = new List<ITestParameters>();

            this.SimplifiedParamMeasures.Clear();
            this.MinLengthChromosome.Clear();

            //adds all possible sub-combinations
            var subCombinations = new HashSet<IGPChromosome>();
            foreach (IGPChromosome parameters in testMeasureList)
            {
                var testMeasure = (ECTestMeasure) testMeasureList.GetTestMeasure(parameters);
                this.MinLengthChromosome[testMeasure] = parameters.Length;
                this.SimplifiedChromosomesMeasures[testMeasure] = new List<ECTestMeasure>();

                var allParamCombinations = parameters.AllCombinations;
                subCombinations.UnionWith(allParamCombinations);

                foreach (var paramCombination in allParamCombinations)
                    this.SimplifiedParamMeasures[paramCombination] = testMeasure;
            }
            testParameters.AddRange(subCombinations);

            //sorts chromosome parameters by length
            testParameters.Sort();

            return testParameters;
        }

        #endregion
    }
}