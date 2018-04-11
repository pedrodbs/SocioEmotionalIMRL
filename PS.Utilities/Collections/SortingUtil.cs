// ------------------------------------------
// <copyright file="SortingUtil.cs" company="Pedro Sequeira">
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

using MathNet.Numerics.Random;
using PS.Utilities.Core;
using System;
using System.Collections.Generic;

namespace PS.Utilities.Collections
{
    public static class SortingUtil
    {
        #region Private Fields

        private static readonly Random Random = new WH2006();

        #endregion Private Fields

        #region Public Methods

        public static int GetSortIndex<T>(this IList<T> list, T item, bool randomWhenEqual = false)
            where T : IComparable<T>
        {
            //checks trivial conditions
            if (list.Count == 0) return 0;
            var minIdx = 0;
            var maxIdx = list.Count;

            //tests against min index value
            var itemToMin = item.CompareTo(list[minIdx]);
            if (itemToMin.Equals(0))
                return randomWhenEqual
                    ? GetEqualsRandomIndex(list, item)
                    : minIdx + 1;
            if (itemToMin < 0)
                return minIdx;

            //tests against max index value
            var itemToMax = item.CompareTo(list[maxIdx - 1]);
            if (itemToMax.Equals(0))
                return randomWhenEqual
                    ? GetEqualsRandomIndex(list, item)
                    : maxIdx;
            if (itemToMax > 0)
                return maxIdx;

            //calculates an index to insert the given sac
            while (!minIdx.Equals(maxIdx))
            {
                //tests against mid index value
                var midIdx = minIdx + ((maxIdx - minIdx) / 2);
                var itemToMid = item.CompareTo(list[midIdx]);
                if (itemToMid.Equals(0))
                    return randomWhenEqual
                        ? GetEqualsRandomIndex(list, item, midIdx)
                        : midIdx + 1;

                //chooses search direction
                if (itemToMid > 0)
                    minIdx = minIdx.Equals(midIdx) ? midIdx + 1 : midIdx;
                else
                    maxIdx = midIdx;
            }
            return minIdx;
        }

        public static void InsertSorted<T>(this IList<T> list, T item, bool randomWhenEqual = false)
            where T : IComparable<T>
        {
            list.Insert(GetSortIndex(list, item, randomWhenEqual), item);
        }

        #endregion Public Methods

        #region Private Methods

        private static int GetEqualsRandomIndex<T>(IList<T> list, T item, int startIdx = 0)
            where T : IComparable<T>
        {
            var equalsRange = list.GetEqualsRange(item, startIdx);
            return Random.Next(equalsRange.min, equalsRange.max + 1);
        }

        private static Range<int> GetEqualsRange<T>(this IList<T> list, T item, int startIdx = 0)
            where T : IComparable<T>
        {
            var range = new Range<int>(int.MinValue, int.MaxValue);

            //gets min idx
            var i = startIdx;
            for (; i < list.Count; i++)
                if (list[i].CompareTo(item).Equals(0))
                    break;
            range.min = i;

            //gets max idx
            for (; i < list.Count; i++)
                if (!list[i].CompareTo(item).Equals(0))
                    break;
            range.max = i;
            return range;
        }

        #endregion Private Methods
    }
}