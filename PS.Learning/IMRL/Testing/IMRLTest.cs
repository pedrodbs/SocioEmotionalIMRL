// ------------------------------------------
// <copyright file="IMRLTest.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.IMRL

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.SingleTests;
using PS.Learning.IMRL.Testing.Config;
using PS.Utilities.Math;

namespace PS.Learning.IMRL.Testing
{
    public class IMRLTest : FitnessTest
    {
        #region Public Constructors

        public IMRLTest(IFitnessScenario scenario, ITestParameters testParameters)
            : base(scenario, testParameters)
        {
            if(testParameters is ArrayParameter arrayParameters)
                arrayParameters.Header = this.TestsConfig.ParamIDNames;
        }

        #endregion Public Constructors

        #region Public Properties

        protected new IIMRLTestsConfig TestsConfig => base.TestsConfig as IIMRLTestsConfig;

        #endregion Public Properties

        #region Protected Methods

        protected override void PrintAgent()
        {
            base.PrintAgent();

            if (!this.LogStatistics) return;

            this.PrintStatistic("NumBackups", "/Learning/NumBackupsAvg.csv");

            var rewardQuantityList = new StatisticsCollection
                                     {
                                         //{"Reward", this.GetAgentQuantityAverage("Reward")},
                                         {"ExtrinsicReward", this.testStatisticsAvg["ExtrinsicReward"]},
                                         {"IntrinsicReward", this.testStatisticsAvg["IntrinsicReward"]}
                                     };
            rewardQuantityList.PrintAllQuantitiesToCSV(this.FilePath + "/STM/RewardAvg.csv");
        }

        #endregion Protected Methods
    }
}