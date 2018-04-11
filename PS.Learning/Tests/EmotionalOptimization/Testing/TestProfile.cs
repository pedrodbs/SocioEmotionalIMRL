using Learning.IMRL.Testing;
using Learning.Testing;

namespace Learning.Tests.EmotionalOptimization.Testing
{
    public class TestProfile
    {
        public string TestMeasuresFilePath { get; set; }
        public string EnvironmentConfigFile { get; set; }
        public string FilePath { get; set; }
        public TestFactory TestFactory { get; set; }
        public ArrayParameter SpecialTestParameters { get; set; }
        public uint MaxStates { get; set; }
    }
}