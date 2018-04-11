using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Learning.Utils;

namespace Learning.Tests.EmotionalOptimization.Testing
{
    public delegate void Simulationner(TestType testType, TestsConfig testsConfig);

    public class OptimizationSimulationner
    {
        protected readonly Simulationner testRunner;
        protected readonly List<TestType> testTypeList;
        protected readonly TestsConfig testsConfig;
        protected int numErrors;

        public OptimizationSimulationner(TestsConfig testsConfig, List<TestType> testTypeList, Simulationner testRunner)
        {
            this.testsConfig = testsConfig;
            this.testTypeList = testTypeList;
            this.testRunner = testRunner;

            //inits tests configs
            this.testsConfig.Init();

            //sets maximum cpu affinity
            ProcessUtil.SetMaximumProcessAffinity();
            //ProcessUtil.SetProcessAffinity(4);
        }

        public void Run()
        {
            //runs all tests while there are tests to run
            while (testTypeList.Any(testType => !testsConfig.CompletedTests.Contains(testType)))
            {
                try
                {
                    //runs all tests from list
                    foreach (var testType in this.testTypeList)
                        this.RunTest(testType);

                    //saves list of completed tests
                    this.testsConfig.WriteCompletedTests();
                }
                
#if !DEBUG
                catch (Exception exception)
                {

                    //if in release mode, write error to file and continue to run tests
                    this.WriteErrorFile(exception);
                    Thread.Sleep(this.numErrors++*1000);
                    continue;
                }
#endif
                finally
                {
                }
            }
        }

        protected void WriteErrorFile(Exception exception)
        {
            try
            {
                var errorDir = Path.GetFullPath("./errors");
                if (!Directory.Exists(errorDir)) Directory.CreateDirectory(errorDir);
                var sw = new StreamWriter(string.Format("{0}/Error{1}.log", errorDir, DateTime.Now.Ticks));
                sw.Write(exception.Message);
                sw.Close();
            }
            catch
            {
            }
        }

        protected void RunTest(TestType testType)
        {
            //checks if test was already completed
            if (this.testsConfig.CompletedTests.Contains(testType)) return;

            //runs test
            this.testRunner(testType, this.testsConfig);

            //moves results to dest dir and marks test as completed
            Console.WriteLine(@"Moving results...");
            this.testsConfig.CompletedTests.Add(testType);
            this.testsConfig.WriteCompletedTests();
            //this.testsConfig.MoveTestResults(testType);
        }
    }
}