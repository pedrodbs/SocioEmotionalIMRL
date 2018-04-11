// ------------------------------------------
// <copyright file="Util.cs" company="Pedro Sequeira">
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
//    Project: Learning.Forms

//    Last updated: 01/16/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using MathNet.Numerics.Random;
using PS.Learning.Core.Testing.Config;

namespace PS.Learning.Forms
{
    public sealed class Util
    {
        private const string JSON_CONFIG_FILES_FILTER = "Json Config files|*.json";
        private const string JSON_CONFIG_FILES_TITLE = "Select tests configuration file";
        private const string JSON_CONFIG_FILES_NAME = "tests-config.json";
        private static readonly Random Rand = new WH2006(true);

        public static Color RandomColor
        {
            get
            {
                var length = Enum.GetValues(typeof (KnownColor)).Length;
                return Color.FromKnownColor((KnownColor) Rand.Next(length));
            }
        }

        public static string GetImageFilesDialogFilter()
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            var sep = string.Empty;
            var filterStr = new StringBuilder();

            foreach (var codecInfo in codecs)
            {
                var codecName = codecInfo.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                filterStr.Append(string.Format("{0}{1} ({2})|{2}", sep, codecName, codecInfo.FilenameExtension.ToLower()));
                sep = "|";
            }

            filterStr.Append(string.Format("{0}{1} ({2})|{2}", sep, "All Files", "*.*"));

            return filterStr.ToString();
        }

        public static List<ITestsConfig> SelectReadTestsConfig(string message = null)
        {
            var fileName = SelectOpenTestsConfigFile(message);
            return fileName == null ? new List<ITestsConfig>() : TestsConfig.DeserializeJsonFile(fileName);
        }

        private static string SelectOpenTestsConfigFile(string message = null)
        {
            var openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Filter = JSON_CONFIG_FILES_FILTER,
                Title = message ?? JSON_CONFIG_FILES_TITLE,
                FileName = JSON_CONFIG_FILES_NAME
            };
            return openFileDialog.ShowDialog().Equals(DialogResult.OK) ? openFileDialog.FileName : null;
        }

        public static string SelectSaveTestsConfigFile(string message = null)
        {
            var openFileDialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                Filter = JSON_CONFIG_FILES_FILTER,
                Title = message ?? JSON_CONFIG_FILES_TITLE,
                FileName = JSON_CONFIG_FILES_NAME
            };
            return openFileDialog.ShowDialog().Equals(DialogResult.OK) ? openFileDialog.FileName : null;
        }
    }
}