// ------------------------------------------
// <copyright file="SelectBestFitnessTest.cs" company="Pedro Sequeira">
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
//    Last updated: 12/09/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

namespace PS.Learning.Core.Testing.MultipleTests
{
    public class SelectBestFitnessTest : ListFitnessTest
    {
        private const string BEST_TEST_ID = "Best";
        private const string STDDEV = "StdDev";

        #region Constructors

        public SelectBestFitnessTest(
            IOptimizationTestFactory optimizationTestFactory, int stdDevTimes) : base(optimizationTestFactory)
        {
            this.StdDevTimes = stdDevTimes;
            this.RunAllTestsAgain = true;
        }

        #endregion

        #region Protected Methods

        protected override void DetermineParametersList(bool readFromFile = false)
        {
            //clears vars
            if (this.testParameters != null) this.testParameters.Clear();
            this.testParameters = this.TestMeasures.SelectBestMeasures(this.StdDevTimes);
        }

        #endregion

        #region Properties

        public override string TestID
        {
            get { return $"{BEST_TEST_ID}{this.StdDevTimes}{STDDEV}"; }
        }

        public int StdDevTimes { get; set; }

        #endregion
    }
}