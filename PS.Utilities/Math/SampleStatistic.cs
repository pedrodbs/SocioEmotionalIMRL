// ------------------------------------------
// <copyright file="SampleStatistic.cs" company="Pedro Sequeira">
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

//    Last updated: 07/16/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Utilities.Core;
using System.Collections.Generic;

namespace PS.Utilities.Math
{
    /// <summary>
    ///     A class to store and update the mean and variance of some sample.
    ///     It implements Welford's algorithm for the online update of mean and variance of a sequence of numbers.
    ///     Welford (1962). "Note on a method for calculating corrected sums of squares and products".
    ///     <seealso cref="http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Online_algorithm" />
    ///     The class supports weighted elements to allow the calculation of averages of averages.
    ///     <seealso cref="http://en.wikipedia.org/wiki/Weighted_arithmetic_mean" />
    /// </summary>
    public class SampleStatistic : IResetable
    {
        #region Private Fields

        private double _sumVarMeanSq;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     Creates a new sample statistic object initialized with the given values.
        /// </summary>
        /// <param name="mean">The sample mean value.</param>
        /// <param name="variance"> The variance of the sample.</param>
        /// <param name="weight"> The weight associated with the sample. </param>
        public SampleStatistic(double mean, double variance = 0, ulong weight = 1)
            : this()
        {
            this.Add(mean, variance, weight);
        }

        /// <summary>
        ///     Creates a new sample statistic object with no elements.
        /// </summary>
        public SampleStatistic()
        {
            this.Reset();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     Gets the sample mean value.
        /// </summary>
        public double Mean { get; private set; }

        /// <summary>
        ///     Gets the (estimated) sample standard deviation.
        /// </summary>
        public double StandardDeviation
        {
            get { return System.Math.Sqrt(this.Variance); }
        }

        /// <summary>
        ///     Gets the (estimated) standard error of the mean.
        /// </summary>
        public double StandardError
        {
            get { return this.TotalCount.Equals(0) ? 0 : this.StandardDeviation / System.Math.Sqrt(this.TotalCount); }
        }

        /// <summary>
        ///     Gets the sample sum.
        /// </summary>
        public double Sum
        {
            get { return this.Mean * this.TotalCount; }
        }

        /// <summary>
        ///     Gets the size of the sample, i.e., total number of sample elements.
        /// </summary>
        public ulong TotalCount { get; private set; }

        /// <summary>
        ///     Gets the (estimated) sample variance.
        /// </summary>
        public double Variance
        {
            get { return this.TotalCount.Equals(0) ? 0 : this._sumVarMeanSq - (this.Mean * this.Mean); }
        }

        #endregion Public Properties

        #region Public Methods

        public static SampleStatistic FromValues(IEnumerable<double> values)
        {
            var sampleStatistic = new SampleStatistic();
            sampleStatistic.AddRange(values);
            return sampleStatistic;
        }

        public static SampleStatistic FromValues(IEnumerable<SampleStatistic> values)
        {
            var sampleStatistic = new SampleStatistic();
            sampleStatistic.AddRange(values);
            return sampleStatistic;
        }

        /// <summary>
        ///     Adds an element to the sequence and updates mean and variance.
        /// </summary>
        /// <param name="x">The new sample element. Can be a mean value.</param>
        /// <param name="varX">
        ///     The variance of the new sample element. 0 means inserting a single element or that <see cref="x" /> represents a
        ///     sub-sample of equal elements.
        /// </param>
        /// <param name="count">
        ///     The weight associated with the element, i.e. the size of the sub-sample. 1 means inserting a single element.
        /// </param>
        public void Add(double x, double varX = 0d, ulong count = 1)
        {
            this._sumVarMeanSq = (this._sumVarMeanSq * this.TotalCount) + (count * varX) + (count * x * x);
            this.Mean = (this.Mean * this.TotalCount) + (count * x);

            this.TotalCount += count;
            this._sumVarMeanSq /= this.TotalCount;
            this.Mean /= this.TotalCount;
        }

        public void Add(SampleStatistic stat)
        {
            this.Add(stat.Mean, stat.Variance, stat.TotalCount);
        }

        public void AddRange(IEnumerable<double> values)
        {
            foreach (var value in values)
                this.Add(value);
        }

        public void AddRange(IEnumerable<SampleStatistic> values)
        {
            foreach (var value in values)
                this.Add(value);
        }

        /// <summary>
        ///     Resets the statistics of the sample sequence, i.e., count = mean = variance = 0
        /// </summary>
        public void Reset()
        {
            this.Mean = this._sumVarMeanSq = this.TotalCount = 0;
        }

        #endregion Public Methods
    }
}