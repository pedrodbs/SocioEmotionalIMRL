// ------------------------------------------
// <copyright file="NumericArrayUtil.cs" company="Pedro Sequeira">
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

using PS.Utilities.Collections;
using System;

namespace PS.Utilities.Math
{
    public class NumericArrayUtil<T> where T : struct
    {
        #region Public Methods

        public static T[][] NumericArrayFromInterval(StepInterval<T>[] stepInts)
        {
            var numArrays = stepInts.Length;
            var arrays = new T[numArrays][];
            for (var i = 0; i < numArrays; i++)
                arrays[i] = NumericArrayFromInterval(stepInts[i]);
            return arrays;
        }

        public static T[] NumericArrayFromInterval(StepInterval<T> stepInt)
        {
            return NumericArrayFromInterval(stepInt.min, stepInt.max, stepInt.step);
        }

        public static T[] NumericArrayFromInterval(T min, T max, T interval)
        {
            //checks if type is numeric
            if (!IsTypeNumeric(typeof(T))) return null;

            //converts params
            var minVal = GetNumericValue(min);
            var maxVal = GetNumericValue(max);
            var intVal = GetNumericValue(interval);

            //creates array wrt given params
            var numElems = (int)((maxVal - minVal) / intVal) + 1;
            var array = new T[numElems];
            var i = 0;

            //fills array with values, from min to max with the given interval
            for (var val = minVal; val <= maxVal; val += intVal)
                array[i++] = CastNumber(val);

            return array;
        }

        #endregion Public Methods

        #region Protected Methods

        protected static T CastNumber(object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        protected static decimal GetNumericValue(T value)
        {
            return Decimal.Parse(value.ToString());
        }

        protected static bool IsTypeNumeric(Type type)
        {
            return type == typeof(sbyte)
                   || type == typeof(byte)
                   || type == typeof(short)
                   || type == typeof(ushort)
                   || type == typeof(int)
                   || type == typeof(uint)
                   || type == typeof(long)
                   || type == typeof(ulong)
                   || type == typeof(float)
                   || type == typeof(double)
                   || type == typeof(decimal);
        }

        #endregion Protected Methods
    }
}