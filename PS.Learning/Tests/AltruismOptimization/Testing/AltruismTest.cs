// ------------------------------------------
// AltruismTest.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/03/26
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using PS.Learning.Social.Domain;
using PS.Learning.Social.Testing;
using PS.Utilities.IO;
using PS.Utilities.Math;
using System.Collections.Generic;

namespace PS.Learning.Tests.AltruismOptimization.Testing
{
    public class AltruismTest : SocialFitnessTest
    {
        #region Public Constructors

        public AltruismTest(ISocialFitnessScenario scenario, ISocialTestParameters testParameters)
            : base(scenario, testParameters)
        {
        }

        #endregion Public Constructors

        #region Protected Methods

        protected StatisticsCollection GetSignalQuantityList(ISocialAgent agent)
        {
            return new StatisticsCollection
                   {
                       //{"Internal L-signal", this.GetAgentQuantityAverage(agent, "Internal L-signal")},
                       //{"External L-signal", this.GetAgentQuantityAverage(agent, "External L-signal")}
                       {"OtherHungerSignal", this.GetAgentQuantityAverage(agent.AgentIdx, "OtherHungerSignal")},
                       {"OtherWorseSignal", this.GetAgentQuantityAverage(agent.AgentIdx, "OtherWorseSignal")},
                       {"OtherPresenceSignal", this.GetAgentQuantityAverage(agent.AgentIdx, "OtherPresenceSignal")},
                       {"IntPerfSignal", this.GetAgentQuantityAverage(agent.AgentIdx, "IntPerfSignal")},
                       {"IntEchoSignal", this.GetAgentQuantityAverage(agent.AgentIdx, "IntEchoSignal")},
                       {"ExtEchoSignal", this.GetAgentQuantityAverage(agent.AgentIdx, "ExtEchoSignal")}
                   };
        }

        protected override void PrintAgent()
        {
            base.PrintAgent();

            if (!this.LogStatistics) return;

            //prints agent statistics
            var agentNum = 0u;
            foreach (var agent in ((SocialSimulation)this.lastSimulation).Population)
            {
                var signalQuantityList = this.GetSignalQuantityList(agent);

                signalQuantityList.PrintAllQuantitiesToCSV(string.Format("{0}/{1}/STM/SignalsAvg.csv", this.FilePath, agent));

                this.PrintAgentQuantity(agent.AgentIdx, "Cumulative Social Encounters",
                    string.Format("/{0}/STM/CumulativeSocialEncounters.csv", agent));

                var path = string.Format("{0}/{1}/LTM", this.FilePath, agent);
                PathUtil.CreateOrClearDirectory(path);
                agent.LongTermMemory.PrintResults(path);

                //this.GetAgentQuantityAverage(agentID, "Hungry").PrintStatisticsToCSV(
                //    string.Format("{0}/{1}/STM/Hungry.csv", this.FilePath, agentID));

                if (++agentNum >= 3) break;
            }
        }

        #endregion Protected Methods
    }
}