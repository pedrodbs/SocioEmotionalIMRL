// ------------------------------------------
// <copyright file="EmotionalOptimizationTestFactory.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL.Emotions

//    Last updated: 10/15/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using System.Linq;
using PS.Learning.IMRL.Emotions.Domain.Managers;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Core.Testing.SingleTests;

namespace PS.Learning.IMRL.Emotions.Testing
{
    public class EmotionalOptimizationTestFactory : OptimizationTestFactory
    {
        public EmotionalOptimizationTestFactory(IFitnessScenario scenario)
            : base(scenario)
        {
        }

        public new EmotionalTestsConfig TestsConfig
        {
            get { return base.TestsConfig as EmotionalTestsConfig; }
        }

        public virtual List<ITestParameters> GenerateEmotionalLabelParameters()
        {
            var manager = new SchererEmotionsManager(null);

            return manager.EmotionLabels.Values.Select(
                label => new ArrayParameter(new[]
                                            {
                                                label.Arousal.Value, label.Valence.Value, label.GoalRelevance.Value,
                                                label.Novelty.Value, label.Control.Value
                                            })).Cast<ITestParameters>().ToList();
        }

        public override FitnessTest CreateTest(ITestParameters parameters)
        {
            var singleTest = new EmotionalTest((IFitnessScenario) this.Scenario.Clone(), (ArrayParameter) parameters);
            singleTest.Reset();
            return singleTest;
        }
    }
}