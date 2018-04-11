// ------------------------------------------
// <copyright file="DataAnalyzer.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL

//    Last updated: 01/27/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PS.Learning.IMRL.Testing.Config;
using PS.Learning.Core.Testing;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Utilities.Collections;
using PS.Utilities.IO;
using PS.Utilities.Math;

namespace PS.Learning.IMRL
{
    public class DataAnalyzer
    {
        public DataAnalyzer(IMRLTestsConfig testsConfig)
        {
            this.TestsConfig = testsConfig;
        }

        public IMRLTestsConfig TestsConfig { get; private set; }

        public bool CreateTestAnalysis(
            IList<uint> testTypes, int xVarIdx, int seriesVarIdx, Dictionary<int, double> fixedParams)
        {
            //analysis test measures for each test type
            var analysisSuccess = true;
            foreach (var testType in testTypes)
            {
                //tries to get test measures for test profile
                var testProfile = this.TestsConfig.ScenarioProfiles[testType];
                var testMeasures = this.GetTestMeasures(testProfile);
                if (testMeasures == null)
                {
                    analysisSuccess = false;
                    continue;
                }

                //gets file header and path
                var header = this.GetAnalysisFileHeader(xVarIdx, seriesVarIdx, fixedParams);
                var filePath = this.GetAnalysisFilePath(testProfile, xVarIdx, seriesVarIdx, fixedParams);

                //gets analysis data
                var data = this.GetAnalysisData(testMeasures, xVarIdx, seriesVarIdx, fixedParams);

                //prints analisys data to csv file
                this.PrintAnalysisFile(filePath, header, data);
            }

            return analysisSuccess;
        }

        protected List<double[]> GetAnalysisData(
            TestMeasureList testMeasures, int xVarIdx, int seriesVarIdx, Dictionary<int, double> fixedParams)
        {
            Console.WriteLine("\n__________________________________________");
            Console.WriteLine("Calculating data...");

            //gets all possible values for the x- and series-variable
            var xValues = this.GetParameterValues(xVarIdx);
            var seriesValues = this.GetParameterValues(seriesVarIdx);

            //creates structure to hold data
            var numVars = (2*seriesValues.Length) + 1;
            var data = new List<double[]>();

            foreach (var x in xValues)
            {
                //creates data point for each possible value of the x-var
                var dataPoint = new double[numVars];
                dataPoint[0] = x;

                //goes to all series to fill the data point with the corresponding fitness values
                for (var sIdx = 1; sIdx < numVars; sIdx += 2)
                {
                    //gets all possible array params according to requested values
                    var s = seriesValues[sIdx/2];
                    var arrayParameters = this.GetPossibleArrayParams(xVarIdx, x, seriesVarIdx, s, fixedParams);

                    //gets all associated test measures
                    var measures = this.GetTestMeasures(arrayParameters, testMeasures);
                    if (testMeasures.Count == 0)
                    {
                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                            "No test measure found for {0}={1:0.00}, {2}=x={3:0.0} !!",
                            this.GetVarLetter(xVarIdx), x, this.GetVarLetter(sIdx), s));
                        continue;
                    }

                    //selects fitness for series
                    var fitness = this.GetFitnessValue(measures);
                    var fitnessErr = this.GetFitnessError(measures);

                    //adds fitness to data point
                    dataPoint[sIdx] = fitness;
                    dataPoint[sIdx + 1] = fitnessErr;
                }

                //adds data point to list
                data.Add(dataPoint);
            }

            return data;
        }

        protected virtual double GetFitnessValue(IEnumerable<TestMeasure> measures)
        {
            //returns max fitness (can be average or other metric)
            return measures.Max(measure => measure.Value);
        }

        protected virtual double GetFitnessError(IEnumerable<TestMeasure> measures)
        {
            //returns avg fitness std dev
            return measures.Average(measure => measure.StdDev);
        }

        protected IEnumerable<TestMeasure> GetTestMeasures(
            IEnumerable<ArrayParameter> arrayParameters, TestMeasureList measures)
        {
            //tries to get all measures associated with the requested parameters
            return (from arrayParameter in arrayParameters
                where measures.Contains(arrayParameter)
                select measures[arrayParameter]);
        }

        protected IEnumerable<ArrayParameter> GetPossibleArrayParams(
            int xParamIdx, double x, int seriesParamIdx, double s, Dictionary<int, double> fixedParams)
        {
            //creates possible values for all params
            var numParams = this.TestsConfig.NumParams;
            var paramIntervals = new double[numParams][];
            for (var i = 0; i < numParams; i++)
                if (i.Equals(xParamIdx))
                    //x param is fixed
                    paramIntervals[i] = new[] {x};
                else if (i.Equals(seriesParamIdx))
                    //series param is fixed
                    paramIntervals[i] = new[] {s};
                else if (fixedParams.ContainsKey(i))
                    //other fixed params
                    paramIntervals[i] = new[] {fixedParams[i]};
                else
                //free params (can be of any value)
                    paramIntervals[i] = this.GetParameterValues(i);

            //gets all possible param combinations
            return paramIntervals.AllCombinations().Select(paramComb => new ArrayParameter(paramComb));
        }

        protected void PrintAnalysisFile(string filePath, string header, List<double[]> data)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            var sw = new StreamWriter(filePath);

            Console.WriteLine("\n__________________________________________");
            Console.WriteLine("Writing data analysis to file: {0}", filePath);

            //prints header
            sw.WriteLine(header);

            //writes all data points to file
            foreach (var point in data)
                sw.WriteLine(point.Aggregate(
                    string.Empty,
                    (current, pData) => current + string.Format(CultureInfo.InvariantCulture, "{0};", pData)));

            sw.Close();
            sw.Dispose();
        }

        protected string GetVarName(int varIdx)
        {
            return this.TestsConfig.ParamIDNames[varIdx];
        }

        protected char GetVarLetter(int varIdx)
        {
            return this.TestsConfig.ParamIDLetters[varIdx];
        }

        protected double[] GetParameterValues(int varIdx)
        {
            return NumericArrayUtil<double>.NumericArrayFromInterval(this.TestsConfig.ParamsStepIntervals[varIdx]);
        }

        protected string GetAnalysisFilePath(
            IScenario scenario, int xVarIdx, int seriesVarIdx, Dictionary<int, double> fixedParams)
        {
            //gets and clears base file path
            var baseFilePath =
                Path.GetFullPath(string.Format("{0}{1}analysis", scenario.FilePath, Path.DirectorySeparatorChar));
            PathUtil.CreateDirectory(baseFilePath);

            //adds dependent elements
            var fileName = string.Format(
                "{0}x{1}_", this.GetVarName(xVarIdx), this.GetVarName(seriesVarIdx));

            //adds fixed elements
            fileName = fixedParams.Aggregate(
                fileName, (current, fixedParam) =>
                    string.Format("{0}{1}={2:0.0}_", current, this.GetVarName(fixedParam.Key), fixedParam.Value));
            fileName = fileName.Remove(fileName.Length - 1);

            //combines with base path
            var measuresFile = string.Format("{0}{1}{2}.csv", baseFilePath, Path.DirectorySeparatorChar, fileName);
            return measuresFile;
        }

        protected string GetAnalysisFileHeader(int xVarIdx, int seriesVarIdx, Dictionary<int, double> fixedParams)
        {
            //gets all possible series values
            var seriesValues = this.GetParameterValues(seriesVarIdx);

            //gets x variable name
            var header = string.Format("{0};", this.GetVarName(xVarIdx));

            ////gets fixed values component, eg fixed1=0.5 fixed2=0.9
            //var fixedValuesStr = fixedParams.Aggregate(
            //    string.Empty, (current, fixedParam) =>
            //                  string.Format("{0}{1}={2:0.0} ", current,
            //                                this.GetVarLetter(fixedParam.Key), fixedParam.Value));
            //fixedValuesStr.Remove(fixedValuesStr.Length - 1);

            // gets series names according to series name and fixed values, eg series=0.1 fixed1=0.5 fixed2=0.9;
            header = seriesValues.Aggregate(
                header, (current, seriesValue) =>
                    current +
                    string.Format("{0}={1:0.0};{0}={1:0.0} err;", this.GetVarLetter(seriesVarIdx), seriesValue));
            return header;
        }

        protected TestMeasureList GetTestMeasures(IScenario scenario)
        {
            //tests for measures file
            if (!File.Exists(scenario.TestMeasuresFilePath))
                return null;

            Console.WriteLine("\n__________________________________________");
            Console.WriteLine("Reading test measures from file: {0}...", scenario.TestMeasuresFilePath);

            //creates list and read measures from file
            var testFactory = this.TestsConfig.CreateTestFactory(scenario);
            var testMeasures = testFactory.CreateTestMeasureList();
            testMeasures.ReadFromFile(scenario.TestMeasuresFilePath);

            Console.WriteLine("{0} test measures found.", testMeasures.Count);

            return testMeasures;
        }
    }
}