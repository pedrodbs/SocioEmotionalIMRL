// ------------------------------------------
// <copyright file="CondorScriptBuilder.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.Core

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Testing.Config;
using PS.Utilities.Collections;
using PS.Utilities.IO.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PS.Learning.Core.Testing
{
    public class CondorScriptBuilder
    {
        #region Protected Fields

        protected const string ARGS_LINE_STR = "Arguments = ";
        protected const int DEF_MAX_JOBS_PER_FILE = 25000;
        protected const string EXECUTABLE_LINE_STR = "Executable = ";
        protected const string FILE_TRANSFER_LINE_STR = "should_transfer_files = NO";
        protected const string PERIODIC_RELEASE_LINE_STR = "periodic_release = TRUE";
        protected const string PROC_ID_STR = "proc";
        protected const string QUEUE_LINE_STR = "Queue";
        protected const string UNIVERSE_LINE_STR = "Universe = vanilla";

        #endregion Protected Fields

        #region Public Constructors

        public CondorScriptBuilder(ITestsConfig testsConfig)
        {
            this.MaxJobsPerFile = DEF_MAX_JOBS_PER_FILE;
            this.TestsConfig = testsConfig;
        }

        #endregion Public Constructors

        #region Protected Properties

        protected int MaxJobsPerFile { get; set; }
        protected ITestsConfig TestsConfig { get; }

        #endregion Protected Properties

        #region Public Methods

        public void CreateCondorFile()
        {
            //gets all tests config for this test
            var allTestsConfigs = TestingUtil.GenerateAllTestsConfigs(this.TestsConfig);

            //gets all arguments for each tests config
            var testArgumentsList = allTestsConfigs.Select(testsConfig => ArgumentsParser.GetArgs(testsConfig)).ToList();

            //create as many condor submit files as necessary
            var scriptFilePath = Path.GetFullPath($"{this.TestsConfig.CondorScriptPath}");
            var executableFile = Assembly.GetEntryAssembly().GetName().Name.Split('.').Last();
            var fileNum = 0;
            for (var i = 0; i < testArgumentsList.Count; i += this.MaxJobsPerFile, fileNum++)
            {
                var numTests = ((i + this.MaxJobsPerFile) > testArgumentsList.Count
                    ? testArgumentsList.Count - i
                    : this.MaxJobsPerFile);

                this.CreateCondorFile(scriptFilePath + fileNum, executableFile, testArgumentsList.GetRange(i, numTests));
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void WriteExecutableArguments(StreamWriter sw, List<string[]> testArgumentsList)
        {
            //writes arguments and queue lines for each desired test
            foreach (var testArguments in testArgumentsList)
            {
                var argLine = $"{ARGS_LINE_STR}{testArguments.ToString(' ')}";
                sw.WriteLine(argLine);
                sw.WriteLine(QUEUE_LINE_STR);
            }
        }

        private static void WriteHeader(string executableFile, StreamWriter sw)
        {
            //writes condor script header
            sw.WriteLine(UNIVERSE_LINE_STR);
            sw.WriteLine("{0}{1}", EXECUTABLE_LINE_STR, executableFile);
            //sw.WriteLine("Log = output/{0}$(Process).log", PROC_ID_STR);
            //sw.WriteLine("Output = output/{0}$(Process).out", PROC_ID_STR);
            //sw.WriteLine("Error = output/{0}$(Process).error", PROC_ID_STR);
            sw.WriteLine(FILE_TRANSFER_LINE_STR);
            sw.WriteLine(PERIODIC_RELEASE_LINE_STR);
        }

        private void CreateCondorFile(
                            string scriptFilePath, string executableFile, List<string[]> testArgumentsList)
        {
            if (File.Exists(scriptFilePath)) File.Delete(scriptFilePath);
            var sw = new StreamWriter(scriptFilePath);

            //writes condor script header
            WriteHeader(executableFile, sw);

            //writes arguments for programs execution for all test parameters
            WriteExecutableArguments(sw, testArgumentsList);

            sw.Close();
            sw.Dispose();

            Console.WriteLine("Created condor submission file '{0}' with {1} test parameters.",
                scriptFilePath, testArgumentsList.Count);
        }

        #endregion Private Methods
    }
}