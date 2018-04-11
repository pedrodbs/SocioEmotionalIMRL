// ------------------------------------------
// <copyright file="SocialGPTestsConfig.cs" company="Pedro Sequeira">
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
//    Project: Learning.Tests.MultiRewardFunctionGPOptimization
//    Last updated: 03/27/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using AForge.Genetic;
using PS.Learning.EvolutionaryComputation;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.IMRL.EC.Genes;
using PS.Learning.IMRL.EC.Testing;
using PS.Learning.IMRL.EC.Testing.MultipleTests;
using PS.Learning.Social.IMRL.EC.Chromosomes;
using PS.Learning.Social.IMRL.EC.Testing;
using PS.Learning.Social.IMRL.EC.Testing.MultipleTests;
using PS.Learning.Social.Testing;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;
using PS.Learning.Tests.AltruismOptimization.Testing;
using PS.Learning.Tests.MultiRewardFunctionGPOptimization.Domain.Agents;

namespace PS.Learning.Tests.MultiRewardFunctionGPOptimization.Testing
{
    public class SocialGPTestsConfig : GPTestsConfig, ISocialGPTestsConfig
    {
        public const string RAT_IMG_PREFIX = "lab/rat";
        public const string FOX_IMG_PREFIX = "fox";

        #region ISocialGPTestsConfig Members

        public bool StoreIndividualAgentStats { get; set; }
        public bool SameAgentParameters { get; set; }
        public uint NumAgents { get; set; }

        public override void SetDefaultConstants()
        {
            base.SetDefaultConstants();
            this.DestinationTestsDir = Path.GetFullPath("../../../../../../experiments/GENETICS 2013/");
            //this.PreviousPhaseID = "Top5";
            this.PreviousPhaseID = "General";
            this.TestMeasuresName = "TestMeasures";
            this.TestIDPrefix = "";

            //directories and config
            this.BaseFilePath = "./";
            this.EnvBaseFilePath = "../../../../../../experiments/multiagent-simple/";

            //test-related constants
            this.StdDevTimes = 2;
            this.NumTimeSteps = 5000;
            this.NumSamples = 100; //100; // NUM_TIME_STEPS;
            this.RandStart = true;
            this.NumSimulations = 104; // 16; // 104;
            this.StoreIndividualAgentStats = true;

            //genetic algorithms constants
            this.NumBaseVariables = 8;
            this.NumTestsPerIteration = 100; //10; //100;
            this.MaxIterations = 50; //25; //50;
            this.NumParallelOptimTests = 16; //16; //32; //56; //multiple of CPUs
            this.FitnessImprovementThreshold = 15; //15; //5; //MAX_GENERATIONS;
            this.RandomSelectionPortion = 0.2f;
            this.SteadyStatePortion = 0.1f;
            this.SymmetryFactor = 0.7f;
            this.MaxProgTreeDepth = 3; //4;

            //learning constants
            this.Epsilon = .0f;
            this.Discount = 0.9f;
            this.LearningRate = 0.3f;
            this.MaximalChangeThreshold = 0.0001f; //0; //
            this.ExploratoryDecay = 1.001f; //1.00005f; //1.0001f;

            //                      $16 $17 $18 $19 $20
            this.Constants = new[] {0d, 1, 2, 3, 5}; //, 7};

            this.SelectionMethod = new EliteSelection();
            //new RouletteWheelSelection();//new RankSelection();

            this.AllowedFunctions = //CustomGeneFunction.AllFunctions;
                new HashSet<FunctionType>
                {
                    FunctionType.Add,
                    FunctionType.Subtract,
                    FunctionType.Multiply,
                    FunctionType.Divide,
                    FunctionType.Exp,
                    FunctionType.Ln,
                    FunctionType.Sqrt
                };

            this.CellSize = 100; // 75;
            this.SingleTestType = (uint) TestType.S211;

            var chromosomes = new[] {"$0", "$0"};
            this.SingleTestParameters = new SocialGPChromosome(chromosomes);
            //var chromosome = new SocialGPChromosome(new[] {"$9 $0 + $9 + ", "$9 $9 $0 + $7 + + "}); //S211
            //var chromosome = new SocialGPChromosome(new[] { "0 $0 - ", "$0" });               //S2112Levers
            //var chromosome = new SocialGPChromosome(new[] { "$0", "$0" });                    //extrinsic
            //var chromosome = new SocialGPChromosome(2, new GPExpressionChromosome("$0 $9 +"));

            this.MultipleTestTypes = new[]
                                     {
                                         //(uint)TestType.S222,
                                         (uint) TestType.S211
                                         //(uint)TestType.S212,
                                      
                                         //(uint)TestType.S313,
                                         //(uint)TestType.S311,
                                         //(uint)TestType.S323,

                                         //(uint)TestType.S211Lever,
                                         //(uint)TestType.S211ElectricLever,
                                         //(uint)TestType.S2112Levers,
                                         //(uint)TestType.S211HeavyLever,
                                         //(uint)TestType.S2122Levers,
                                         //(uint)TestType.S2222Levers
                                     };
        }

        public override string GetTestName(IScenario scenario, ITestParameters testParameters)
        {
            return testParameters.ToString();
        }

        public override IOptimizationTestFactory CreateTestFactory(
            IScenario scenario, uint numSimulations, uint numSamples)
        {
            return
                new SocialGPOptimizationTestFactory(
                    (ISocialFitnessScenario) scenario.Clone(numSimulations, numSamples));
        }

        public override IGPSimplifierOptimizationTestFactory CreateSimplifierTestFactory(
            IScenario scenario, uint numSimulations, uint numSamples)
        {
            return new SocialGPSimplifierOptimizationTestFactory(
                (ISocialFitnessScenario) scenario.Clone(numSimulations, numSamples));
        }

        public override List<ITestParameters> GetSpecialTestParameters(IScenario scenario)
        {
            var specialParams = base.GetSpecialTestParameters(scenario);
            for (var i = 0; i < specialParams.Count; i++)
                specialParams[i] = new SocialGPChromosome(
                    ((ISocialScenario) scenario).NumAgents, (GPChromosome) specialParams[i]);
            return specialParams;
        }

        public override IECChromosome CreateBaseChromosome()
        {
            var gpChromosome = new GPChromosome(
                new FlexibleGPGene(this.AllowedFunctions.ToList(), (int) (this.Constants.Length + this.NumBaseVariables)));

            return this.SameAgentParameters
                ? (IECChromosome) new SocialCommonGPChromosome(this.NumAgents, gpChromosome)
                : new SocialGPChromosome(this.NumAgents, gpChromosome) {SymmetryFactor = this.SymmetryFactor};
        }

        #endregion

        protected override string GetTestID(uint testType)
        {
            return ((TestType) testType).ToString();
        }

        protected override void CreateTestsProfiles()
        {
            this.ScenarioProfiles = new Dictionary<uint, IScenario>();
            var testType = (uint) TestType.S212;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new FoodSharingEnvironment(),
                    "co-op-environment.xml", 2, 300, false, FOX_IMG_PREFIX, 1, 100, -.25));

            testType = (uint) TestType.S211;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new FoodSharingEnvironment(),
                    "stronger-co-op-environment.xml", 2, 300, true, FOX_IMG_PREFIX, 1, 100, -.25));

            testType = (uint) TestType.S222;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new FoodSharingEnvironment(),
                    "no-co-op-environment.xml", 2, 300, false, FOX_IMG_PREFIX, 2, 100, -.25));

            testType = (uint) TestType.S313;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new FoodSharingEnvironment(),
                    "1food-3pos-environment.xml", 3, 400, false, FOX_IMG_PREFIX, 1, 60, -.15));

            testType = (uint) TestType.S311;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new FoodSharingEnvironment(),
                    "1food-1pos-environment.xml", 3, 400, false, FOX_IMG_PREFIX, 1, 60, -.15));

            testType = (uint) TestType.S323;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new FoodSharingEnvironment(),
                    "2food-3pos-environment.xml", 3, 400, false, FOX_IMG_PREFIX, 2, 60, -.15));


            testType = (uint) TestType.S211ElectricLever;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new ElectricLeverEnvironment(),
                    "env-2111.xml", 2, 80, false, RAT_IMG_PREFIX, 1, 6, -.3));

            testType = (uint) TestType.S211Lever;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new LeverEnvironment(),
                    "env-2111.xml", 2, 80, false, RAT_IMG_PREFIX, 1, 6, -.3));

            testType = (uint) TestType.S211HeavyLever;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new HeavyLeverEnvironment(),
                    "env-2111.xml", 2, 80, false, RAT_IMG_PREFIX, 1, 8, -.18));

            testType = (uint) TestType.S2112Levers;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new TwoLeverEnvironment(),
                    "env-2112.xml", 2, 80, false, RAT_IMG_PREFIX, 2, 4, -.4));

            testType = (uint) TestType.S2122Levers;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new TwoLeverEnvironment(),
                    "env-2122.xml", 2, 80, false, RAT_IMG_PREFIX, 1, 8, -.2));

            testType = (uint) TestType.S2222Levers;
            this.ScenarioProfiles.Add(
                testType, this.CreateTestProfile(
                    testType, new FoodSharingGPAgent(), new TwoLeverEnvironment(),
                    "env-2122.xml", 2, 80, false, RAT_IMG_PREFIX, 2, 4, -.4));
        }

        protected virtual SocialScenario CreateTestProfile(
            uint testType, FoodSharingGPAgent agent, FoodSharingEnvironment environment, string envConfigFile,
            uint numAgents, uint maxStates, bool strongerAgent, string agentImgPrefix, uint numFoodResources,
            uint maxStepsWithoutEating, double hungryReward)
        {
            return new FoodSharingScenario(agent, environment, this)
                   {
                       FilePath = this.GetFilePath(testType),
                       EnvironmentConfigFile = this.GetEnvironmentFilePath(envConfigFile),
                       TestMeasuresFilePath = this.GetTestMeasuresFilePath(testType),
                       NumAgents = numAgents,
                       MaxStates = maxStates,
                       StrongerAgent = strongerAgent,
                       AgentImgPrefix = agentImgPrefix,
                       NumFoodResources = numFoodResources,
                       MaxStepsWithoutEating = maxStepsWithoutEating,
                       HungryReward = hungryReward
                   };
        }
    }
}