// ------------------------------------------
// <copyright file="ArrayUtil.cs" company="Pedro Sequeira">
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
//    Project: PS.Utilities.Collections

//    Last updated: 11/04/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Utilities.Core.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS.Utilities.Collections
{
    public static class ArrayUtil
    {
        #region Private Fields

        private const char DEFAULT_SEPARATOR = ',';

        #endregion Private Fields

        #region Public Methods

        public static List<T[]> AllCombinations<T>(this T[] array, uint numElements, bool repsAllowed)
        {
            var allCombs = new List<T[]>();
            for (uint curIdx = 0; curIdx < array.Length; curIdx++)
                AllCombinations(array, numElements, repsAllowed, allCombs, curIdx, new T[0]);
            return allCombs;
        }

        public static List<T[]> AllCombinations<T>(this T[][] matrix)
        {
            var allCombs = new List<T[]>();
            AllCombinations(matrix, allCombs, 0, 0, new T[0]);
            return allCombs;
        }

        public static List<T[]> AllPermutations<T>(this T[] array, uint numElements, bool repsAllowed)
        {
            var allPerms = new List<T[]>();
            for (uint curIdx = 0; curIdx < array.Length; curIdx++)
                AllPermutations(array, numElements, repsAllowed, allPerms, curIdx, new T[0]);
            return allPerms;
        }

        public static T[] Append<T>(this T[] array, T element)
        {
            return Append(array, new List<T> { element });
        }

        public static T[] Append<T>(this T[] array, IEnumerable<T> elements)
        {
            if ((array == null) || (elements == null)) return null;

            var array2 = elements.ToArray();
            var newArray = new T[array.Length + array2.Length];
            array.CopyTo(newArray, 0);
            array2.CopyTo(newArray, array.Length);
            return newArray;
        }

        public static T[][] Create2DArray<T>(uint numX, uint numY)
        {
            var array = new T[numX][];
            for (var x = 0; x < numX; x++)
                array[x] = new T[numY];
            return array;
        }

        public static T[][][] Create3DArray<T>(uint numX, uint numY, uint numZ)
        {
            var array = new T[numX][][];
            for (var x = 0; x < numX; x++)
            {
                array[x] = new T[numY][];
                for (var y = 0; y < numY; y++)
                    array[x][y] = new T[numZ];
            }
            return array;
        }

        public static bool Equals<T>(this T[] array1, T[] array2)
        {
            if (array1.Length != array2.Length) return false;
            for (var i = 0; i < array1.Length; i++)
                if (!array1[i].Equals(array2[i]))
                    return false;
            return true;
        }

        public static void ForEach<T>(this T[] array, Func<T, T> func)
        {
            if (array == null) return;
            for (var i = 0; i < array.Length; i++)
                array[i] = func(array[i]);
        }

        public static T[] FromString<T>(string strValue, char separator = DEFAULT_SEPARATOR)
        {
            return string.IsNullOrWhiteSpace(strValue)
                ? null
                : FromStringArray<T>(strValue.Split(separator));
        }

        public static T[] FromStringArray<T>(string[] strValues)
        {
            if (strValues == null) return null;

            var values = new List<T>(strValues.Length);
            foreach (var field in strValues)
            {
                T value;
                if (ValueConverter.Convert(out value, field))
                    values.Add(value);
            }
            return values.ToArray();
        }

        public static void Initialize<T>(this T[] array, T element)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = element;
        }

        public static List<T[]> Split<T>(this T[] array, uint numSplits)
        {
            if ((array == null) || (array.Length < numSplits))
                return new List<T[]>();

            if ((numSplits == 0) || (array.Length == 0))
                return new List<T[]> { array };

            var list = new List<T[]>((int)numSplits);
            var newArrayLength = (int)(array.Length / numSplits);
            for (var i = 0; i < numSplits; i++)
                list.Add(array.SubArray(i * newArrayLength, newArrayLength));
            return list;
        }

        public static T[] SubArray<T>(this T[] array, int startIndex)
        {
            if ((array == null) || (startIndex >= array.Length))
                return null;

            return SubArray(array, startIndex, array.Length - startIndex);
        }

        public static T[] SubArray<T>(this T[] array, int startIndex, int length)
        {
            if ((array == null) || (length <= 0))
                return null;

            //caps length
            length = System.Math.Min(length, array.Length - startIndex);

            var result = new T[length];
            Array.Copy(array, startIndex, result, 0, length);
            return result;
        }

        public static string ToString<T>(this T[] array, char separator = DEFAULT_SEPARATOR,
            bool includeBrackets = false, string prefix = null, string postfix = null)
        {
            var str = new StringBuilder(includeBrackets ? "[" : string.Empty);
            if ((array != null) && (array.Length > 0))
            {
                foreach (var elem in array)
                    str.Append($"{prefix}{elem}{postfix}{separator}");
                str.Remove(str.Length - 1, 1);
            }
            if (includeBrackets) str.Append("]");
            return str.ToString();
        }

        public static string[] ToStringArray<T>(this T[] array)
        {
            if (array == null) return null;

            var values = new string[array.Length];
            for (var i = 0; i < array.Length; i++)
                values[i] = array[i].ToString();
            return values;
        }

        public static string ToVectorString<T>(this T[] array)
        {
            return ToString(array, ',', true);
        }

        #endregion Public Methods

        #region Private Methods

        private static void AllCombinations<T>(
            this T[] array, uint numElements, bool repsAllowed, List<T[]> allCombs, uint curIdx, T[] curPrefix)
        {
            if ((numElements < 1) || (numElements > array.Length) || (curIdx >= array.Length))
                return;

            var newPrefix = Append(curPrefix, array[curIdx]);
            if (newPrefix.Length == numElements)
                allCombs.Add(newPrefix);
            else
                for (var auxIdx = (repsAllowed ? curIdx : curIdx + 1); auxIdx < array.Length; auxIdx++)
                    AllCombinations(array, numElements, repsAllowed, allCombs, auxIdx, newPrefix);
        }

        private static void AllCombinations<T>(
            this T[][] matrix, List<T[]> allCombs, uint curIdx, uint curElemIdx, T[] curPrefix)
        {
            if ((curIdx >= matrix.Length) || (curElemIdx >= matrix[(int)curIdx].Length))
                return;

            var newPrefix = Append(curPrefix, matrix[(int)curIdx][curElemIdx]);
            if (newPrefix.Length == matrix.Length)
                allCombs.Add(newPrefix);
            else
                for (var auxIdx = curIdx + 1; auxIdx < matrix.Length; auxIdx++)
                    AllCombinations(matrix, allCombs, auxIdx, 0, newPrefix);

            AllCombinations(matrix, allCombs, curIdx, curElemIdx + 1, curPrefix);
        }

        private static void AllPermutations<T>(
            this T[] array, uint numElements, bool repsAllowed, List<T[]> allPerms, uint curIdx, T[] curPrefix)
        {
            if ((numElements < 1) || (numElements > array.Length) || (curIdx >= array.Length) ||
                (!repsAllowed && Array.Exists(curPrefix, elem => elem.Equals(array[curIdx]))))
                return;

            var newPrefix = Append(curPrefix, array[curIdx]);
            if (newPrefix.Length == numElements)
                allPerms.Add(newPrefix);
            else
                for (uint auxIdx = 0; auxIdx < array.Length; auxIdx++)
                    AllPermutations(array, numElements, repsAllowed, allPerms, auxIdx, newPrefix);
        }

        #endregion Private Methods
    }
}