using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Learning.Tests.EmotionalOptimization.Testing.Factories;
using Learning.Utils;

namespace Learning.Tests.EmotionalOptimization.Testing
{

    #region TestType enum

    public enum TestType
    {
        Exploration,
        Persistence,
        DifferentPreySeason,
        PoisonedSeason,
        HungerThirst,
        Lairs,
        Pacman,
        PowerPellets,
        EatAllDots,
        RewardingDots,
    }

    #endregion

    public class TestsConfig
    {
        protected readonly uint maxMoveActionsRequired;
        protected readonly uint maxSteps;
        protected readonly uint numStepsPerSeason;
        protected readonly bool randStart;
        protected readonly uint sampleSteps;

        public TestsConfig(
            uint sampleSteps, uint numStepsPerSeason, uint maxMoveActionsRequired, uint maxSteps, bool randStart)
        {
            this.sampleSteps = sampleSteps;
            this.numStepsPerSeason = numStepsPerSeason;
            this.maxMoveActionsRequired = maxMoveActionsRequired;
            this.maxSteps = maxSteps;
            this.randStart = randStart;

            this.SetDefaultConstants();
        }

        public string IREnvironmentFile { get; set; }
        public string CornersEnvironmentFile { get; set; }
        public string PacmanEnvironmentFile { get; set; }
        public string PhaseID { get; set; }
        public string TestMeasuresName { get; set; }
        public string TestIDPrefix { get; set; }
        public HashSet<TestType> CompletedTests { get; protected set; }
        public Dictionary<TestType, TestProfile> ConfigList { get; protected set; }
        public Dictionary<TestType, string> TestIDs { get; protected set; }
        public string CompletedTestsFilePath { get; set; }
        public string DestinationTestsDir { get; set; }

        public void Init()
        {
            this.ReadCompletedTests();
            this.CreateTestsConfigs();
        }

        protected virtual void SetDefaultConstants()
        {
            this.CompletedTestsFilePath = Path.GetFullPath("./CompletedTests.cfg");
            this.DestinationTestsDir =
                //Path.GetFullPath("../../../../../../experiments/TAC 2012/II - Foraging IMRL Genetics");
                Path.GetFullPath("../../../../../../experiments/TAC 2012/II - Pacman IMRL Genetics");
            this.IREnvironmentFile = Path.GetFullPath("../../../../bin/config/ir-environment.xml");
            this.CornersEnvironmentFile = Path.GetFullPath("../../../../bin/config/corners-environment.xml");
            this.PacmanEnvironmentFile = Path.GetFullPath("../../../../bin/config/pacman-environment.xml");
            this.PhaseID = "Top10";
            //this.PhaseID = "Optimization0.1";
            this.TestMeasuresName = "TestMeasures.csv";
            this.TestIDPrefix = "IMGP"; //"IMGP"; //"IM";
        }

        protected void ReadCompletedTests()
        {
            this.CompletedTests = new HashSet<TestType>();

            //check file path
            if (!File.Exists(this.CompletedTestsFilePath)) return;

            //reads completed tests (type) from file
            var sr = new StreamReader(this.CompletedTestsFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                TestType testType;
                if (Enum.TryParse(line, out testType))
                    this.CompletedTests.Add(testType);
            }
            sr.Close();
            sr.Dispose();
        }

        public virtual void WriteCompletedTests()
        {
            //create completed tests file
            var sw = new StreamWriter(this.CompletedTestsFilePath) {AutoFlush = true};
            foreach (var testType in this.CompletedTests)
                sw.WriteLine(testType.ToString());
            sw.Close();
            sw.Dispose();
        }

        public virtual void MoveTestResults(TestType testType)
        {
            var testConfig = this.ConfigList[testType];
            var dirName = Path.GetFileName(testConfig.FilePath);
            var destDirPath = Path.GetFullPath(string.Format("{0}/{1}", this.DestinationTestsDir, dirName));

            //move test results to another location
            if (Directory.Exists(destDirPath))
            {
                PathUtil.ClearDirectory(destDirPath);
                Thread.Sleep(500);
                Directory.Delete(destDirPath);
                Thread.Sleep(500);
            }
            Directory.Move(testConfig.FilePath, destDirPath);
        }

        protected virtual void CreateTestsConfigs()
        {
            this.ConfigList = new Dictionary<TestType, TestProfile>();

            //ext, val, mot, nov, cont
            this.ConfigList.Add(
                TestType.Exploration,
                new TestProfile
                    {
                        FilePath = this.GetFilePath(TestType.Exploration),
                        EnvironmentConfigFile = this.IREnvironmentFile,
                        TestFactory = new ExplorationTestFactory(this.sampleSteps),
                        //SpecialTestParameters = new ArrayParameter(new float[] {0, 0, 0, 1, 0}),
                        TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.Exploration),
                        MaxStates = 14
                    });
            this.ConfigList.Add(
                TestType.DifferentPreySeason,
                new TestProfile
                    {
                        FilePath = this.GetFilePath(TestType.DifferentPreySeason),
                        EnvironmentConfigFile = this.IREnvironmentFile,
                        TestFactory =
                            new PreySeasonTestFactory(
                            this.sampleSteps, this.numStepsPerSeason, this.randStart),
                        //SpecialTestParameters = new ArrayParameter(new float[] {0, 0, 0, 0, 1}),
                        TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.DifferentPreySeason),
                        MaxStates = 13
                    });
            this.ConfigList.Add(
                TestType.Persistence,
                new TestProfile
                    {
                        FilePath = this.GetFilePath(TestType.Persistence),
                        EnvironmentConfigFile = this.IREnvironmentFile,
                        TestFactory = new PersistanceTestFactory(
                            this.sampleSteps, this.maxMoveActionsRequired, this.maxSteps),
                        //SpecialTestParameters = new ArrayParameter(new float[] {0, 0, 1, 0, 0}),
                        TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.Persistence),
                        MaxStates = 11
                    });
            this.ConfigList.Add(
                TestType.PoisonedSeason,
                new TestProfile
                    {
                        FilePath = this.GetFilePath(TestType.PoisonedSeason),
                        EnvironmentConfigFile = this.IREnvironmentFile,
                        TestFactory = new PoisonedSeasonTestFactory(this.sampleSteps),
                        //SpecialTestParameters = new ArrayParameter(new float[] {0, 0, 0, 0, 1}),
                        TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.PoisonedSeason),
                        MaxStates = 13
                    });
            this.ConfigList.Add(
                TestType.HungerThirst,
                new TestProfile
                    {
                        FilePath = this.GetFilePath(TestType.HungerThirst),
                        EnvironmentConfigFile = this.CornersEnvironmentFile,
                        TestFactory = new HungerThirstTestFactory(this.sampleSteps, this.randStart),
                        //SpecialTestParameters = new ArrayParameter(new float[] { 0, 0, 0, 0, 1 }),
                        TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.HungerThirst),
                        MaxStates = 50
                    });
            this.ConfigList.Add(
                TestType.Lairs,
                new TestProfile
                    {
                        FilePath = this.GetFilePath(TestType.Lairs),
                        EnvironmentConfigFile = this.CornersEnvironmentFile,
                        TestFactory = new LairsTestFactory(this.sampleSteps, this.randStart),
                        //SpecialTestParameters = new ArrayParameter(new float[] { 0, 0, 0, 0, 1 }),
                        TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.Lairs),
                        MaxStates = 80
                    });
            this.ConfigList.Add(
                TestType.Pacman,
                new TestProfile
                    {
                        FilePath = this.GetFilePath(TestType.Pacman),
                        EnvironmentConfigFile = this.PacmanEnvironmentFile,
                        TestFactory = new PacmanTestFactory(this.sampleSteps, false, false, true, true, 3, 0, 0, -0.1f),
                        TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.Pacman),
                        MaxStates = 1200
                    });
            this.ConfigList.Add(
                TestType.EatAllDots,
                new TestProfile
                {
                    FilePath = this.GetFilePath(TestType.EatAllDots),
                    EnvironmentConfigFile = this.PacmanEnvironmentFile,
                    TestFactory = new PacmanTestFactory(this.sampleSteps, false, false, true, false, 3, 0.5f, 0, -0.5f),
                    TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.EatAllDots),
                    MaxStates = 1000
                });
            this.ConfigList.Add(
                TestType.PowerPellets,
                new TestProfile
                    {
                        FilePath = this.GetFilePath(TestType.PowerPellets),
                        EnvironmentConfigFile = this.PacmanEnvironmentFile,
                        TestFactory = new PacmanTestFactory(this.sampleSteps, true, false, false, true, 0, 0.8f, 0, -1),
                        TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.PowerPellets),
                        MaxStates = 200
                    });
            this.ConfigList.Add(
                TestType.RewardingDots,
                new TestProfile
                    {
                        FilePath = this.GetFilePath(TestType.RewardingDots),
                        EnvironmentConfigFile = this.PacmanEnvironmentFile,
                        TestFactory =
                            new PacmanTestFactory(this.sampleSteps, false, false, false, false, 0, 0.8f, 0.1f, -1),
                        TestMeasuresFilePath = this.GetTestMeasuresFilePath(TestType.RewardingDots),
                        MaxStates = 1000
                    });
        }

        protected string GetFilePath(TestType testType)
        {
            return Path.GetFullPath(string.Format("./{0}", this.GetTestID(testType)));
        }

        protected string GetTestMeasuresFilePath(TestType testType)
        {
            return Path.GetFullPath(
                string.Format("{0}/{1}/{2}/{3}",
                              this.DestinationTestsDir, this.GetTestID(testType), this.PhaseID, this.TestMeasuresName));
        }

        protected string GetTestID(TestType testType)
        {
            return this.TestIDPrefix + testType;
        }
    }
}