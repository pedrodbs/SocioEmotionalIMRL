// ------------------------------------------
// <copyright file="TestMeasure.cs" company="Pedro Sequeira">
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
//    Project: Learning
//    Last updated: 12/09/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Utilities.Collections;
using PS.Utilities.Math;

namespace PS.Learning.Core.Testing
{
    public class TestMeasure : IComparable<TestMeasure>, ICsvConvertible
    {
        private static readonly CultureInfo CultureInfo = CultureInfo.InvariantCulture;

        #region IComparable<TestMeasure> Members

        public int CompareTo(TestMeasure other)
        {
            return this.Value.CompareTo(other.Value);
        }

        #endregion

        #region ICsvConvertible Members

        public virtual string[] ToValue()
        {
            return this.Parameters.ToValue().Append(
                new List<string>
                {
                    this.Value.ToString(CultureInfo),
                    this.StdDev.ToString(CultureInfo),
                    $"\"{this.ID}\""
                });
        }

        public virtual bool FromValue(string[] value)
        {
            if ((value == null) || (value.Length < 4)) return false;

            //gets test measure values
            var length = value.Length;
            this.ID = value[length - 1].Replace("\"", string.Empty);
            this.StdDev = double.Parse(value[length - 2], CultureInfo);
            this.Value = double.Parse(value[length - 3], CultureInfo);

            //let rest of values to parameter (has to be set beforehand!)
            return this.Parameters == null ||
                   this.Parameters.FromValue(value.SubArray(0, length - 3));
        }

        public virtual string[] Header => this.Parameters.Header.Append(new List<string> {"Fitness", "Std Dev", "ID"});

        #endregion

        public override string ToString()
        {
            return this.ID;
        }

        public bool IsStatiscallyDifferentFrom(TestMeasure testMeasure, int n, double minPvalue)
        {
            var ttestResult = TTest.CalcTTest(this.Value, testMeasure.Value, this.StdDev*this.StdDev,
                testMeasure.StdDev*testMeasure.StdDev, n, n);

            return ttestResult.PValue <= minPvalue;
        }

        #region Properties

        public ITestParameters Parameters { get; set; }
        public string ID { get; set; }
        public double Value { get; set; }
        public double StdDev { get; set; }
        public StatisticalQuantity Quantity { get; set; }

        #endregion
    }
}