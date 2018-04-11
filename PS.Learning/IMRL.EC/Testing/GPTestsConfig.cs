// ------------------------------------------
// <copyright file="GPTestsConfig.cs" company="Pedro Sequeira">
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

//    Last updated: 05/22/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using AForge.Genetic;
using PS.Learning.EvolutionaryComputation.Testing;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.IMRL.EC.Genes;
using PS.Learning.IMRL.EC.Testing.MultipleTests;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;

namespace PS.Learning.IMRL.EC.Testing
{
    [Serializable]
    public abstract class GPTestsConfig : ECTestsConfig, IGPTestsConfig
    {
        private const string FIT_CHROMOSOME_EXPRESSION = "$0 ";
        private const string RAND_CHROMOSOME_EXPRESSION = "0 ";

        public virtual int VariablesCount
        {
            get { return (int) (this.Constants.Length + this.NumBaseVariables); }
        }

        #region IGPTestsConfig Members

        public int MaxInitialLevel { get; set; }
        public uint NumBaseVariables { get; set; }
        public double[] Constants { get; set; }
        public int MaxProgTreeDepth { get; set; }
        public HashSet<FunctionType> AllowedFunctions { get; set; }

        public abstract IGPSimplifierOptimizationTestFactory CreateSimplifierTestFactory(
            IScenario scenario, uint numSimulations, uint numSamples);

        public override List<ITestParameters> GetSpecialTestParameters(IScenario scenario)
        {
            //adds random and fitness only test parameters 
            return new List<ITestParameters>
                   {
                       new GPExpressionChromosome(FIT_CHROMOSOME_EXPRESSION),
                       new GPExpressionChromosome(RAND_CHROMOSOME_EXPRESSION)
                   };
        }

        public override void SetDefaultConstants()
        {
            base.SetDefaultConstants();

            this.MaxProgTreeDepth = 3; //4;
            this.MaxInitialLevel = 1; //4;

            //sets maximum level of tree depth for genetic programs
            GPTreeChromosome.MaxLevel = this.MaxProgTreeDepth;
            GPTreeChromosome.MaxInitialLevel = this.MaxInitialLevel;
        }

        public override List<ITestParameters> GetOptimizationTestParameters()
        {
            return new List<ITestParameters>();
        }

        #endregion
    }
}