// ------------------------------------------
// <copyright file="EmotionalTestsConfig.cs" company="Pedro Sequeira">
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
//    Project: Learning.Tests.EmotionalOptimization

//    Last updated: 06/20/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Environments;
using PS.Learning.IMRL.Emotions.Domain.Agents;
using PS.Learning.IMRL.Emotions.Testing;
using PS.Learning.Core.Testing;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Tests.EmotionalOptimization.Domain.Agents;
using PS.Learning.Tests.EmotionalOptimization.Domain.Environments;
using PS.Utilities.Core;
using PS.Utilities.Collections;

namespace PS.Learning.Tests.EmotionalOptimization.Testing
{

    #region TestType enum

    public enum TestType
    {
        MovingPreys,
        Persistence,
        DifferentPreySeason,
        PoisonedSeason,
        HungerThirst,
        Lairs,
        Pacman,
        PowerPellets,
        EatAllDots,
        RewardingDots,
        AllTests,
        AllPacManTests,
        AllForagingTests
    }

    #endregion

    [Serializable]
    public class EmotionalTestsConfig : IMRL.Emotions.Testing.EmotionalTestsConfig
    {
        private const string SCORE_TEXT = "Score";
        protected string EnvironmentFile { get; set; }
        protected string CornersEnvironmentFile { get; set; }
        protected string PacmanEnvironmentFile { get; set; }

        public override void SetDefaultConstants()
        {
            base.SetDefaultConstants();

            //directories and files config
            this.BaseFilePath = "./";
            this.EnvBaseFilePath = "../../../../bin/config";
            this.CondorServerBaseEnvPath = "../config/";
            this.CondorScriptPath = "./submit";

            this.DestinationTestsDir = Path.GetFullPath("Tests/");
            this.PreviousPhaseID = "Optim";
            this.TestMeasuresName = "TestMeasures";
            this.TestIDPrefix = "IM"; //"IMGP"; //"IM";

            //test-related constants
            var stepInterval = new StepInterval<double>(-1, 1, .1); //(-1, 1, .2); //.1m; //.25m; //1m; //.2m
            this.ParamsStepIntervals = StepInterval<double>.CreateArray(stepInterval, 5);
            this.NumTimeSteps = 100000;
            this.NumSimulations = 10; //208; //1; //4; //16; //208; //104; //48;
            this.NumSamples = 100; //100; // this.NumTimeSteps;

            //environment-related constants
            this.NumStepsPerSeason = 5000; //10000;
            this.MaxMoveActionsRequired = 30;
            this.RandStart = true; //true; //false;

            //learning-related constants
            this.Temperature = 3.0f; // this.NumTimeSteps;
            this.Epsilon = 1.0f;
            this.Discount = 0.9f;
            this.LearningRate = 0.3f;
            this.MaximalChangeThreshold = 0.0001f; //0; //
            this.ExploratoryDecay = 1.0001f; //1.0001f; //1.00004f;

            //                                                      n    gr    c     v    er
            this.SingleTestParameters = new ArrayParameter(new[] { -.1, .10, -.1, .10, .60 });    //persistence
            //this.SingleTestParameters = new ArrayParameter(new[] { .00, .00, -.7, 0.1, 0.2 });    //persistence gp
            //this.SingleTestParameters = new ArrayParameter(new[] { .00, .10, .60, .00, .30 });    //diff prey season
            //this.SingleTestParameters = new ArrayParameter(new[] { .00, .20, -.5, .10, .20 });    //diff prey season gp
            //this.SingleTestParameters = new ArrayParameter(new[] { .10, -.2, .10, .00, .60 });    //poisoned prey season
            //this.SingleTestParameters = new ArrayParameter(new[] { .00, -.1, .40, -.1, .40 });    //poisoned prey season gp
            //this.SingleTestParameters = new ArrayParameter(new[] { .10, .00, -.2, .00, .70 });    //lairs
            //this.SingleTestParameters = new ArrayParameter(new[] { .10, .00, -.3, .40, .20 });    //lairs gp
            //this.SingleTestParameters = new ArrayParameter(new[] { -.4, .00, .00, .50, .10 });    //hunger-thirst
            //this.SingleTestParameters = new ArrayParameter(new[] { .00, .00, -.2, .50, .30 });    //hunger-thirst gp
            //this.SingleTestParameters = new ArrayParameter(new[] { .40, .00, -.1, .20, -.3 });    //moving preys
            //this.SingleTestParameters = new ArrayParameter(new[] { 1.0, .00, .00, .00, .00 });    //moving preys gp

            //this.SingleTestParameters = new ArrayParameter(new[] { .10, .10, .10, .60, .10 });    //eat all dots gp
            //this.SingleTestParameters = new ArrayParameter(new[] { .20, .10, .20, .20, .30 });    //pacman gp
            //this.SingleTestParameters = new ArrayParameter(new[] { -.2, .20, .10, .50, .00 });    //power pellets gp
            //this.SingleTestParameters = new ArrayParameter(new[] { .50, .00, .10, .20, .20 });    //rewarding dots gp

            //this.SingleTestParameters = new ArrayParameter(new[] { .00, .00, -.3, .00, .70 });    //universal
            //this.SingleTestParameters = new ArrayParameter(new[] { .00, .00, .00, .00, 1.0 });    //extrinsic only
            //this.SingleTestParameters = new ArrayParameter(new[] { .20, .20, .20, .20, .20 });    //equal-weights
            //this.SingleTestParameters = new ArrayParameter(new[] { .00, .00, .00, .00, .00 });    //random

            //this.SingleTestParameters = new ArrayParameter(new[] { .00, .00, .00, 1.00, .00 });    //one feature

            ((ArrayParameter) this.SingleTestParameters).Header = this.ParamIDNames;

            this.GraphicsEnabled = true;
            this.SingleTestType = (uint) TestType.Persistence;
            this.CellSize = 50;
            this.MaxCPUsUsed = ProcessUtil.GetCPUCount(); //3; 

            this.MultipleTestTypes = new []
                                     {
                                         (uint) TestType.Lairs,
                                         (uint) TestType.HungerThirst,
                                         (uint) TestType.MovingPreys,
                                         (uint) TestType.Persistence,
                                         (uint) TestType.PoisonedSeason,
                                         (uint) TestType.DifferentPreySeason,
                                         (uint) TestType.PowerPellets,
                                         (uint) TestType.RewardingDots,
                                         (uint) TestType.EatAllDots,
                                         (uint) TestType.Pacman,
                                         //(uint) TestType.AllForagingTests
                                     };
        }

        protected override string GetTestID(uint testType)
        {
            return ((TestType) testType).ToString();
        }

        public override IOptimizationTestFactory CreateTestFactory(
            IScenario scenario, uint numSimulations, uint numSamples)
        {
            return new EmotionalOptimizationTestFactory(
                    (IFitnessScenario) scenario.Clone(numSimulations, numSamples));
        }

        public override void Init()
        {
            this.EnvironmentFile = this.GetEnvironmentFilePath("ir-environment.xml");
            this.CornersEnvironmentFile = this.GetEnvironmentFilePath("corners-environment.xml");
            this.PacmanEnvironmentFile = this.GetEnvironmentFilePath("pacman-environment.xml");

            base.Init();
        }

        protected override void CreateTestsProfiles()
        {
            this.ScenarioProfiles = new Dictionary<uint, IScenario>();

            var testType = (uint) TestType.MovingPreys;
            this.ScenarioProfiles.Add(
                testType, this.CreateForagingTestProfile(
                    testType, new EmotionalAgent(), new MovingPreysEnvironment(), this.EnvironmentFile, 14));

            testType = (uint) TestType.DifferentPreySeason;
            this.ScenarioProfiles.Add(
                testType, this.CreateForagingTestProfile(
                    testType, new EmotionalAgent(), new PreySeasonEnvironment(), this.EnvironmentFile, 13));

            testType = (uint) TestType.Persistence;
            this.ScenarioProfiles.Add(
                testType, this.CreateForagingTestProfile(
                    testType, new EmotionalAgent(), new PersistenceEnvironment(), this.EnvironmentFile, 11));
            testType = (uint) TestType.PoisonedSeason;
            this.ScenarioProfiles.Add(
                testType, this.CreateForagingTestProfile(
                    testType, new EmotionalAgent(), new PoisonedSeasonEnvironment(), this.EnvironmentFile, 13));
            testType = (uint) TestType.HungerThirst;
            this.ScenarioProfiles.Add(
                testType, this.CreateForagingTestProfile(
                    testType, new HungerThirstAgent(), new HungerThirstEnvironment(), this.CornersEnvironmentFile, 50));

            testType = (uint) TestType.Lairs;
            this.ScenarioProfiles.Add(
                testType, this.CreateForagingTestProfile(
                    testType, new LairsAgent(), new LairsEnvironment(), this.CornersEnvironmentFile, 80));

            testType = (uint) TestType.Pacman;
            this.ScenarioProfiles.Add(
                testType, this.CreatePacManTestProfile(testType, 1200, false, false, true, true, 3, 0, 0, -0.1));

            testType = (uint) TestType.EatAllDots;
            this.ScenarioProfiles.Add(
                testType, this.CreatePacManTestProfile(testType, 1000, false, false, true, false, 3, 0.5, 0, -0.5));

            testType = (uint) TestType.PowerPellets;
            this.ScenarioProfiles.Add(
                testType, this.CreatePacManTestProfile(testType, 200, true, false, false, true, 0, 0.8, 0, -1));

            testType = (uint) TestType.RewardingDots;
            this.ScenarioProfiles.Add(
                testType, this.CreatePacManTestProfile(testType, 1000, false, false, false, false, 0, 0.8, 0.1, -1));

            testType = (uint) TestType.AllTests;
            this.ScenarioProfiles.Add(
                testType,
                new MultipleScenario(new List<IScenario>(this.ScenarioProfiles.Values))
                {
                    FilePath = this.GetFilePath(testType),
                    TestMeasuresFilePath = this.GetTestMeasuresFilePath(testType)
                });

            testType = (uint) TestType.AllPacManTests;
            this.ScenarioProfiles.Add(
                testType,
                new MultipleScenario(new List<IScenario>
                                     {
                                         this.ScenarioProfiles[(uint) TestType.EatAllDots],
                                         this.ScenarioProfiles[(uint) TestType.Pacman],
                                         this.ScenarioProfiles[(uint) TestType.RewardingDots],
                                         this.ScenarioProfiles[(uint) TestType.PowerPellets]
                                     })
                {
                    FilePath = this.GetFilePath(testType),
                    TestMeasuresFilePath = this.GetTestMeasuresFilePath(testType)
                });

            testType = (uint) TestType.AllForagingTests;
            this.ScenarioProfiles.Add(
                testType,
                new MultipleScenario(new List<IScenario>
                                     {
                                         this.ScenarioProfiles[(uint) TestType.MovingPreys],
                                         this.ScenarioProfiles[(uint) TestType.Persistence],
                                         this.ScenarioProfiles[(uint) TestType.HungerThirst],
                                         this.ScenarioProfiles[(uint) TestType.Lairs],
                                         this.ScenarioProfiles[(uint) TestType.PoisonedSeason],
                                         this.ScenarioProfiles[(uint) TestType.DifferentPreySeason]
                                     })
                {
                    FilePath = this.GetFilePath(testType),
                    TestMeasuresFilePath = this.GetTestMeasuresFilePath(testType)
                });
        }

        private PacmanScenario CreatePacManTestProfile(
            uint testType, uint maxStates, bool hideDots, bool hideBigDot,
            bool hideKeeperGhost, bool powerPelletEnabled, uint maxLives,
            double bigDotReward, double dotReward, double deathReward)
        {
            return new PacmanScenario(new PacmanAgent(), new PacmanEnvironment(), this)
                   {
                       FilePath = this.GetFilePath(testType),
                       EnvironmentConfigFile = this.PacmanEnvironmentFile,
                       TestMeasuresFilePath = this.GetTestMeasuresFilePath(testType),
                       FitnessText = SCORE_TEXT,
                       MaxStates = maxStates,
                       HideDots = hideDots,
                       HideBigDot = hideBigDot,
                       HideKeeperGhost = hideKeeperGhost,
                       PowerPelletEnabled = powerPelletEnabled,
                       MaxLives = maxLives,
                       BigDotReward = bigDotReward,
                       DotReward = dotReward,
                       DeathReward = deathReward
                   };
        }

        private SingleScenario CreateForagingTestProfile(
            uint testType, IAgent agent, IEnvironment environment, string environmentConfigFile, uint maxStates)
        {
            return new SingleScenario(agent, environment, this)
                   {
                       FilePath = this.GetFilePath(testType),
                       EnvironmentConfigFile = environmentConfigFile,
                       TestMeasuresFilePath = this.GetTestMeasuresFilePath(testType),
                       MaxStates = maxStates
                   };
        }
    }
}