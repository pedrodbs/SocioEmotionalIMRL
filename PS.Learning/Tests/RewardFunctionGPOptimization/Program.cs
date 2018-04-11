// ------------------------------------------
// <copyright file="Program.cs" company="Pedro Sequeira">
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
//    Project: Learning.Tests.RewardFunctionGPOptimization

//    Last updated: 03/25/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using PS.Learning.EvolutionaryComputation.Testing.MultipleTests;
using PS.Learning.Forms;
using PS.Learning.Forms.Testing;
using PS.Learning.IMRL.EC.Testing;
using PS.Learning.IMRL.EC.Testing.MultipleTests;
using PS.Learning.Core.Testing;
using PS.Learning.Tests.RewardFunctionGPOptimization.Testing;

namespace PS.Learning.Tests.RewardFunctionGPOptimization
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var testsConfig = new EmotionalGPTestsConfig();
            testsConfig.SetDefaultConstants();
            testsConfig.Init();

            var simplifierTest = new GPSimplifierFitnessTest(null);
            var programRunner = new ProgramRunner(
                testsConfig, new FormsSingleTestRunner(testsConfig),
                new GPOptimizationScheme(new ECParallelOptimTest(null), 208,
                    new List<TopTestScheme>
                    {
                        new TopTestScheme(208, 8),
                        new TopTestScheme(104, 16),
                        new TopTestScheme(48, 48),
                        new TopTestScheme(10, 104),
                        new TopTestScheme(5, 208),
                        new TopTestScheme(1, 208)
                    }, simplifierTest));

            programRunner.Run(args);
        }
    }
}