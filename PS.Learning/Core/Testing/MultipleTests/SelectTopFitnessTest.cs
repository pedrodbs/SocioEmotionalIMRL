// ------------------------------------------
// <copyright file="SelectTopFitnessTest.cs" company="Pedro Sequeira">
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

using PS.Learning.Core.Testing.SingleTests;

namespace PS.Learning.Core.Testing.MultipleTests
{
    public class SelectTopFitnessTest : ListFitnessTest
    {
        #region Constructors

        public SelectTopFitnessTest(
            IOptimizationTestFactory optimizationTestFactory, uint numberTests, bool topTests)
            : base(optimizationTestFactory)
        {
            this.TopTests = topTests;
            this.NumberTests = numberTests;
            this.RunAllTestsAgain = true;
        }

        #endregion

        #region Public Methods

        public override bool Run()
        {
            return this.NumberTests != 0 && base.Run();
        }

        #endregion

        #region Constants

        protected const string BOTTOM_TESTS_ID = "Bottom";
        protected const string TOP_TESTS_ID = "Top";
        protected const string GENERAL_TESTS_ID = "General";

        #endregion

        #region Fields

        #endregion

        #region Properties

        public override string TestID
        {
            get { return $"{(this.TopTests ? TOP_TESTS_ID : BOTTOM_TESTS_ID)}{this.NumberTests}"; }
        }

        public bool TopTests { get; set; }

        public uint NumberTests { get; set; }

        #endregion

        #region Protected Methods

        protected override void DetermineParametersList(bool fromFile = false)
        {
            //selects top chromosomes from current history
            if (this.testParameters != null) this.testParameters.Clear();
            this.testParameters = this.TestMeasures.SelectTopParameters(this.NumberTests, this.TopTests);
        }

        protected override void PrepareSingleTest(FitnessTest test)
        {
            base.PrepareSingleTest(test);
            lock (this.locker)
                test.LogStatistics = this.TestsConfig.GraphicsEnabled;
        }

        #endregion
    }
}