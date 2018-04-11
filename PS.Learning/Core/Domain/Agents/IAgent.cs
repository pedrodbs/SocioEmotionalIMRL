// ------------------------------------------
// <copyright file="IAgent.cs" company="Pedro Sequeira">
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

using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Managers.Behavior;
using PS.Learning.Core.Domain.Managers.Learning;
using PS.Learning.Core.Domain.Managers.Motivation;
using PS.Learning.Core.Domain.Memories;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Utilities.Core;
using PS.Utilities.Math;
using System;
using System.Collections.Generic;

namespace PS.Learning.Core.Domain.Agents
{
    public interface IAgent : IUpdatable, IDisposable, IIdentifiableObject, IInitializable
    {
        #region Public Properties

        Dictionary<string, IAction> Actions { get; }
        BehaviorManager BehaviorManager { get; }
        StatisticalQuantity Fitness { get; set; }
        LearningManager LearningManager { get; }
        LogWriter LogWriter { get; set; }
        LongTermMemory LongTermMemory { get; }
        IMotivationManager MotivationManager { get; }
        IScenario Scenario { get; set; }
        ShortTermMemory ShortTermMemory { get; }
        StatisticsCollection StatisticsCollection { get; }
        ITestParameters TestParameters { get; set; }

        #endregion Public Properties

        #region Public Methods

        IAgent CreateNew();

        void PrintAll(string path, string imgFormat);

        void Reset();

        #endregion Public Methods
    }
}