// ------------------------------------------
// <copyright file="Range.cs" company="Pedro Sequeira">
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

//    Last updated: 04/28/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;

namespace PS.Utilities.Core
{
    public struct Range<T> where T : IComparable<T>
    {
        #region Public Fields

        public T max;
        public T min;

        #endregion Public Fields

        #region Public Constructors

        public Range(T min, T max)
        {
            if (min.CompareTo(max) > 0)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }
            this.max = max;
            this.min = min;
        }

        #endregion Public Constructors

        #region Public Methods

        public override string ToString()
        {
            return $"[{this.min},{this.max}]";
        }

        #endregion Public Methods
    }

    public static class Range
    {
        #region Public Fields

        public static readonly Range<double> FullDoubleRange = new Range<double>(double.MinValue, double.MaxValue);
        public static readonly Range<int> FullIntRange = new Range<int>(int.MinValue, int.MaxValue);

        #endregion Public Fields
    }
}