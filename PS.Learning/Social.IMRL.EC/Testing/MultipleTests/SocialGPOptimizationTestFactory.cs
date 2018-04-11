// ------------------------------------------
// <copyright file="SocialGPOptimizationTestFactory.cs" company="Pedro Sequeira">
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

//    Last updated: 3/27/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PS.Learning.EvolutionaryComputation;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.IMRL.EC.Testing;
using PS.Learning.Core.Testing;
using PS.Learning.Core.Testing.Config;
using PS.Learning.IMRL.EC.Testing.MultipleTests;
using PS.Learning.Social.IMRL.EC.Chromosomes;
using PS.Learning.Social.IMRL.EC.Domain;
using PS.Learning.Social.IMRL.EC.Testing.SingleTests;
using PS.Learning.Social.Testing;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.SingleTests;

namespace PS.Learning.Social.IMRL.EC.Testing.MultipleTests
{
    public class SocialGPOptimizationTestFactory : GPOptimizationTestFactory
    {
        #region Constructors

        public SocialGPOptimizationTestFactory(ISocialFitnessScenario scenario) : base(scenario)
        {
        }

        #endregion

        #region Properties

        public new SocialScenario Scenario
        {
            get { return base.Scenario as SocialScenario; }
        }

        public new ISocialGPTestsConfig TestsConfig
        {
            get { return base.TestsConfig as ISocialGPTestsConfig; }
        }

        #endregion

        #region Protected Methods

        public override FitnessTest CreateTest(ITestParameters parameters)
        {
            var singleTest = new SocialGPFitnessTest(this.Scenario, (SocialGPChromosome) parameters);
            singleTest.Reset();
            return singleTest;
        }

        #endregion

        protected override string GetTranslatedExpression(SingleTest test)
        {
            var simulation = (SocialSimulation) test.CreateAndSetupSimulation(0);
            var translatedExpressions =
                this.TestsConfig.SameAgentParameters
                    ? new List<string>
                      {
                          ((ISocialGPAgent) simulation.Population[0]).MotivationManager.TranslatedExpression
                      }
                    : new List<string>(from ISocialGPAgent agent in simulation.Population
                        select agent.MotivationManager.TranslatedExpression);

            var translatedExpression =
                translatedExpressions.Aggregate(
                    string.Empty, (current, value) => string.Format("{0}{1}_", current, value));
            return translatedExpression.Remove(translatedExpression.Length - 1, 1);
        }
    }
}