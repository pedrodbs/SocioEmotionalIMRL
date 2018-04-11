// ------------------------------------------
// <copyright file="ITestsConfig.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.Core

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Utilities.IO.Parsing;
using System;
using System.Collections.Generic;

namespace PS.Learning.Core.Testing.Config
{
    /// <summary>
    ///     Configuration for a group of tests. General tests configuration.
    /// </summary>
    public interface ITestsConfig : ICloneable, IArgumentsParsable
    {
        #region Public Properties

        double ActionAccuracy { get; set; }

        string BaseFilePath { get; set; }

        int CellSize { get; set; }

        string CondorScriptPath { get; set; }

        string CondorServerBaseEnvPath { get; set; }

        double Discount { get; set; }

        string EnvBaseFilePath { get; set; }

        double Epsilon { get; set; }

        double ExploratoryDecay { get; set; }

        bool GraphicsEnabled { get; set; }

        double InitialActionValue { get; set; }

        double LearningRate { get; set; }

        uint MaxCPUsUsed { get; set; }

        double MaximalChangeThreshold { get; set; }

        string MemoryBaseFilePath { get; set; }

        uint[] MultipleTestTypes { get; set; }

        uint NumSamples { get; set; }

        uint NumSimulations { get; set; }

        uint NumTimeSteps { get; set; }

        bool RandStart { get; set; }

        Dictionary<uint, IScenario> ScenarioProfiles { get; set; }

        ITestParameters SingleTestParameters { get; set; }

        uint SingleTestType { get; set; }

        double Temperature { get; set; }

        string TestListFilePath { get; set; }

        string TestMeasuresName { get; set; }

        #endregion Public Properties

        #region Public Methods

        IOptimizationTestFactory CreateTestFactory(
            IScenario scenario, uint numSimulations = 100, uint numSamples = 100);

        List<ITestParameters> GetOptimizationTestParameters();

        List<ITestParameters> GetSpecialTestParameters(IScenario scenario);

        string GetTestName(IScenario scenario, ITestParameters testParameters);

        void Init();

        void SetDefaultConstants();

        #endregion Public Methods
    }
}