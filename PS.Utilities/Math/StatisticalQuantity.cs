// ------------------------------------------
// <copyright file="StatisticalQuantity.cs" company="Pedro Sequeira">
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
//    Project: PS.Utilities.Math

//    Last updated: 11/04/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using Newtonsoft.Json;
using PS.Utilities.Core;
using PS.Utilities.IO.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PS.Utilities.Math
{
    public delegate void SampleStatisticHandler(
        StatisticalQuantity quantity, ulong sampleIdx, SampleStatistic sampleStatistic);

    [Serializable]
    public sealed class StatisticalQuantity : IDisposable
    {
        #region Private Fields

        private const double NUM_LABELS = 10f;

        private double _lastValue;

        private SampleStatistic _sampleStatistic = new SampleStatistic();

        private uint _stepSampleCount;

        #endregion Private Fields

        #region Public Constructors

        public StatisticalQuantity(double value, uint maxNumSamples, Range<double> range)
        {
            this.InitDefaults();
            this._lastValue = value;
            this.Range = range;
            this.MaxNumSamples = maxNumSamples;
        }

        public StatisticalQuantity(double value, Range<double> range)
            : this(value, 0, range)
        {
        }

        public StatisticalQuantity(uint maxNumSamples)
            : this(0, maxNumSamples, Core.Range.FullDoubleRange)
        {
        }

        public StatisticalQuantity()
            : this(0, 0, Core.Range.FullDoubleRange)
        {
        }

        #endregion Public Constructors

        #region Public Events

        public event SampleStatisticHandler NewSample;

        #endregion Public Events

        #region Public Properties

        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public double Max { get; private set; }

        public uint MaxNumSamples
        {
            get { return this.Samples == null ? 0 : (uint)this.Samples.Length; }
            set
            {
                if (value <= 0) return;
                this.Samples = new SampleStatistic[value];
                for (var i = 0; i < value; i++)
                    this.Samples[i] = new SampleStatistic();
            }
        }

        [JsonProperty]
        public double Mean => this._sampleStatistic.Mean;

        [JsonProperty]
        public double Min { get; private set; }

        [JsonProperty]
        public Range<double> Range { get; private set; }

        [JsonProperty]
        public uint SampleCount { get; private set; }

        [JsonProperty]
        public SampleStatistic[] Samples { get; private set; }

        public uint SampleSteps { get; set; }

        [JsonProperty]
        public double StdDev => this._sampleStatistic.StandardDeviation;

        [JsonProperty]
        public double Sum => this._sampleStatistic.Sum;

        [JsonProperty]
        public double TotalCount => this._sampleStatistic.TotalCount;

        public double Value
        {
            get { return this._lastValue; }
            set { this.AddValue(value); }
        }

        [JsonProperty]
        public ulong ValueCount { get; private set; }

        [JsonProperty]
        public double Variance => this._sampleStatistic.Variance;

        #endregion Public Properties

        #region Public Methods

        public static void Copy(StatisticalQuantity source, StatisticalQuantity dest)
        {
            //checks for null entries
            if ((source == null) || (dest == null)) return;
            source.CopyTo(dest);
        }

        public static StatisticalQuantity GetQuantitiesAverage(ICollection<StatisticalQuantity> quantities)
        {
            //checks basic cases
            var quantityCount = quantities.Count;
            if (quantityCount == 0) return new StatisticalQuantity(0);
            if (quantityCount == 1) return quantities.First();

            //averages all quantities
            var quantList = quantities.ToList();
            var avgQuantity = quantList[0];
            for (var i = 1; i < quantList.Count; i++)
                avgQuantity.AverageWith(quantList[i]);

            return avgQuantity;
        }

        public static StatisticalQuantity LoadFromJson(string filePath)
        {
            return JsonUtil.DeserializeJsonFile<StatisticalQuantity>(filePath);
        }

        public void AddRange(IEnumerable<double> values)
        {
            //adds all values of the list to the quantity
            foreach (var newValue in values)
                this.Value = newValue;
        }

        public void AddValue(double value, double variance = 0, ulong count = 1)
        {
            this._lastValue = value; //this.CapValue(avgValue);

            //checks sampling
            if ((this.SampleSteps > 0) && (this.MaxNumSamples > 0))
            {
                //checks step limit, i.e. a need for a new sample
                if ((this.SampleCount < this.MaxNumSamples) && (++this._stepSampleCount >= this.SampleSteps))
                    this.CreateSample(value, variance, count);

                //raises new sample event
                this.NewSample?.Invoke(this, this.ValueCount, this.Samples[this.SampleCount]);
            }

            //updates statistical values
            this.UpdateQuantityStats(value, variance, count);
        }

        public void AverageWith(StatisticalQuantity quantity)
        {
            //updates stats based on other quantity's stats
            this._sampleStatistic.Add(quantity._sampleStatistic);
            this.Max = System.Math.Max(this.Max, quantity.Max);
            this.Min = System.Math.Min(this.Min, quantity.Min);

            //determines min sample count from quantities values
            var numSamples = System.Math.Min(this.SampleCount, quantity.SampleCount);

            //updates each sample
            for (var i = 0; i < numSamples; i++)
                this.Samples[i].Add(quantity.Samples[i]);

            //average (last) value
            if (numSamples > 0)
                this._lastValue = this.Samples[numSamples - 1].Mean;
        }

        public void AverageWith(IEnumerable<StatisticalQuantity> quantities)
        {
            foreach (var quantity in quantities)
                this.AverageWith(quantity);
        }

        public StatisticalQuantity Clone()
        {
            var quantity = new StatisticalQuantity(this.MaxNumSamples);
            this.CopyTo(quantity);
            return quantity;
        }

        public void CopyTo(StatisticalQuantity dest)
        {
            //checks for null entries
            if (dest == null) return;

            //sets capacity
            dest.MaxNumSamples = this.MaxNumSamples;

            //copies samples
            this.Samples?.CopyTo(dest.Samples, 0);

            //copies all other statistical fields
            dest.Range = this.Range;
            //dest.QuantityCount = this.QuantityCount;
            dest.ValueCount = this.ValueCount;
            dest._lastValue = this._lastValue;
            dest._sampleStatistic = this._sampleStatistic;
            dest._stepSampleCount = this._stepSampleCount;
            dest.Max = this.Max;
            dest.Min = this.Min;
            dest.SampleCount = this.SampleCount;
            dest.Id = this.Id;
        }

        public void Dispose()
        {
            this.Samples = null;
        }

        public void PrintStatisticsToCSV(
            string filePath, bool printValuesOnly = false)
        {
            if (filePath == null) return;

            //checks directory and file
            var directoryName = Path.GetDirectoryName(filePath);
            if ((directoryName != null) && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            else if ((directoryName != null) && File.Exists(filePath))
                File.Delete(filePath);

            if (!printValuesOnly)
            {
                var statsFilePath = Path.Combine(Path.GetDirectoryName(filePath),
                    $"{Path.GetFileNameWithoutExtension(filePath)}-stats{Path.GetExtension(filePath)}");
                using (var sw = new StreamWriter(statsFilePath))
                {
                    this.PrintQuantityStatistics(sw);
                    sw.Close();
                }
            }

            //prints all quantity statistics and values to file
            using (var sw = new StreamWriter(filePath))
            { 
                this.PrintQuantityValues(sw);
                sw.Close();
            }
        }

        public void Reset()
        {
            this.SampleCount = 0;
            this.ValueCount = 0;
        }

        public void SaveToJson(string filePath)
        {
            this.SerializeJsonFile(filePath);
        }

        #endregion Public Methods

        #region Private Methods

        private double CapValue(double value)
        {
            return value > this.Range.max
                ? this.Range.max
                : (value < this.Range.min ? this.Range.min : value);
        }

        private void CreateSample(double value, double variance, ulong count)
        {
            //updates the correspondent sample
            this.Samples[this.SampleCount].Add(value, variance, count);

            this.SampleCount++;
            this._stepSampleCount = 0;
        }

        private void InitDefaults()
        {
            this._sampleStatistic.Reset();
            this._lastValue = 0;
            this._stepSampleCount = 0;
            this.MaxNumSamples = 0;
            this.SampleSteps = 1;
            this.Max = double.MinValue;
            this.Min = double.MaxValue;
        }

        private void PrintQuantityStatistics(StreamWriter sw)
        {
            //prints quantity statistics
            sw.WriteLine("\"ID\",{0}", this.Id);
            sw.WriteLine("\"Range\",{0},{1}", this.Range.min, this.Range.max);
            sw.WriteLine("\"Value Count\",{0}", this.ValueCount);
            sw.WriteLine("\"Min\",{0}", this.Min);
            sw.WriteLine("\"Max\",{0}", this.Max);
            sw.WriteLine("\"Mean\",{0}", this.Mean);
            sw.WriteLine("\"Variance\",{0}", this.Variance);
            sw.WriteLine("\"Std Dev\",{0}", this.StdDev);
            sw.WriteLine("\"Std Error\",{0}", this._sampleStatistic.StandardError);
            sw.WriteLine("\"Mean + Std Dev\",{0}", (this.Mean + this.StdDev));
            sw.WriteLine("\"Mean - Std Dev\",{0}", (this.Mean - this.StdDev));
            sw.WriteLine("\"Mean + 2*Std Dev\",{0}", this.Mean + (this.StdDev * 2));
            sw.WriteLine("\"Mean - 2*Std Dev\",{0}", this.Mean - (this.StdDev * 2));
        }

        private void PrintQuantityValues(StreamWriter sw)
        {
            if (this.Samples == null) return;

            //prints header
            sw.WriteLine("Time,Value,StdDev,StdErr,Cumulative");
            var labelFactor = this.Samples == null ? 1 : this.SampleCount / NUM_LABELS;

            //prints samples info
            var cumValue = 0d;
            for (var i = 0; i < this.SampleCount; i++)
            {
                var sample = this.Samples[i];
                cumValue += sample.Mean;

                sw.WriteLine("{0},{1},{2},{3},{4}",
                    i / labelFactor,
                    sample.Mean.ToString(CultureInfo.InvariantCulture),
                    sample.StandardDeviation.ToString(CultureInfo.InvariantCulture),
                    sample.StandardError.ToString(CultureInfo.InvariantCulture),
                    cumValue.ToString(CultureInfo.InvariantCulture));
            }
        }

        private void UpdateQuantityStats(double value, double variance, ulong count)
        {
            //update all counters and statistics
            this.ValueCount++;
            this.Min = System.Math.Min(this.Min, this._lastValue);
            this.Max = System.Math.Max(this.Max, this._lastValue);
            this._sampleStatistic.Add(value, variance, count);
        }

        #endregion Private Methods
    }
}