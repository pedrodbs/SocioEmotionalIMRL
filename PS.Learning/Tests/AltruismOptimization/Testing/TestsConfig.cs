using System.Collections.Generic;
using System.IO;

namespace Learning.Tests.AltruismOptimization.Testing
{

    #region TestType enum

    public enum TestType
    {
        SingleFood,
        StrongerAgent,
        EqualResource,
        Multi313,
        Multi311,
        Multi323
    }

    #endregion

    public class TestsConfig
    {
        private static TestsConfig _instance;

        private TestsConfig()
        {
            ConfigList = new Dictionary<TestType, TestProfile>();
            ConfigList.Add(
                TestType.SingleFood, new TestProfile
                                         {
                                             FilePath = Path.GetFullPath("./SingleFood"),
                                             EnvironmentConfigFile = "./config/co-op-environment.xml",
                                             MaxAgents = 2,
                                             MaxFoodResources = 1,
                                             StrongerAgent = false
                                         });
            ConfigList.Add(
                TestType.StrongerAgent, new TestProfile
                                            {
                                                FilePath = Path.GetFullPath("./StrongerAgent"),
                                                EnvironmentConfigFile =
                                                    "./config/stronger-co-op-environment.xml",
                                                MaxAgents = 2,
                                                MaxFoodResources = 1,
                                                StrongerAgent = true
                                            });
            ConfigList.Add(
                TestType.EqualResource, new TestProfile
                                            {
                                                FilePath = Path.GetFullPath("./EqualResource"),
                                                EnvironmentConfigFile =
                                                    "./config/no-co-op-environment.xml",
                                                MaxAgents = 2,
                                                MaxFoodResources = 2,
                                                StrongerAgent = false
                                            });
            ConfigList.Add(
                TestType.Multi313, new TestProfile
                                       {
                                           FilePath = Path.GetFullPath("3Agents-1Food-3Positions"),
                                           EnvironmentConfigFile =
                                               "./config/1food-3pos-environment.xml",
                                           MaxAgents = 3,
                                           MaxFoodResources = 1,
                                           NumPositions = 3,
                                           StrongerAgent = false
                                       });
            ConfigList.Add(
                TestType.Multi311, new TestProfile
                                       {
                                           FilePath = Path.GetFullPath("3Agents-1Food-1Positions"),
                                           EnvironmentConfigFile =
                                               "./config/1food-1pos-environment.xml",
                                           MaxAgents = 3,
                                           MaxFoodResources = 1,
                                           NumPositions = 1,
                                           StrongerAgent = false
                                       });
            ConfigList.Add(
                TestType.Multi323, new TestProfile
                                       {
                                           FilePath = Path.GetFullPath("3Agents-2Food-3Positions"),
                                           EnvironmentConfigFile =
                                               "./config/2food-3pos-environment.xml",
                                           MaxAgents = 3,
                                           MaxFoodResources = 2,
                                           NumPositions = 3,
                                           StrongerAgent = false
                                       });
        }

        public Dictionary<TestType, TestProfile> ConfigList { get; private set; }

        public static TestsConfig Instance
        {
            get { return _instance ?? (_instance = new TestsConfig()); }
        }
    }
}