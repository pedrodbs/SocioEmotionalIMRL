// ------------------------------------------
// <copyright file="StatisticsCollectionExtensions.cs" company="Pedro Sequeira">
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
//    Project: PS.Utilities.Forms

//    Last updated: 12/18/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using OxyPlot;
using OxyPlot.Series;
using PS.Utilities.Math;
using System.Collections.Generic;

namespace PS.Utilities.Forms.Math
{
    public struct DataErrorPoint
    {
        #region Public Properties

        public DataPoint Data { get; set; }
        public ScatterErrorPoint Error { get; set; }

        #endregion Public Properties
    }

    public static class StatisticsCollectionExtensions
    {
        #region Public Methods

        public static PlotModel CreateAndPrintChart(
            this StatisticsCollection statisticalQuantities,
            string filePath, string chartName, ChartType chartType = ChartType.Line,
            int width = 640, int height = 480, ChartImageFormat imageFormat = ChartImageFormat.Png)
        {
            var dataCollection = CreateDataPointCollection(statisticalQuantities);

            //ignore if no data
            if (dataCollection.Count == 0) return null;

            //creates chart
            var chartModel = PlottingUtil.CreateAndPrintChart(
                dataCollection, filePath, chartName, chartType, width, height, imageFormat);

            return chartModel;
        }

        public static PlotModel CreateChart(
            this StatisticsCollection statisticalQuantities,
            string chartName, ChartType chartType = ChartType.Line)
        {
            var data = CreateDataPointCollection(statisticalQuantities);
            return PlottingUtil.CreatePlotModel(data, chartName, chartType);
        }

        public static void PrintAllQuantitiesToImage(
            this StatisticsCollection statisticalQuantities, string filePath,
            ChartType chartType = ChartType.Line, ChartImageFormat imageFormat = ChartImageFormat.Png,
            int width = 1024, int height = 768)
        {
            CreateAndPrintChart(statisticalQuantities, filePath, "Chart", chartType, width, height, imageFormat);
        }

        #endregion Public Methods

        #region Private Methods

        private static Dictionary<string, IEnumerable<DataErrorPoint>> CreateDataPointCollection(
                                                    StatisticsCollection statisticalQuantities)
        {
            var dictionary = new Dictionary<string, IEnumerable<DataErrorPoint>>();
            foreach (var quantity in statisticalQuantities)
            {
                if (quantity.Value.Samples == null) continue;
                var samples = quantity.Value.Samples;
                var dataPoints = new List<DataErrorPoint>();
                for (var i = 0; i < quantity.Value.SampleCount; i++)
                {
                    var sample = samples[i];
                    var data = new DataPoint(i, sample.Mean);
                    var error = new ScatterErrorPoint(i, sample.Mean, 0, sample.StandardError);
                    var point = new DataErrorPoint { Data = data, Error = error };
                    dataPoints.Add(point);
                }

                //only add >0 point series
                if (dataPoints.Count > 0)
                    dictionary.Add(quantity.Key, dataPoints);
            }
            return dictionary;
        }

        #endregion Private Methods
    }
}