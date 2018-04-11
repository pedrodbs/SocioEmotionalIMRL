// ------------------------------------------
// FoodSharingTestProfile.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/19
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Environments;
using PS.Learning.Social.Testing;

namespace PS.Learning.Tests.AltruismOptimization.Testing
{
    [Serializable]
    public class FoodSharingScenario : SocialScenario, IFoodSharingScenario
    {
        public FoodSharingScenario(
            ISocialAgent baseAgent, ISocialEnvironment baseEnvironment, ISocialTestsConfig testsConfig)
            : base(baseAgent, baseEnvironment, testsConfig)
        {
        }

        public new ISocialTestsConfig TestsConfig
        {
            get { return base.TestsConfig as ISocialTestsConfig; }
        }

        #region IFoodSharingTestProfile Members

        public uint MaxStepsWithoutEating { get; set; }
        public double HungryReward { get; set; }
        public uint NumFoodResources { get; set; }
        public bool SeeFoodFromAfar { get; set; }
        public bool SoloHunting { get; set; }

        #endregion
    }
}