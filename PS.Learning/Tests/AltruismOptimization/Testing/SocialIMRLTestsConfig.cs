// ------------------------------------------
// SocialIMRLTestsConfig.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2015/3/31
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PS.Learning.IMRL.Testing.Config;
using PS.Learning.Social.Testing;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Utilities.Collections;

namespace PS.Learning.Tests.AltruismOptimization.Testing
{
    [Serializable]
    public abstract class SocialIMRLTestsConfig : IMRLTestsConfig, ISocialTestsConfig
    {
        #region ISocialTestsConfig Members

        public bool StoreIndividualAgentStats { get; set; }
        public bool SameAgentParameters { get; set; }
        public uint NumAgents { get; set; }

        public override List<ITestParameters> GetSpecialTestParameters(IScenario scenario)
        {
            var specialParams = base.GetSpecialTestParameters(scenario);
            for (var i = 0; i < specialParams.Count; i++)
                specialParams[i] = new SocialArrayParameter(
                    ((SocialScenario) scenario).NumAgents, (IArrayParameter) specialParams[i]);
            return specialParams;
        }

        public override string GetTestName(IScenario scenario, ITestParameters testParameters)
        {
            //create test name from parameters, eg (a-0.1_b0.7_c0.2)
            var paramLetterIDs = this.ParamIDLetters;

            var socialTestProfile = (ISocialScenario) scenario;
            var sb = new StringBuilder("(");

            for (var agIdx = 0u; agIdx < socialTestProfile.NumAgents; agIdx++)
            {
                var arrayParameter = ((SocialArrayParameter) testParameters)[agIdx];
                for (var i = 0; i < arrayParameter.Length; i++)
                    sb.AppendFormat("{0}{1}{2:0.0}_", paramLetterIDs[i], agIdx, arrayParameter[i]);
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            return sb.ToString();
        }

        public override List<ITestParameters> GetOptimizationTestParameters()
        {
            var allArrayParameters = base.GetOptimizationTestParameters();

            if (this.SameAgentParameters)
            {
                //generates test parameters where all agents within the group have the same parameters
                var testParameters = allArrayParameters.Select(
                    arrayParameter => new SocialArrayParameter(
                        this.NumAgents,
                        (IArrayParameter) arrayParameter)).Cast<ITestParameters>().ToList();
                return testParameters;
            }

            // generates all possible permutations of parameters for the agents in each different group
            var allPermutations =
                allArrayParameters.Cast<ArrayParameter>().ToArray().AllCombinations(this.NumAgents, true);
            //ArrayUtil<ArrayParameter>.AllPermutations(
            //    allArrayParameters.ToArray(), this.TestProfile.NumAgents, true);

            return new List<ITestParameters>(
                allPermutations.Select(parametersComb => new SocialArrayParameter(parametersComb)));
        }

        #endregion
    }
}