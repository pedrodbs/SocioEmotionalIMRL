// ------------------------------------------
// <copyright file="PlottingUtil.cs" company="Pedro Sequeira">
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

using MathNet.Numerics.Random;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using PS.Utilities.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SvgExporter = OxyPlot.SvgExporter;

namespace PS.Utilities.Forms.Math
{
    public enum ChartImageFormat
    {
        Png,
        Pdf,
        Svg,
        Xps
    }

    public enum ChartType
    {
        Line,
        Column,
        Area,
        Pie
    }

    public class PlottingUtil
    {
        #region Private Fields

        private const int DEFAULT_IMAGE_HEIGHT = 768;
        private const int DEFAULT_IMAGE_WIDTH = 1024;
        private const int ERROR_COLOR_ALPHA = 70;
        private const int ERROR_MARKER_SIZE = 2;
        private const int ERROR_STROKE_THICKNESS = 1;
        private const string FONT_NAME = "Times"; //"Verdana"; //"Segoe UI";
        private const int FONT_SIZE = 11;
        private const int STROKE_THICKNESS = 3;

        //2;
        //3;
        private static readonly Random Rand = new WH2006(true);

        #endregion Private Fields

        #region Public Methods

        public static PlotModel CreateAndPrintChart(
            Dictionary<string, IEnumerable<DataErrorPoint>> data,
            string filePath, string chartName,
            ChartType chartType = ChartType.Line,
            int width = DEFAULT_IMAGE_HEIGHT, int height = DEFAULT_IMAGE_WIDTH, ChartImageFormat imageFormat = ChartImageFormat.Png)
        {
            //creates chart engine
            var chartModel = CreatePlotModel(data, chartName, chartType);

            //export chart as image file
            ExportChartToImage(filePath, width, height, chartModel, imageFormat);

            return chartModel;
        }

        public static Series CreateChartSeries(ChartType chartType)
        {
            return CreateChartSeries(chartType, new List<DataErrorPoint>(), CreateRandomColor());
        }

        public static OxyPalette CreatePalette(IDictionary<string, IEnumerable<DataErrorPoint>> data)
        {
            return (data == null) || (data.Count == 0)
                ? OxyPalettes.Hue64
                : OxyPalettes.Jet(System.Math.Max(4, data.Count));
        }

        public static PlotModel CreatePlotModel(
            IDictionary<string, IEnumerable<DataErrorPoint>> data,
            string chartName, ChartType chartType)
        {
            //creates chart engine
            var plotModel = CreatePlotModel(chartName, data, chartType);

            //creates new chart for the given quantities
            AddChartSeries(plotModel, data, chartType);

            return plotModel;
        }

        public static PlotModel CreatePlotModel(
            string chartName, IDictionary<string, IEnumerable<DataErrorPoint>> data, ChartType chartType)
        {
            var plotModel = new PlotModel
            {
                Title = chartName,
                Background = OxyColor.FromRgb(255, 255, 255),
                TitleFont = FONT_NAME,
                TitleFontSize = FONT_SIZE + 6,
                TitleFontWeight = FontWeights.Bold,
                IsLegendVisible = chartType != ChartType.Pie && data.Count > 1,
                LegendPosition = LegendPosition.TopLeft,
                LegendBorderThickness = 1,
                LegendBorder = OxyColor.FromRgb(0, 0, 0),
                LegendBackground = OxyColor.FromRgb(255, 255, 255),
                LegendFontSize = FONT_SIZE,
                LegendFont = FONT_NAME,
                LegendFontWeight = FontWeights.Normal,
                DefaultColors = CreatePalette(data).Colors
            };

            //gets max X and Y axis values
            GetAxesMinMax(data.Values, out var maxX, out var minY, out var maxY);

            // Y-axis
            plotModel.Axes.Add(new LinearAxis
            {
                Minimum = minY,
                Maximum = maxY,
                MajorGridlineStyle = LineStyle.Dot,
                Font = FONT_NAME,
                FontSize = FONT_SIZE + 1
                //FontWeight = FontWeights.Bold
            });

            // X-axis
            var xAxis = chartType.Equals(ChartType.Column) ? new CategoryAxis() : new LinearAxis();
            xAxis.Minimum = chartType.Equals(ChartType.Column) ? -0.5 : 0;
            xAxis.Maximum = maxX - (chartType.Equals(ChartType.Column) ? 0.5 : 1);
            xAxis.Position = AxisPosition.Bottom;
            xAxis.Font = FONT_NAME;
            xAxis.FontSize = FONT_SIZE + 1;
            //xAxis.FontWeight = FontWeights.Bold;

            plotModel.Axes.Add(xAxis);
            return plotModel;
        }

        public static OxyColor CreateRandomColor()
        {
            return OxyColor.FromRgb((byte)Rand.Next(256), (byte)Rand.Next(256), (byte)Rand.Next(256));
        }

        public static void ExportChartToImage(
            string filePath, int width, int height, PlotModel chartModel, ChartImageFormat imageFormat)
        {
            using (var stream = File.Create(filePath))
            {
                switch (imageFormat)
                {
                    case ChartImageFormat.Svg:
                        new SvgExporter { Width = width, Height = height }.Export(chartModel, stream);
                        break;

                    case ChartImageFormat.Pdf:
                        new PdfExporter { Width = width, Height = height }.Export(chartModel, stream);
                        break;

                    default:
                        new PngExporter { Width = width, Height = height }.Export(chartModel, stream);
                        break;
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void AddChartSeries(
            PlotModel plotModel,
            IDictionary<string, IEnumerable<DataErrorPoint>> data,
            ChartType chartType)
        {
            //creates a series for each data set presented
            var i = 0;
            foreach (var chartPointCollection in data)
            {
                var dataId = chartPointCollection.Key;
                var dataErrorPoints = chartPointCollection.Value as IList<DataErrorPoint> ??
                                      chartPointCollection.Value.ToList();
                var color = plotModel.DefaultColors[i++];

                //adds error points series
                var errorSeries = CreateErrorSeries(dataErrorPoints, color);
                errorSeries.Title = null;
                plotModel.Series.Add(errorSeries);

                //adds data points series
                var dataSeries = CreateChartSeries(chartType, dataErrorPoints, color);
                dataSeries.Title = dataId;
                plotModel.Series.Add(dataSeries);
            }
        }

        private static Series CreateChartSeries(
            ChartType chartType, IEnumerable<DataErrorPoint> data, OxyColor color)
        {
            var dataPoints = data.Select(d => d.Data).ToList();
            Series series;
            switch (chartType)
            {
                case ChartType.Area:
                    series = new AreaSeries { Color = color };
                    ((AreaSeries)series).Points.AddRange(dataPoints);
                    break;

                case ChartType.Column:
                    series = new ColumnSeries();
                    foreach (var dataPoint in dataPoints)
                        ((ColumnSeries)series).Items.Add(new ColumnItem(dataPoint.Y) { Color = color });
                    break;

                case ChartType.Pie:
                    series = new PieSeries();
                    foreach (var dataPoint in dataPoints)
                        ((PieSeries)series).Slices.Add(new PieSlice(dataPoint.X.ToString(), dataPoint.Y));
                    break;

                default:
                    series = new LineSeries();
                    var lineSeries = ((LineSeries)series);
                    lineSeries.Points.AddRange(dataPoints);
                    lineSeries.StrokeThickness = STROKE_THICKNESS;
                    lineSeries.Color = color;
                    break;
            }
            return series;
        }

        private static ScatterErrorSeries CreateErrorSeries(
            IEnumerable<DataErrorPoint> data, OxyColor color)
        {
            var aColor = OxyColor.FromAColor(ERROR_COLOR_ALPHA, color);
            var errorSeries = new ScatterErrorSeries
            {
                MarkerSize = ERROR_MARKER_SIZE,
                MarkerFill = aColor,
                ErrorBarColor = aColor,
                ErrorBarStrokeThickness = ERROR_STROKE_THICKNESS
            };
            errorSeries.Points.AddRange(data.Select(d => d.Error));
            return errorSeries;
        }

        private static void GetAxesMinMax(IEnumerable<IEnumerable<DataErrorPoint>> data,
            out double maxX, out double minY, out double maxY)
        {
            maxX = maxY = double.MinValue;
            minY = double.MaxValue;
            foreach (var points in data)
            {
                var x = 0d;
                foreach (var point in points)
                {
                    minY = System.Math.Min(minY, point.Data.Y - (0.5 * point.Error.ErrorY));
                    maxY = System.Math.Max(maxY, point.Data.Y + (0.5 * point.Error.ErrorY));
                    x++;
                }

                maxX = System.Math.Max(maxX, x);
            }
        }

        #endregion Private Methods
    }
}