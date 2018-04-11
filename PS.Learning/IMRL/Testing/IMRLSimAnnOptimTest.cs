// ------------------------------------------
// <copyright file="IMRLSimAnnOptimTest.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL

//    Last updated: 02/14/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Testing;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Core.Testing.StochasticOptimization;

namespace PS.Learning.IMRL.Testing
{
    public class IMRLSimAnnOptimTest : SimulatedAnnealingOptimTest
    {
        public IMRLSimAnnOptimTest(
            string id, IOptimizationTestFactory optimizationTestFactory, TestMeasureList testMeasures)
            : base(id, optimizationTestFactory, testMeasures)
        {
        }

        protected override ITestParameters ComputeNeighbour(ITestParameters currentParam)
        {
            //first tested params
            if (currentParam == null)
                return this.testsConfig.SingleTestParameters;

            //define search radius based on temperature
            var next = (ArrayParameter) currentParam.Clone();
            var initialTemperature = this.TestsConfig.InitialTemperature;
            var searchRadius = 2*Random.NextDouble()*(1d - ((initialTemperature - temperature)/initialTemperature));

            //generate random point in hyper-sphere according to radius
            for (var i = 0; i < next.Length; i++)
                //"moves" current point in the direction of that point
                next[i] = ((ArrayParameter) currentParam)[i] + (searchRadius*((Random.NextDouble()*2) - 1d));

            //normalizes result
            next.NormalizeUnitSum();
            next.Round();

            return next;
        }
    }
}