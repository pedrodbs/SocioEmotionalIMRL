// ------------------------------------------
// <copyright file="SocialFitnessTest.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.Social

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Testing.Simulations;
using PS.Learning.Core.Testing.SingleTests;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Environments;
using PS.Utilities.Forms.Math;
using PS.Utilities.Math;
using System.Linq;

namespace PS.Learning.Social.Testing
{
    public abstract class SocialFitnessTest : FitnessTest
    {
        #region Public Fields

        public const string AG_FITNESS_STAT_ID = "AgFitness";

        #endregion Public Fields

        #region Protected Constructors

        protected SocialFitnessTest(
            ISocialFitnessScenario scenario, ISocialTestParameters testParameters)
            : base(scenario, testParameters)
        {
        }

        #endregion Protected Constructors

        #region Public Properties

        protected new ISocialFitnessScenario Scenario => base.Scenario as ISocialFitnessScenario;

        public new ISocialTestParameters TestParameters => base.TestParameters as ISocialTestParameters;

        protected new ISocialTestsConfig TestsConfig => base.TestsConfig as ISocialTestsConfig;

        #endregion Public Properties

        #region Public Methods

        public override Simulation CreateAndSetupSimulation(uint simulationIDx)
        {
            var population = new Population();

            for (var i = 0u; i < this.Scenario.NumAgents; i++)
            {
                var agent = (ISocialAgent)this.CreateAgent();
                agent.AgentIdx = i;
                agent.IdToken = $"agent{i + 1}";
                population.Add(agent);
            }

            //creates new environment and cells
            var environment = (SocialEnvironment)this.CreateEnvironment();
            if (environment != null)
            {
                //sets environment for all agents
                environment.SetAgents(population);
                foreach (var socialAgent in population)
                    socialAgent.Environment = environment;
                environment.CreateCells(3, 3, this.Scenario.EnvironmentConfigFile);
                environment.Scenario = this.Scenario;
                environment.Init();
                environment.Reset();
            }

            //sets parameters for all agents in the simulation
            foreach (var agent in population)
                this.SetAgentParameters(agent);

            //creates and returns simulation
            return this.CreateSimulation(simulationIDx, population, environment);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual SocialSimulation CreateSimulation(
            uint simulationIDx, IPopulation population, SocialEnvironment environment)
        {
            return new SocialSimulation(
                simulationIDx, population, environment, this.Scenario);
        }

        protected string GetAgentFitnessStatID(uint agentIdx)
        {
            return $"{agentIdx}{AG_FITNESS_STAT_ID}";
        }

        protected StatisticalQuantity GetAgentQuantityAverage(uint agentIdx, string quantityName)
        {
            return this.testStatisticsAvg[this.GetAgentStatID(agentIdx, quantityName)];
        }

        protected string GetAgentStatID(uint agentIdx, string quantityName)
        {
            return $"{agentIdx}{quantityName}";
        }

        protected virtual StatisticsCollection GetPopulationStatsCollectionAvg(IPopulation population)
        {
            if ((population == null) || (population.Count == 0)) return null;
            var popStatsCollectionAvg = new StatisticsCollection();

            //for each agent statistic
            foreach (var statKey in population[0].StatisticsCollection.Keys)
            {
                //average statistic between all agents
                var stat = StatisticalQuantity.GetQuantitiesAverage(
                    population.Select(ag => ag.StatisticsCollection[statKey]).ToList());

                popStatsCollectionAvg.Add(statKey, stat);
            }
            return popStatsCollectionAvg;
        }

        protected override StatisticsCollection GetTestStatistics(Simulation simulation)
        {
            if (!(simulation is SocialSimulation)) return base.GetTestStatistics(simulation);
            var socialSimulation = (SocialSimulation)simulation;

            //adds all statistics relevant for the test from the given simulation
            // for multi-agent tests, this includes simulation stats..
            var statistics = simulation.StatisticsCollection.Clone();

            if (!this.LogStatistics) return statistics;

            //.. the population's "avg agent" statistics
            statistics.AddRange(this.GetPopulationStatsCollectionAvg(socialSimulation.Population));

            if (!this.TestsConfig.StoreIndividualAgentStats) return statistics;

            //..and each agent's fitness and stats
            foreach (var agent in socialSimulation.Population)
            {
                var agentFitnessStatID = this.GetAgentFitnessStatID(agent.AgentIdx);
                statistics.Add(agentFitnessStatID, agent.Fitness.Clone());
                statistics.AddRange(agent.StatisticsCollection.Clone(), $"{agent.AgentIdx}");
            }

            return statistics;
        }

        protected override void PrintAgent()
        {
            if (!this.LogStatistics) return;

            //prints avg agent statistics
            this.PrintStatistic("Reward", "/STM/Reward.csv");

            if (!this.TestsConfig.StoreIndividualAgentStats) return;

            //prints agent statistics
            const int maxAgents = 3;
            for (uint i = 0; i < maxAgents; i++)
                this.PrintAgent(i);
        }

        protected virtual void PrintAgent(uint agentIdx)
        {
            this.WriteLine($@"Printing {agentIdx} statistical quantities...");

            //this.PrintAgentQuantity(agentID, "StateActionValue", "/" + agentID + "/STM/StateActionValueAvg.csv");
            this.PrintAgentQuantity(agentIdx, "Epsilon", $"/{agentIdx}/Learning/EpsilonAvg.csv");
            //this.PrintAgentQuantity(agentID, "PredictionError", "/" + agentID + "/Learning/PredictionErrorAvg.csv");
            //this.PrintAgentQuantity(agentID, "NumBackups", string.Format("/{0}/Learning/NumBackups.csv", agentID));

            var rewardQuantityList =
                new StatisticsCollection
                {
                    //{"Reward", this.GetAgentQuantityAverage(agentID, "Reward")},
                    {"ExtrinsicReward", this.GetAgentQuantityAverage(agentIdx, "ExtrinsicReward")},
                    {"IntrinsicReward", this.GetAgentQuantityAverage(agentIdx, "IntrinsicReward")}
                };

            rewardQuantityList.PrintAllQuantitiesToCSV($"{this.FilePath}/{agentIdx}/STM/RewardAvg.csv");
        }

        protected void PrintAgentQuantity(uint agentIdx, string quantityName, string quantityFilePath)
        {
            this.GetAgentQuantityAverage(agentIdx, quantityName)
                .PrintStatisticsToCSV($"{this.FilePath}{quantityFilePath}");
        }

        protected override void PrintPerformanceResults()
        {
            //prints agents' cumulative fitness
            this.WriteLine(@"Printing agents' performances...");

            if (this.TestsConfig.StoreIndividualAgentStats)
            {
                var agentsFitnessList = new StatisticsCollection();
                for (uint i = 0; i < this.TestsConfig.NumAgents; i++)
                    agentsFitnessList.Add($"Agent{i}",
                        this.testStatisticsAvg[this.GetAgentFitnessStatID(i)]);

                agentsFitnessList.PrintAllQuantitiesToCSV($"{this.FilePath}/AgentsCumulativeFitnessAvg.csv");
                //agentsFitnessList.PrintAllQuantitiesToImage($"{this.FilePath}/AgentsCumulativeFitnessAvg.png");
            }

            //prints overall cumulative fitness
            this.SimulationScoreAvg.PrintStatisticsToCSV($"{this.FilePath}/CumulativeFitnessAvg.csv");
            this.FinalScores.PrintStatisticsToCSV($"{this.FilePath}/CumulativeFitnessValues.csv", false, true, ChartType.Column);
        }

        protected override void SetAgentParameters(IAgent agent)
        {
            //inits agent parameters according to agent index
            agent.TestParameters = this.TestParameters[((ISocialAgent)agent).AgentIdx];
            agent.Scenario = this.Scenario;
            agent.Init();
        }

        #endregion Protected Methods
    }
}