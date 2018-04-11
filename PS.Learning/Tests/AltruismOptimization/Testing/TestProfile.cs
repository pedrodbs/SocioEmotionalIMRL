namespace Learning.Tests.AltruismOptimization.Testing
{
    public struct TestProfile
    {
        public string EnvironmentConfigFile { get; set;}
        public string FilePath { get; set; }
        public uint MaxAgents { get; set; }
        public uint MaxFoodResources { get; set; }
        public uint NumPositions { get; set; }
        public bool StrongerAgent { get; set; } 
    }
}