// ------------------------------------------
// <copyright file="Simulation.cs" company="Pedro Sequeira">
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
//    Project: Learning
//    Last updated: 06/26/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Domain.Agents;
using PS.Utilities.Core;
using PS.Utilities.Math;

namespace PS.Learning.Core.Testing.Simulations
{
    public abstract class Simulation : ISimulation, IProgressHandler, IUpdatable
    {
        #region Constructor

        protected Simulation(uint simulationIDx, IAgent agent, IScenario scenario)
        {
            //checks arguments
            if (agent == null) throw new ArgumentNullException(nameof(agent), "Agent can't be null");

            this.ID = simulationIDx;
            this.Agent = agent;
            this.Scenario = scenario;

            this.StatisticsCollection = new StatisticsCollection();
        }

        #endregion

        #region Properties

        public ITestsConfig TestsConfig => this.Scenario.TestsConfig;

        #endregion

        #region IProgressHandler Members

        public virtual double ProgressValue => (double) this.TimeStep/this.TestsConfig.NumTimeSteps;

        #endregion

        #region ISimulation Members

        /// <summary>
        ///     Collection of <see cref="StatisticalQuantity" /> relevant for this simulation.
        /// </summary>
        public StatisticsCollection StatisticsCollection { get; }

        public IScenario Scenario { get; }
        public long MemoryUsage => this.performanceMeasure.MemoryUsage;
        public TimeSpan TimeElapsed => this.performanceMeasure.TimeElapsed;
        public IAgent Agent { get; set; }
        public LogWriter LogWriter { get; protected set; }
        public abstract StatisticalQuantity Score { get; }
        public uint ID { get; }
        public ulong TimeStep { get; protected set; }

        #endregion

        #region Fields

        public const string SCORE_ID = "Score";
        protected PerformanceMeasure performanceMeasure = new PerformanceMeasure();

        #endregion

        #region Public Methods

        public virtual bool Run()
        {
            this.StartSimulation();
            this.RunSimulation();
            this.FinishSimulation();

            return true;
        }

        public virtual void Reset()
        {
            this.performanceMeasure.Reset();
        }

        public virtual void Dispose()
        {
            this.Agent.Dispose();
            this.StatisticsCollection.Dispose();
        }

        public virtual void Update()
        {
            //updates agent
            this.Agent.Update();

            this.TimeStep++;
        }

        public virtual void StartSimulation()
        {
            //starts performance measures 
            this.performanceMeasure.Start();
        }

        public virtual void RunSimulation()
        {
            //updates simulation while simulation doesn't terminate
            while (!this.SimulationFinished())
                this.Update();
        }

        public virtual bool SimulationFinished()
        {
            //generic simulation terminates when it performs maximum steps
            return this.TimeStep > this.TestsConfig.NumTimeSteps;
        }

        public virtual void FinishSimulation()
        {
            //stops performance measures
            this.performanceMeasure.Stop();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates a statistical quantity with sample parameters according to <see cref="TestsConfig" />.
        /// </summary>
        protected StatisticalQuantity CreateStatisticalQuantity(string id)
        {
            return new StatisticalQuantity(this.TestsConfig.NumSamples)
                   {
                       Id = id,
                       SampleSteps = this.TestsConfig.NumTimeSteps/this.TestsConfig.NumSamples
                   };
        }

        #endregion
    }
}