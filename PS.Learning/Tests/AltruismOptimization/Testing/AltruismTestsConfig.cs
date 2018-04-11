// ------------------------------------------
// <copyright file="AltruismTestsConfig.cs" company="Pedro Sequeira">
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
//    Project: Learning.Tests.AltruismOptimization
//    Last updated: 03/10/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Testing;
using PS.Learning.Social.Testing.FitnessFunctions;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;
using PS.Utilities.Core;
using PS.Utilities.Collections;

namespace PS.Learning.Tests.AltruismOptimization.Testing
{

    #region TestType enum

    public enum TestType
    {
        S212,
        S211,
        S222,
        S313,
        S311,
        S323,
        S211ElectricLever,
        S211Lever,
        S211HeavyLever,
        S2112Levers,
        S2112LeversCloser,
        S2122Levers,
        S2222Levers,
        Hunting,
        HuntingSolo
    }

    #endregion

    [Serializable]
    public class AltruismTestsConfig : SocialIMRLTestsConfig
    {
        public const string RAT_IMG_PREFIX = "lab/rat";
        public const string FOX_IMG_PREFIX = "fox";
        public const string CHEESE_IMG_PATH = "../../../../bin/resources/lab/cheese.png";
        public const string RABBIT_IMG_PATH = "../../../../bin/resources/rabbit.png";
        private const string FITNESS_TEXT = "Score";
        public bool SimpleTests { get; set; }

        public override void SetDefaultConstants()
        {
            base.SetDefaultConstants();

            this.SimpleTests = true;

            this.DestinationTestsDir = Path.GetFullPath("../../../../../../experiments/2014-SOCIAL");
            this.PreviousPhaseID = "Top5"; //ParallelOptimizationTest.OPTIM_TESTS_ID;
            this.TestIDPrefix = "";

            var envBasePath = string.Format("multiagent{0}", (this.SimpleTests ? "-medium/" : "/"));
            //"-simple/" : "/");
            this.EnvBaseFilePath += envBasePath;
            this.CondorServerBaseEnvPath += envBasePath;
            this.TestListFilePath = Path.GetFullPath("../../../../bin/config/testsconfig.csv");

            //IMRL-related constants
            var socInt = new StepInterval<double>(.0, .8, .1); // .2m; //.25m;
            var fitInt = new StepInterval<double>(.2, 1, .2); // .2m; //.25m;
            this.ParamsStepIntervals = new[] {socInt, socInt, socInt, socInt, socInt, socInt, fitInt};
            this.SameAgentParameters = true; // false; // true;
            this.ParamIDNames = new[] {"Worse", "Hunger", "Pres", "Ext Echo", "Int Echo", "Perf", "Fit"};
            this.ParamIDLetters = new[] {'w', 'h', 'p', 'e', 'i', 'o', 'f'};

            //test-related constants
            this.NumTimeSteps = this.SimpleTests ? 10000u : 50000u; //5000u : 50000u; // 100000;
            this.NumSimulations = 16; //1; //8; //16; //208; //104; //48;
            this.NumSamples = 100; // this.NumTimeSteps;
            this.RandStart = true;

            //learning-related constants
            this.Epsilon = 1.0;
            this.Discount = 0.9;
            this.LearningRate = 0.3; //0.9; // 0.3;
            this.MaximalChangeThreshold = 0.0001; //0; //
            this.ExploratoryDecay = this.SimpleTests ? 1.001 : 1.0002; //1.001 : 1.0002; // 1.0001; //1.00005;


            this.CellSize = this.SimpleTests ? 100 : 50;
            this.MaxCPUsUsed = ProcessUtil.GetCPUCount(); //3; //ProcessUtil.GetCPUCount();

            //                                                          'w', 'h', 'p', 'e', 'i', 'o', 'f'
            //this.DefaultTestParameters = new SocialArrayParameter(3, new[] { -.2, 0, .8 });   // 212
            //this.DefaultTestParameters = new SocialArrayParameter(3, new[] {0, .8, .2});      // 211
            //this.DefaultTestParameters = new SocialArrayParameter(3, new[] { -.4, -.2, .4 }); // S211ElectricLever

            this.SingleTestParameters = new SocialArrayParameter(3, new[] {.14, .14, .14, .14, .14, .15, .15});
            //equal weights
            //this.DefaultTestParameters = new SocialArrayParameter(3, new[] {0, 0, 1d}); //extrinsic only
            //this.DefaultTestParameters = new SocialArrayParameter(3, new[] { 0, 0, 0d });     //random agents

            this.SingleTestType = (uint) TestType.S212;

            //creates list of tests types to run
            this.MultipleTestTypes = new[]
                                     {
                                         //(uint) TestType.S222,
                                         (uint) TestType.S211
                                         //(uint) TestType.S212,
                                         //(uint) TestType.S211ElectricLever,
                                         //(uint) TestType.S211Lever,
                                         //(uint) TestType.S211HeavyLever,
                                         //(uint) TestType.S2112Levers,
                                         //(uint) TestType.S2122Levers,
                                         //(uint) TestType.S2222Levers
                                      
                                         //(uint)TestType.S313,
                                         //(uint)TestType.S311,
                                         //(uint)TestType.S323,
                                     };
        }

        public override IOptimizationTestFactory CreateTestFactory(
            IScenario scenario, uint numSimulations, uint numSamples)
        {
            return new AltruismOptimizationTestFactory(
                (ISocialFitnessScenario) scenario.Clone(numSimulations, numSamples));
        }

        protected override string GetTestID(uint testType)
        {
            return ((TestType) testType).ToString();
        }

        protected override void CreateTestsProfiles()
        {
            if (this.SimpleTests)
                this.CreateSimpleTestsProfiles();
            else
                this.CreateNormalTestsProfiles();
        }

        protected void CreateSimpleTestsProfiles()
        {
            this.ScenarioProfiles = new Dictionary<uint, IScenario>();
            var testType = (uint) TestType.S212;
            var stepsToEat = 5u; //2u;
            var numPos = 13u; //5u; //3u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-212.xml", 2, 32*numPos, false, FOX_IMG_PREFIX, 1, (2*stepsToEat) + 1, -(1d/stepsToEat) - .1,
                    false, false));

            testType = (uint) TestType.S211;
            stepsToEat = 7u; //5u; //2u;
            numPos = 13u; //5u; //2u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-211.xml", 2, 64*numPos, true, FOX_IMG_PREFIX, 1, (2*stepsToEat) + 1, -(1d/stepsToEat) - .1,
                    false, false));

            testType = (uint) TestType.S222;
            stepsToEat = 5u; //3u; //2u;
            numPos = 13u; //7u; //2u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-222.xml", 2, 64*numPos, false, FOX_IMG_PREFIX, 2, (2*stepsToEat) + 1, -(1d/stepsToEat) - .1,
                    false, false));

            testType = (uint) TestType.S313;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-313.xml", 3, 64*4, false, FOX_IMG_PREFIX, 1, 10, -.2, false, false));

            testType = (uint) TestType.S311;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-311.xml", 3, 64*3, false, FOX_IMG_PREFIX, 1, 10, -.2, false, false));

            testType = (uint) TestType.S323;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-323.xml", 3, 64*5, false, FOX_IMG_PREFIX, 2, 6, -.7, false, false));


            testType = (uint) TestType.S211ElectricLever;
            stepsToEat = 4u; //8u; //20; //6u; //3u;
            numPos = 7u; //5u; //3u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new ElectricLeverEnvironment(),
                    "env-2111.xml", 2, 32*numPos, false, RAT_IMG_PREFIX, 1, (2*stepsToEat) + 1, -(1d/stepsToEat) - .1,
                    false, false));
            //     -0.1, false, false));


            testType = (uint) TestType.S211Lever;
            stepsToEat = 4u; //6u; //3u;
            numPos = 13u; //5u; //3u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new LeverEnvironment(),
                    "env-2111.xml", 2, 64*numPos, false, RAT_IMG_PREFIX, 1, (2*stepsToEat) + 1, -(1d/stepsToEat) - .1,
                    false, false));

            testType = (uint) TestType.S211HeavyLever;
            stepsToEat = 10; //8u; //4u;
            numPos = 7u; //5u;//3u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new HeavyLeverEnvironment(),
                    "env-2111.xml", 2, 64*numPos, false, RAT_IMG_PREFIX, 1, (2*stepsToEat) + 1, -(1d/stepsToEat) - .1,
                    false, false));

            testType = (uint) TestType.S2112Levers;
            stepsToEat = 7u; //3u;
            numPos = 7u; //3u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new TwoLeverEnvironment(),
                    "env-2112.xml", 2, 64*numPos, false, RAT_IMG_PREFIX, 1, (2*stepsToEat) + 1, -(1d/stepsToEat) - .1,
                    false, false));

            testType = (uint) TestType.S2112LeversCloser;
            stepsToEat = 8u; //3u;
            numPos = 7u; //3u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new TwoLeverEnvironment(),
                    "env-2112-closer.xml", 2, 64*numPos, false, RAT_IMG_PREFIX, 1, (2*stepsToEat) + 1,
                    -(1d/stepsToEat) - .1, false, false));

            testType = (uint) TestType.S2122Levers;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new TwoLeverEnvironment(),
                    "env-2122.xml", 2, 64*4, false, RAT_IMG_PREFIX, 2, 8, -.2, false, false));

            testType = (uint) TestType.S2222Levers;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new TwoLeverEnvironment(),
                    "env-2122.xml", 2, 64*4, false, RAT_IMG_PREFIX, 2, 4, -.4, false, false));


            testType = (uint) TestType.Hunting;
            stepsToEat = this.NumTimeSteps; //6u; //3u;
            numPos = 13u; //3u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new HuntingEnvironment(),
                    "env-hunting.xml", 2, 32*numPos, false, FOX_IMG_PREFIX, 1, (2*stepsToEat) + 1, 0, true, false));

            testType = (uint) TestType.HuntingSolo;
            stepsToEat = this.NumTimeSteps; //6u; //3u;
            numPos = 13u; //3u;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new HuntingEnvironment(),
                    "env-hunting.xml", 2, 32*numPos, false, FOX_IMG_PREFIX, 1, (2*stepsToEat) + 1, 0, true, true));
        }

        protected void CreateNormalTestsProfiles()
        {
            this.ScenarioProfiles = new Dictionary<uint, IScenario>();
            var testType = (uint) TestType.S212;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-212.xml", 2, 600, false, FOX_IMG_PREFIX, 1, 60, -.3, false, false)); //18, -.15));

            testType = (uint) TestType.S211ElectricLever;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new ElectricLeverEnvironment(),
                    "env-2111.xml", 2, 1000, false, RAT_IMG_PREFIX, 1, 100, -.05, false, false));

            testType = (uint) TestType.S211;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-211.xml", 2, 600, true, FOX_IMG_PREFIX, 1, 22, -.1, false, false));

            testType = (uint) TestType.S222;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-222.xml", 2, 160, false, FOX_IMG_PREFIX, 2, 14, -.15, false, false));

            testType = (uint) TestType.S313;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-313.xml", 3, 400, false, FOX_IMG_PREFIX, 1, 60, -.15, false, false));

            testType = (uint) TestType.S311;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-311.xml", 3, 400, false, FOX_IMG_PREFIX, 1, 60, -.15, false, false));

            testType = (uint) TestType.S323;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new FoodSharingEnvironment(),
                    "env-323.xml", 3, 400, false, FOX_IMG_PREFIX, 2, 60, -.15, false, false));


            testType = (uint) TestType.S211Lever;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new LeverEnvironment(),
                    "env-2111.xml", 2, 180, false, RAT_IMG_PREFIX, 1, 200, -.05, false, false));

            testType = (uint) TestType.S211HeavyLever;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new HeavyLeverEnvironment(),
                    "env-2111.xml", 2, 320, true, RAT_IMG_PREFIX, 1, 100, -.01, false, false));

            testType = (uint) TestType.S2112Levers;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new TwoLeverEnvironment(),
                    "env-2112.xml", 2, 210, false, RAT_IMG_PREFIX, 1, 100, -.4, false, false));

            testType = (uint) TestType.S2122Levers;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new TwoLeverEnvironment(),
                    "env-2122.xml", 2, 230, false, RAT_IMG_PREFIX, 1, 100, -.2, false, false));

            testType = (uint) TestType.S2222Levers;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, this.CreateAgent(), new TwoLeverEnvironment(),
                    "env-2122.xml", 2, 250, false, RAT_IMG_PREFIX, 2, 100, -.4, false, false));
        }

        protected virtual ISocialAgent CreateAgent()
        {
            return this.SimpleTests ? new SimpleAltruisticAgent() : new AltruisticAgent();
        }

        protected virtual SocialScenario CreateTestProfile(
            uint testType, ISocialAgent agent, FoodSharingEnvironment environment, string envConfigFile,
            uint numAgents, uint maxStates, bool strongerAgent, string agentImgPrefix, uint numFoodResources,
            uint maxStepsWithoutEating, double hungryReward, bool seeFoodFromAfar, bool soloHunting)
        {
            return new FoodSharingScenario(agent, environment, this)
                   {
                       FilePath = this.GetFilePath(testType),
                       EnvironmentConfigFile = this.GetEnvironmentFilePath(envConfigFile),
                       TestMeasuresFilePath = this.GetTestMeasuresFilePath(testType),
                       FitnessText = FITNESS_TEXT,
                       NumAgents = numAgents,
                       MaxStates = maxStates,
                       StrongerAgent = strongerAgent,
                       AgentImgPrefix = agentImgPrefix,
                       NumFoodResources = numFoodResources,
                       MaxStepsWithoutEating = maxStepsWithoutEating,
                       HungryReward = hungryReward,
                       SeeFoodFromAfar = seeFoodFromAfar,
                       SoloHunting = soloHunting,
                       PopulationFitnessFunction = new AvgMinStdDevPopFitFunction()
                   };
        }
    }
}