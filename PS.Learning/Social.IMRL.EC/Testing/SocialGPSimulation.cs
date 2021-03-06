﻿// ------------------------------------------
// <copyright file="SocialGPSimulation.cs" company="Pedro Sequeira">
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
//    Project: Learning.Social.IMRL.EC

//    Last updated: 3/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Environments;
using PS.Learning.Social.IMRL.EC.Domain;
using PS.Learning.Social.Testing;

namespace PS.Learning.Social.IMRL.EC.Testing
{
    public class SocialGPSimulation : SocialSimulation
    {
        public SocialGPSimulation(
			uint simulationIDx, IPopulation population, 
			SocialEnvironment environment, ISocialFitnessScenario scenario)
			: base(simulationIDx, population, environment, scenario)
        {
        }

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

            //let agents update (their own intrinsic reward)
            foreach (var agent in this.Population)
                agent.Update();

            //finally let agents update reward with the other's reward as well
            foreach (ISocialGPAgent agent in this.Population)
                agent.UpdateIntrinsicReward();

            //update fitness
            this.UpdateFitness();
        }
    }
}