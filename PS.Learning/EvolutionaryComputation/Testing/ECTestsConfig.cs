// ------------------------------------------
// <copyright file="ECTestsConfig.cs" company="Pedro Sequeira">
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

//    Last updated: 05/22/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using AForge.Genetic;
using CommandLine;
using Newtonsoft.Json;
using PS.Learning.Core.Testing.Config;

namespace PS.Learning.EvolutionaryComputation.Testing
{
    [Serializable]
    public abstract class ECTestsConfig : TestsConfig, IECTestsConfig
    {
        #region IECTestsConfig Members

        public ISelectionMethod SelectionMethod { get; set; }
        public uint FitnessImprovementThreshold { get; set; }
        public double RandomSelectionPortion { get; set; }
        public double SteadyStatePortion { get; set; }
        public double SymmetryFactor { get; set; }
        public int StdDevTimes { get; set; }
        public abstract IECChromosome CreateBaseChromosome();

        #endregion

        #region Json and parsing

        public const string NUM_TESTS_ITER_ARG = "testsPerIteration";
        private const string NUM_TESTS_ITER_MSG = "The number of tests per iteration (pop. size).";

        private const string TEST_MEASURES_PREFIX_MSG = "Prefix to use on test measures file.";
        public const string TEST_MEASURES_PREFIX_ARG = "measuresPrefix";

        public const string MAX_ITERATIONS_ARG = "iterations";

        private const string MAX_ITERATIONS_MSG =
            "The maximum number of iterations to run for each optimization test (max. generations).";

        public const string NUM_PARALLEL_TESTS_ARG = "parallelTests";
        private const string NUM_PARALLEL_TESTS_MSG = "The number of optimization tests to run (num. populations).";

        [JsonProperty(NUM_TESTS_ITER_ARG)]
        [Option(NUM_TESTS_ITER_ARG, Required = true, HelpText = NUM_TESTS_ITER_MSG)]
        public int NumTestsPerIteration { get; set; }

        [JsonProperty(MAX_ITERATIONS_ARG)]
        [Option(MAX_ITERATIONS_ARG, Required = true, HelpText = MAX_ITERATIONS_MSG)]
        public uint MaxIterations { get; set; }

        [JsonProperty(NUM_PARALLEL_TESTS_ARG)]
        [Option(NUM_PARALLEL_TESTS_ARG, Required = true, HelpText = NUM_PARALLEL_TESTS_MSG)]
        public uint NumParallelOptimTests { get; set; }

        [JsonProperty(TEST_MEASURES_PREFIX_ARG)]
        [Option(TEST_MEASURES_PREFIX_ARG, Required = false, HelpText = TEST_MEASURES_PREFIX_MSG)]
        public string TestMeasuresPrefix { get; set; }

        #endregion
    }
}