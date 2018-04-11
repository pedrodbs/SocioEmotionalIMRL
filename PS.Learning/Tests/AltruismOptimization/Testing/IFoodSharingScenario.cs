// ------------------------------------------
// IFoodSharingTestProfile.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/19
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using PS.Learning.Social.Testing;

namespace PS.Learning.Tests.AltruismOptimization.Testing
{
    public interface IFoodSharingScenario : ISocialScenario
    {
        uint MaxStepsWithoutEating { get; set; }
        double HungryReward { get; set; }
        uint NumFoodResources { get; set; }
        bool SeeFoodFromAfar { get; set; }
        bool SoloHunting { get; set; }
    }
}