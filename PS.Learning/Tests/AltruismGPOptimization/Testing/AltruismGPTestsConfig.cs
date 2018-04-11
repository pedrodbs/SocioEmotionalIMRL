// ------------------------------------------
// <copyright file="AltruismGPTestsConfig.cs" company="Pedro Sequeira">
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
//    Project: Learning.Tests.AltruismGPOptimization
//    Last updated: 03/10/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AForge.Genetic;
using PS.Learning.EvolutionaryComputation;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.IMRL.EC.Genes;
using PS.Learning.IMRL.EC.Testing.MultipleTests;
using PS.Learning.Social.Domain;
using PS.Learning.Social.IMRL.EC.Chromosomes;
using PS.Learning.Social.IMRL.EC.Testing;
using PS.Learning.Social.Testing;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.MultipleTests;
using PS.Learning.Tests.AltruismGPOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;
using PS.Learning.Tests.AltruismOptimization.Testing;
using PS.Utilities.Core;

namespace PS.Learning.Tests.AltruismGPOptimization.Testing
{
    [Serializable]
    public class AltruismGPTestsConfig : AltruismTestsConfig, ISocialGPTestsConfig
    {
        #region ISocialGPTestsConfig Members

        public override void SetDefaultConstants()
        {
            base.SetDefaultConstants();

            //genetic programming constants
            this.NumBaseVariables = 8; //10;
            this.NumTestsPerIteration = 30; //30;//50; //10; //100;               (pop size)
            this.MaxIterations = 25; //25; //25; //50;                            (num generations)
            this.NumParallelOptimTests = 16; //8; //16; //32; //56;          multiple of CPUs (num pops)
            this.FitnessImprovementThreshold = 15; //15; //5; //this.MaxIterations;
            this.RandomSelectionPortion = 0.2f;
            this.SteadyStatePortion = 0.1f;
            this.SymmetryFactor = 0.7f;
            this.MaxProgTreeDepth = 4; //3; //4;

            //GP variables
            this.SelectionMethod = new EliteSelection(); //new RouletteWheelSelection();//new RankSelection();

            this.AllowedFunctions = //CustomGeneFunction.AllFunctions;
                new HashSet<FunctionType>
                {
                    FunctionType.Add,
                    FunctionType.Subtract,
                    FunctionType.Multiply,
                    FunctionType.Divide,
                    FunctionType.Exp,
                    FunctionType.Ln,
                    FunctionType.Sqrt,
                    FunctionType.Cos
                };

            //                      $8 $9 $10 $11 
            this.Constants = new[] {0d, 1, 2, 3}; // 5}; //, 7};

            // $0-Re; $1-Hng; $2-Fdif; $3-Ce; $4-Tle; $5-Cs; $6-Pssa $7-Obj
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$12 $6 sqrt / $9 $7 / + $0 + $1 +");

            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$9 $0 ln / cos ");       //211
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$9 $4 / $9 + ");         //211
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$9 $6 $4 * / ");         //212
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$0 $9 $1 exp + sqrt + ");  //S222
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$7 $4 / ");              //211Lever
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$0 $1 + ");              //211Lever
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$7 $0 sqrt + ");         //S211HeavyLever, S211ElectricLever
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$0 $6 - ");              //Hunting
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$9 $4 ln - ");           //HuntingSolo
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$5 $0 * exp ");          //S2112Levers
            //this.DefaultTestParameters = this.CreateCommonChromosome(2, "$1 $4 $7 - ln - ");      //S2112Levers

            this.SingleTestParameters = this.CreateCommonChromosome(2, "$0 "); //extrinsic only
            //this.DefaultTestParameters = this.CreateCommonChromosome(3, "0 ");                    //random agents

            this.SameAgentParameters = true;

            this.SingleTestType = (uint) TestType.S222;

            this.CellSize = 50;
            this.MaxCPUsUsed = ProcessUtil.GetCPUCount(); //3; //ProcessUtil.GetCPUCount();

            this.MultipleTestTypes = new[]
                                     {
                                         //(uint)TestType.S222,
                                         //(uint) TestType.S211,
                                         //(uint) TestType.S212,
                                      
                                         //(uint)TestType.S313,
                                         //(uint)TestType.S311,
                                         //(uint)TestType.S323,

                                         (uint) TestType.S211Lever
                                         ////(uint) TestType.S211ElectricLever,
                                         //(uint) TestType.S2112Levers,
                                         //(uint) TestType.S2112LeversCloser,
                                         //(uint) TestType.S211HeavyLever,
                                         ////(uint)TestType.S2122Levers,
                                         ////(uint)TestType.S2222Levers,
                                         ////(uint) TestType.Hunting,
                                         //(uint) TestType.SoloHunting,
                                     };
        }

        public int NumTestsPerIteration { get; set; }
        public uint MaxIterations { get; set; }
        public uint NumParallelOptimTests { get; set; }
        public uint FitnessImprovementThreshold { get; set; }
        public ISelectionMethod SelectionMethod { get; set; }
        public double RandomSelectionPortion { get; set; }
        public double SteadyStatePortion { get; set; }
        public double SymmetryFactor { get; set; }
        public int StdDevTimes { get; set; }
        public uint NumBaseVariables { get; set; }
        public double[] Constants { get; set; }
        public HashSet<FunctionType> AllowedFunctions { get; set; }
        public int MaxProgTreeDepth { get; set; }
        public int MaxInitialLevel { get; set; }

        public IGPSimplifierOptimizationTestFactory CreateSimplifierTestFactory(
            IScenario scenario, uint numSimulations, uint numSamples)
        {
            var profile = (ISocialFitnessScenario) scenario.Clone(numSimulations, numSamples);
            ((AltruismGPTestsConfig) profile.TestsConfig).SelectionMethod = this.SelectionMethod;
            return new AltruismGPSimplifierOptimizationTestFactory(profile);
        }

        public override IOptimizationTestFactory CreateTestFactory(
            IScenario scenario, uint numSimulations, uint numSamples)
        {
            var profile = (ISocialFitnessScenario) scenario.Clone(numSimulations, numSamples);
            ((AltruismGPTestsConfig) profile.TestsConfig).SelectionMethod = this.SelectionMethod;
            return new AltruismGPOptimizationTestFactory(profile);
        }

        public override string GetTestName(IScenario scenario, ITestParameters testParameters)
        {
            //gets test name from translated expression
            if (this.SameAgentParameters)
                return this.GetTranslatedExpression(scenario, testParameters);

            var translatedExpr = "";
            for (var i = 0u; i < ((ISocialScenario) scenario).NumAgents; i++)
                translatedExpr = string.Format("{0}{1};", translatedExpr,
                    this.GetTranslatedExpression(scenario, ((ISocialTestParameters) testParameters)[i]));
            return translatedExpr;
        }

        public override List<ITestParameters> GetSpecialTestParameters(IScenario scenario)
        {
            return new List<ITestParameters>
                   {
                       this.CreateCommonChromosome(((ISocialScenario) scenario).NumAgents, "$0 "),
                       this.CreateCommonChromosome(((ISocialScenario) scenario).NumAgents, "0 ")
                   };
        }

        public IECChromosome CreateBaseChromosome()
        {
            var gpChromosome = new GPChromosome(
                new FlexibleGPGene(this.AllowedFunctions.ToList(), (int) (this.Constants.Length + this.NumBaseVariables)));

            return this.SameAgentParameters
                ? (IECChromosome) new SocialCommonGPChromosome(this.NumAgents, gpChromosome)
                : new SocialGPChromosome(this.NumAgents, gpChromosome) {SymmetryFactor = this.SymmetryFactor};
        }

        #endregion

        protected string GetTranslatedExpression(IScenario scenario, ITestParameters testParameters)
        {
            var agent = new EchoGPSocialAgent
                        {
                            TestParameters = testParameters,
                            Scenario = (IFoodSharingScenario) scenario,
                            Environment = new FoodSharingEnvironment()
                        };
            agent.Init();
            return agent.MotivationManager.TranslatedExpression;
        }

        protected SocialCommonGPChromosome CreateCommonChromosome(uint numAgents, string expression)
        {
            return new SocialCommonGPChromosome(numAgents, new GPExpressionChromosome(expression));
        }

        protected override ISocialAgent CreateAgent()
        {
            return new EchoGPSocialAgent();
            return this.SimpleTests ? new SimpleEchoGPSocialAgent() : new EchoGPSocialAgent();
        }
    }
}