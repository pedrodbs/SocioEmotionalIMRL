// ------------------------------------------
// <copyright file="StatisticsCollection.cs" company="Pedro Sequeira">
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
using PS.Utilities.IO.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PS.Utilities.Math
{
    public class StatisticsCollection : Dictionary<string, StatisticalQuantity>, IDisposable
    {
        #region Public Fields

        public const string MAX_PARAM_NAME = "max";
        public const char SEPARATION_CHAR = ',';
        public const string SUM_PARAM_NAME = "sum";

        #endregion Public Fields

        #region Private Fields

        private const double NUM_LABELS = 10f;

        #endregion Private Fields

        #region Public Properties

        public uint MaxNumSamples { get; set; }
        public string Prefix { get; set; }
        public uint SampleSteps { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static StatisticsCollection LoadFromJson(string filePath)
        {
            return JsonUtil.DeserializeJsonFile<StatisticsCollection>(filePath, JsonUtil.TypeSpecifySettings, true);
        }

        public void Add(StatisticalQuantity quantity)
        {
            if (this.ContainsKey(quantity.Id))
                this[quantity.Id] = quantity;
            else
                base.Add(quantity.Id, quantity);
        }

        public void AddRange(StatisticsCollection statisticsCollection, string prefix = "")
        {
            //adds all statistics from the collection to this collection
            foreach (var quantity in statisticsCollection)
                this.Add($"{prefix}{quantity.Key}", quantity.Value);
        }

        public void AddRange(IDictionary<string, StatisticalQuantity> statisticsCollection)
        {
            //adds all statistics from the collection to this collection
            foreach (var quantity in statisticsCollection)
                this.Add(quantity.Key, quantity.Value);
        }

        public void AverageWith(StatisticsCollection collection)
        {
            //averages all possible quantities
            foreach (var quantityID in new List<string>(this.Keys))
                if (collection.ContainsKey(quantityID))
                    this[quantityID].AverageWith(collection[quantityID]);
        }

        public StatisticsCollection Clone()
        {
            //clones all quantities
            var statCollection = new StatisticsCollection
            {
                MaxNumSamples = this.MaxNumSamples,
                SampleSteps = this.SampleSteps
            };
            foreach (var quantity in this)
                statCollection.Add(quantity.Key, quantity.Value.Clone());
            return statCollection;
        }

        public virtual void Dispose()
        {
            foreach (var quantity in this.Values)
                quantity.Dispose();
            this.Clear();
        }

        public void InitParameters()
        {
            foreach (var quantity in this.Values)
            {
                quantity.SampleSteps = this.SampleSteps;
                quantity.MaxNumSamples = this.MaxNumSamples;
            }
        }

        public void PrintAllQuantitiesCountToCSV(string filePath)
        {
            PrintAllQuantitiesParameterToCSV(SUM_PARAM_NAME, filePath);
        }

        public void PrintAllQuantitiesMaxToCSV(string filePath)
        {
            PrintAllQuantitiesParameterToCSV(MAX_PARAM_NAME, filePath);
        }

        public void PrintAllQuantitiesParameterToCSV(string paramName, string filePath)
        {
            //checks directory and file
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            else if (File.Exists(filePath))
                File.Delete(filePath);

            var sw = new StreamWriter(filePath);

            //prints header
            sw.WriteLine("Quantity{0}{1}", SEPARATION_CHAR, paramName);

            //prints quantities parameter
            foreach (var statisticalQuantity in this)
            {
                var value = 0d;
                switch (paramName)
                {
                    case SUM_PARAM_NAME:
                        value = statisticalQuantity.Value.Sum;
                        break;

                    case MAX_PARAM_NAME:
                        value = statisticalQuantity.Value.Max;
                        break;
                }

                sw.WriteLine("{0}{1}{2}", statisticalQuantity.Key, SEPARATION_CHAR, value);
            }

            //closes stream
            sw.Close();
            sw.Dispose();
        }

        public void PrintAllQuantitiesToCSV(string filePath)
        {
            //checks args
            if ((filePath == null) || (this.Count == 0))
                return;

            //checks directory and file
            var directoryName = Path.GetDirectoryName(filePath);
            if ((directoryName != null) && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            else if (File.Exists(filePath))
                File.Delete(filePath);

            //crates file and writes header
            var sw = new StreamWriter(filePath);
            var line = this.Keys.Aggregate(
                $"t{SEPARATION_CHAR}",
                (current, quantityID) =>
                    string.Format("{0}{1}.val{2}{1}.StdDev{2}{1}.StdErr{2}{1}.Cumulative{2}",
                        current, quantityID, SEPARATION_CHAR));
            sw.WriteLine(line);

            //gets all quantities samples, one column per quantity
            var numSamples =
                this.Values.Max(quantity => quantity.Samples == null ? 0 : quantity.SampleCount);
            var labelFactor = numSamples / NUM_LABELS;
            var cumValues = this.Values.ToDictionary<StatisticalQuantity, StatisticalQuantity, double>(
                statisticalQuantity => statisticalQuantity, statisticalQuantity => 0);

            for (var i = 0; i < numSamples; i++)
            {
                line = $"{i / labelFactor}{SEPARATION_CHAR}";
                foreach (var statisticalQuantity in this.Values)
                {
                    var sample = statisticalQuantity.Samples[i];
                    var cumValue = cumValues[statisticalQuantity] += sample.Mean;

                    line = string.Format("{0}{1}{2}{3}{2}{4}{2}{5}{2}", line,
                        sample.Mean, SEPARATION_CHAR,
                        sample.StandardDeviation.ToString(CultureInfo.InvariantCulture),
                        sample.StandardError.ToString(CultureInfo.InvariantCulture),
                        cumValue.ToString(CultureInfo.InvariantCulture));
                }

                //removes last character
                line = line.Remove(line.Length - 1);
                sw.WriteLine(line);
            }

            //closes stream
            sw.Close();
            sw.Dispose();
        }

        public virtual void PrintResultsIndividualy(string basePath)
        {
            foreach (var statQuantityElem in this)
            {
                var quantity = statQuantityElem.Value;
                var csvFilePath = $"{basePath}/{this.GetFileStatName(statQuantityElem.Key)}.csv";
                quantity.PrintStatisticsToCSV(csvFilePath);
            }
        }

        public void SaveToJson(string filePath)
        {
            this.SerializeJsonFile(filePath, JsonUtil.TypeSpecifySettings, Formatting.Indented, true);
        }

        #endregion Public Methods

        #region Protected Methods

        protected string GetFileStatName(string statName)
        {
            return this.Prefix.Equals(string.Empty)
                ? statName
                : statName.Replace($"{this.Prefix}-", string.Empty);
        }

        protected string GetStatName(string statName)
        {
            return $"{this.Prefix}-{statName}";
        }

        #endregion Protected Methods
    }
}