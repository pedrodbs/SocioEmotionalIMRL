// ------------------------------------------
// <copyright file="FormsParallelOptimTestRunnner.cs" company="Pedro Sequeira">
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
//    Project: Learning.Forms

//    Last updated: 04/01/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Testing;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Core.Testing.Runners;
using PS.Utilities.Forms;

namespace PS.Learning.Forms.Testing
{
    public class FormsParallelOptimTestRunnner : ParallelOptimTestRunnner
    {
        public FormsParallelOptimTestRunnner(ITestsConfig testsConfig, OptimizationScheme optimizationScheme)
            : base(testsConfig, optimizationScheme)
        {
        }

        protected override void RunTest(ParallelOptimizationTest test)
        {
            //starts progress form, also links close button to interrupt test
            using (var progressFormUpdater = new ProgressFormUpdater(test)
                                             {
                                                 Visible = this.TestsConfig.GraphicsEnabled,
                                                 Text = string.Format("{0}", test.TestID)
                                             })
            {
                progressFormUpdater.FormTerminated += test.InterruptProcessing;
                base.RunTest(test);
            }
        }
    }
}