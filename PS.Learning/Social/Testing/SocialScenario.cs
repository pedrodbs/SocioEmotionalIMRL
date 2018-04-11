﻿// ------------------------------------------
// <copyright file="SocialScenario.cs" company="Pedro Sequeira">
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
//    Project: Learning.Social

//    Last updated: 03/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Environments;
using PS.Learning.Social.Testing.FitnessFunctions;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;

namespace PS.Learning.Social.Testing
{
    [Serializable]
    public class SocialScenario : SingleScenario, ISocialFitnessScenario
    {
        public SocialScenario(ISocialAgent baseAgent, ISocialEnvironment baseEnvironment, ITestsConfig testsConfig)
            : base(baseAgent, baseEnvironment, testsConfig)
        {
            //default fitness function
            this.PopulationFitnessFunction = new MeanPopFitnessFunction();
        }

        #region ISocialFitnessScenario Members

        public uint NumAgents { get; set; }
        public bool StrongerAgent { get; set; }
        public string AgentImgPrefix { get; set; }

        #region Implementation of ISocialFitnessTestProfile

        public IPopulationFitnessFunction PopulationFitnessFunction { get; set; }

        #endregion

        #endregion
    }
}