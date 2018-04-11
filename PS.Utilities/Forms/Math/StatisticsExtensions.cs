// ------------------------------------------
// <copyright file="StatisticsExtensions.cs" company="Pedro Sequeira">
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

using PS.Utilities.IO;
using PS.Utilities.Math;
using System.IO;

namespace PS.Utilities.Forms.Math
{
    public static class StatisticsExtensions
    {
        #region Private Fields

        private const int DEFAULT_IMAGE_HEIGHT = 300;
        private const int DEFAULT_IMAGE_WIDTH = 400;

        #endregion Private Fields

        #region Public Methods

        public static void PrintStatisticsToCSV(
            this StatisticalQuantity quantity, string filePath, bool printValuesOnly = false,
            bool printImage = true, ChartType chartType = ChartType.Line, 
            int width= DEFAULT_IMAGE_WIDTH, int height = DEFAULT_IMAGE_HEIGHT,
            ChartImageFormat imageFormat = ChartImageFormat.Pdf)
        {
            quantity.PrintStatisticsToCSV(filePath, printValuesOnly);

            if (printImage)
                quantity.PrintStatisticsToImage(
                    PathUtil.ReplaceExtension(filePath, imageFormat.ToString().ToLower()),
                    chartType, width, height, imageFormat);
        }

        public static void PrintStatisticsToImage(
            this StatisticalQuantity quantity, string filePath, ChartType chartType,
            int width=DEFAULT_IMAGE_WIDTH, int height = DEFAULT_IMAGE_HEIGHT,
            ChartImageFormat imageFormat= ChartImageFormat.Pdf)
        {
            if ((quantity.Samples == null) || (quantity.Samples.Length == 0)) return;

            //creates chart
            var title = Path.GetFileNameWithoutExtension(filePath);
            new StatisticsCollection { { title, quantity } }.CreateAndPrintChart(
                filePath, title, chartType, width, height, imageFormat);
        }

        #endregion Public Methods
    }
}