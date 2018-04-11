// ------------------------------------------
// <copyright file="GPOptimizationTestFactory.cs" company="Pedro Sequeira">
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

//    Last updated: 03/27/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.EvolutionaryComputation;
using PS.Learning.EvolutionaryComputation.Testing;
using PS.Learning.IMRL.EC.Domain;
using PS.Learning.Core.Testing;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Core.Testing.SingleTests;

namespace PS.Learning.IMRL.EC.Testing.MultipleTests
{
    public class GPOptimizationTestFactory : OptimizationTestFactory
    {
        #region Constructors

        public GPOptimizationTestFactory(IFitnessScenario scenario) : base(scenario)
        {
        }

        #endregion

        public override TestMeasure CreateTestMeasure(FitnessTest test)
        {
            if(test==null) return new GPTestMeasure();

            var chromosome = (IECChromosome) test.TestParameters;

            //creates new chromosome test measure
            return new GPTestMeasure
                   {
                       ID = test.TestName,
                       Parameters = chromosome,
                       Quantity = test.SimulationScoreAvg,
                       Value = test.FinalScores.Mean,
                       StdDev = test.FinalScores.StdDev,
                       TranslatedExpression = this.GetTranslatedExpression(test),
                       TimesGenerated = 1,
                       GenerationNumber =
                           chromosome.Population == null ? -1 : chromosome.Population.GenerationNumber
                   };
        }

        protected virtual string GetTranslatedExpression(SingleTest test)
        {
            var simulation = test.CreateAndSetupSimulation(0);
            return ((IGPAgent) simulation.Agent).MotivationManager.TranslatedExpression;
        }

        public override TestMeasureList CreateTestMeasureList()
        {
            //use as base parameters the single test params
            var baseMeasure = this.CreateTestMeasure(null);
            baseMeasure.Parameters = this.TestsConfig.SingleTestParameters;
            return new ECTestMeasureList(this.Scenario, (GPTestMeasure) baseMeasure);
        }
    }
}