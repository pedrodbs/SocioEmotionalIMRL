// ------------------------------------------
// <copyright file="TestRunner.cs" company="Pedro Sequeira">
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
//    Project: Learning
//    Last updated: 04/01/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Utilities.Core;
using PS.Utilities.IO.Parsing;
using PS.Utilities.Math;

namespace PS.Learning.Core.Testing.Runners
{
    public abstract class TestRunner
    {
        #region Protected Constructors

        protected TestRunner(ITestsConfig testsConfig)
        {
            this.TestsConfig = testsConfig;
            this.ForceConsole = false;
        }

        #endregion Protected Constructors

        #region Public Properties

        public bool ForceConsole { get; set; }
        public ITestsConfig TestsConfig { get; set; }

        #endregion Public Properties

        #region Protected Properties

        protected IScenario DefaultScenario
        {
            get { return this.TestsConfig.ScenarioProfiles[this.TestsConfig.SingleTestType]; }
        }

        protected IOptimizationTestFactory DefaultTestFactory { get; private set; }

        #endregion Protected Properties

        #region Public Methods

        public bool ConfigureTest(string[] args = null)
        {
            //tries to get tests config from cmd-line, otherwise uses default config
            if ((args != null) && (args.Length > 0))
                ArgumentsParser.Parse(args, this.TestsConfig);

            //sets affinity
            ProcessUtil.SetProcessAffinity(this.TestsConfig.MaxCPUsUsed);

            //inits test config
            this.TestsConfig.Init();
            this.DefaultTestFactory = this.TestsConfig.CreateTestFactory(
                this.DefaultScenario, this.TestsConfig.NumSimulations, this.TestsConfig.NumSamples);

            return true;
        }

        public abstract void RunTest();

        #endregion Public Methods
    }
}