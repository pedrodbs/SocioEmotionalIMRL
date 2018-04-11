// ------------------------------------------
// <copyright file="IMRLTestsConfig.cs" company="Pedro Sequeira">
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

using CommandLine.Text;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Utilities.Collections;
using PS.Utilities.Core;
using PS.Utilities.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace PS.Learning.IMRL.Testing.Config
{
    [Serializable]
    public abstract class IMRLTestsConfig : TestsConfig, IIMRLTestsConfig
    {
        #region Protected Constructors

        protected IMRLTestsConfig()
        {
            this.AddSpecialTests = true;
        }

        #endregion Protected Constructors

        #region Public Properties

        public bool AddSpecialTests { get; set; }

        public int NumParams
        {
            get { return this.ParamsStepIntervals.Length; }
        }

        public char[] ParamIDLetters { get; set; }
        public string[] ParamIDNames { get; set; }
        public Range<double>[] ParamsDomains { get; set; }
        public StepInterval<double>[] ParamsStepIntervals { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override List<ITestParameters> GetOptimizationTestParameters()
        {
            //creates tests for different weight parameters according to TestsConfig
            var testParameters = new List<ITestParameters>();
            var elements = NumericArrayUtil<double>.NumericArrayFromInterval(this.ParamsStepIntervals);
            var allParamsComb = elements.AllCombinations();
            foreach (var paramsComb in allParamsComb)
            {
                //creates array parameter
                var arrayParameter = new ArrayParameter(paramsComb);

                //only considers combinations with abs sum of 1, eg (-0.1, 0.9), (0.4, 0.6)
                if (this.IsValidParameter(arrayParameter))
                    testParameters.Add(arrayParameter);
            }

            return testParameters;
        }

        public override List<ITestParameters> GetSpecialTestParameters(IScenario scenario)
        {
            var specialParams = new List<ITestParameters>();

            if (!this.AddSpecialTests) return specialParams;

            //adds random test parameter (eg. (0, 0, 0))
            var randomParam = new ArrayParameter(new double[this.NumParams]);
            specialParams.Add(randomParam);

            //adds fitness only test parameter (eg. (0, 0, 1))
            var fitOnlyParam = new ArrayParameter(new double[this.NumParams]);
            fitOnlyParam[(this.NumParams - 1)] = 1d;
            specialParams.Add(fitOnlyParam);

            return specialParams;
        }

        public override string GetTestName(IScenario scenario, ITestParameters testParameters)
        {
            //create test name from parameters, eg (a-0.1_b0.7_c0.2)
            var paramLetterIDs = this.ParamIDLetters;
            var arrayParameter = (ArrayParameter)testParameters;
            var sb = new StringBuilder("(");

            for (var i = 0; i < arrayParameter.Length; i++)
                sb.AppendFormat("{0}{1:0.0}_", paramLetterIDs[i], arrayParameter[i]);

            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            return sb.ToString();
        }

        public virtual bool IsValidParameter(IArrayParameter parameter)
        {
            //only params that have sum of absolute values of 1, eg (-0.1, 0.9) or (0.4, 0.6)
            return parameter.AbsoulteSum.Equals(1);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnFormatOptionHelpText(object sender, FormatOptionHelpTextEventArgs e)
        {
            if (e.Option.LongName.Equals(SINGLE_TEST_PARAMS_ARG))
                e.Option.HelpText += string.Format(" Usage: --{0}={1}.", SINGLE_TEST_PARAMS_ARG,
                    this.ParamIDNames.ToString(' ', false, "'", "'"));
        }

        #endregion Protected Methods
    }
}