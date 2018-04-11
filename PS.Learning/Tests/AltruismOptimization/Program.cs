// ------------------------------------------
// Program.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2015/4/1
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using System;
using System.Collections.Generic;
using PS.Learning.Forms;
using PS.Learning.Forms.Testing;
using PS.Learning.Core.Testing;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Tests.AltruismOptimization.Testing;

namespace PS.Learning.Tests.AltruismOptimization
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var testsConfig = new AltruismTestsConfig();

            var programRunner = new ProgramRunner(testsConfig,
                new FormsSingleTestRunner(testsConfig), new OptimizationScheme(new ListFitnessTest(null),
                    new List<TopTestScheme>
                    {
                        new TopTestScheme(208, 8),
                        new TopTestScheme(48, 48),
                        new TopTestScheme(10, 104),
                        new TopTestScheme(5, 208),
                        new TopTestScheme(1, 208)
                    }));

            programRunner.Run(args);
        }
    }
}