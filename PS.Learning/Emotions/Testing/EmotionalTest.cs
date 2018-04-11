// ------------------------------------------
// <copyright file="EmotionalTest.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.IMRL.Emotions

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.Simulations;
using PS.Learning.IMRL.Emotions.Domain.Agents;
using PS.Learning.IMRL.Testing;
using PS.Utilities.Math;
using System.Collections.Generic;

namespace PS.Learning.IMRL.Emotions.Testing
{
    public class EmotionalTest : IMRLTest
    {
        #region Public Fields

        public const string CONTROL_STAT_ID = "Control";
        public const string GOAL_REL_STAT_ID = "GoalRelevance";
        public const string NOVELTY_STAT_ID = "Novelty";
        public const string VALENCE_STAT_ID = "Valence";

        #endregion Public Fields

        #region Protected Fields

        protected List<string> emotionLabels;

        #endregion Protected Fields

        #region Public Constructors

        public EmotionalTest(IFitnessScenario scenario, ITestParameters testParameters)
            : base(scenario, testParameters)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override Simulation CreateAndSetupSimulation(uint simulationIDx)
        {
            var simulation = base.CreateAndSetupSimulation(simulationIDx);

            //stores emotion labels list
            if ((this.emotionLabels == null) && (simulation.Agent is IEmotionalAgent))
                this.emotionLabels =
                    new List<string>(((IEmotionalAgent)simulation.Agent).EmotionsManager.EmotionLabels.Keys);
            return simulation;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void PrintAgent()
        {
            base.PrintAgent();

            if (!this.LogStatistics) return;

            var emotionalQuantityList = new StatisticsCollection
                                        {
                                            {CONTROL_STAT_ID, this.testStatisticsAvg[CONTROL_STAT_ID]},
                                            {VALENCE_STAT_ID, this.testStatisticsAvg[VALENCE_STAT_ID]},
                                            {GOAL_REL_STAT_ID, this.testStatisticsAvg[GOAL_REL_STAT_ID]},
                                            {NOVELTY_STAT_ID, this.testStatisticsAvg[NOVELTY_STAT_ID]}
                                        };

            emotionalQuantityList.PrintAllQuantitiesToCSV(string.Format("{0}/Emotions/DimensionsAvg.csv", this.FilePath));

            //emotionalQuantityList.Clear();
            //foreach (var emotionLabel in this.emotionLabels)
            //    emotionalQuantityList.Add(emotionLabel, this.testStatisticsAvg[emotionLabel));

            //StatisticsUtil.PrintAllQuantitiesToCSV(this.FilePath + "/Emotions/LabelsAvg.csv", emotionalQuantityList);
            //StatisticsUtil.PrintAllQuantitiesCountToXLS(this.FilePath + "/Emotions/LabelsCountAvg.csv",
            //                                     emotionalQuantityList);
        }

        #endregion Protected Methods
    }
}