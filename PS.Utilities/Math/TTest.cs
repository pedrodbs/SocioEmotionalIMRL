// ------------------------------------------
// <copyright file="TTest.cs" company="Pedro Sequeira">
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
//    Project: PS.Utilities

//    Last updated: 07/18/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using MathNet.Numerics.Statistics;

namespace PS.Utilities.Math
{
    public struct TTestResult
    {
        public TTestResult(double statistics = 0, double pValue = 1) : this()
        {
            this.TestStatistics = statistics;
            this.PValue = pValue;
        }

        public double TestStatistics { get; set; }
        public double PValue { get; private set; }

        public void UpdateP(StudentT tDistribution)
        {
            this.PValue = 2.0*tDistribution.CumulativeDistribution(-this.TestStatistics);
        }
    }

    public static class TTest
    {
        private static readonly Random Random = new WH2006();
        //Two sided one sample t test
        public static TTestResult CalcTTest(double[] data, double testValue)
        {
            var sampleMean = data.Average();
            var sampleVariance = data.Variance();
            var result = new TTestResult {TestStatistics = T(sampleMean, testValue, sampleVariance, data.Length)};
            result.UpdateP(new StudentT(sampleMean, sampleVariance, data.Length, Random));
            return result;
        }

        public static TTestResult CalcTTest(double[] data1, double[] data2, bool equalVariance)
        {
            var mean1 = data1.Average();
            var mean2 = data2.Average();
            var variance1 = data1.Variance();
            var variance2 = data2.Variance();

            var result = new TTestResult(equalVariance
                ? System.Math.Abs(T2Equal(
                    mean1, mean2, variance1, variance2, data1.Length, data2.Length))
                : System.Math.Abs(T2Unequal(
                    mean1, mean2, variance1, variance2, data1.Length, data2.Length)));

            var dof = equalVariance
                ? data1.Length + data2.Length - 2
                : DOF(variance1, variance2, data1.Length, data2.Length);

            if (dof > 0)
                result.UpdateP(new StudentT(mean1, variance1, dof, Random));
            return result;
        }

        public static TTestResult CalcTTest(
            double data1Mean, double data2Mean, double data1Variance,
            double data2Variance, int data1Length, int data2Length)
        {
            var result = new TTestResult(
                System.Math.Abs(T2Unequal(data1Mean, data2Mean, data1Variance, data2Variance,
                    data1Length, data2Length)));

            var dof = DOF(data1Variance, data2Variance, data1Length, data2Length);
            if (dof > 0)
                result.UpdateP(
                    new StudentT(data1Variance.Equals(0) ? data2Mean : data1Mean,
                        data1Variance.Equals(0) ? data2Variance : data1Variance, dof, Random));
            return result;
        }

        //test statistic for 1-sample t-test
        private static double T(double sampleMean, double constant, double sampleVariance, double length)
        {
            return (sampleMean - constant)/System.Math.Sqrt(sampleVariance/length);
        }

        //test statistic for 2-sample test does not assume equal variance
        private static double T2Unequal(
            double mean1, double mean2, double variance1, double variance2, double length1, double length2)
        {
            return (mean1 - mean2)/System.Math.Sqrt((variance1/length1) + (variance2/length2));
        }

        //test statistic for 2-sample test does assume equal variance
        private static double T2Equal(
            double mean1, double mean2, double variance1, double variance2, double length1, double length2)
        {
            var variance = ((length1 - 1.0)*variance1 + (length2 - 1.0)*variance2)/(length1 + length2 - 2.0);
            return (mean1 - mean2)/System.Math.Sqrt(variance*(1.0/length1 + 1.0/length2));
        }

        //Degrees of freedom to 2 sample unequal variance
        private static double DOF(double variance1, double variance2, double length1, double length2)
        {
            if (length1.Equals(1d) || length2.Equals(1d) || (variance1.Equals(0d) && variance2.Equals(0d)))
                return 0;

            var nom = ((variance1/length1) + (variance2/length2))*((variance1/length1) + (variance2/length2));
            var denom = ((variance1*variance1)/(length1*length1*(length1 - 1.0)) + (variance2*variance2)/
                         (length2*length2*(length2 - 1.0)));
            return nom/denom;
        }
    }
}