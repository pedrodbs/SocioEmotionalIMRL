// ------------------------------------------
// <copyright file="SocialSimulation.cs" company="Pedro Sequeira">
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

//    Last updated: 06/26/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Environments;
using PS.Learning.Core.Testing.Simulations;
using PS.Utilities.Math;

namespace PS.Learning.Social.Testing
{
    public class SocialSimulation : FitnessSimulation
    {
        protected readonly SocialEnvironment environment;

        public SocialSimulation(
            uint simulationIDx, IPopulation population,
            SocialEnvironment environment, ISocialFitnessScenario scenario)
            : base(simulationIDx, population[0], scenario)
        {
            this.environment = environment;

            //creates cumulative fitness quantities for all agents and population
            this.StatisticsCollection.Add(population.Fitness = this.CreateStatisticalQuantity(SCORE_ID));
            foreach (var agent in population)
                agent.Fitness = this.CreateStatisticalQuantity($"Ag{agent.AgentIdx}{SCORE_ID}");

            this.Population = population;
        }

        public override StatisticalQuantity Score => this.Population.Fitness;
        public new ISocialFitnessScenario Scenario => base.Scenario as ISocialFitnessScenario;
        public IPopulation Population { get; }

        public override void Update()
        {
            //first let agents choose next action
            foreach (var agent in this.Population)
                agent.ChooseNextAction();

            //then executes chosen actions
            foreach (var agent in this.Population)
                agent.ExecuteAction();

            //updates environment
            this.environment.Update();

            //finally let update agents
            foreach (var agent in this.Population)
                agent.Update();

            this.UpdateFitness();

            this.TimeStep++;
        }

        protected void UpdateFitness()
        {
            //updates overall and each agents' fitness
            foreach (var agent in this.Population)
                this.Scenario.AgentFitnessFunction.UpdateCurrentFitness(agent);
            this.Scenario.PopulationFitnessFunction.UpdateCurrentFitness(this.Population);
        }

        public override void Dispose()
        {
            //disposes all agents and quantities
            foreach (var agent in this.Population)
                agent.Dispose();

            this.Population.Clear();
            this.environment.Dispose();
            this.Score.Dispose();
        }
    }
}