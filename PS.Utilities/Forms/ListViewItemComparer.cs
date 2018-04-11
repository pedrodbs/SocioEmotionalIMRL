// ------------------------------------------
// <copyright file="ListViewItemComparer.cs" company="Pedro Sequeira">
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

//    Last updated: 11/26/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections;
using System.Windows.Forms;

namespace PS.Utilities.Forms
{
    public class ListViewItemComparer : IComparer
    {
        private readonly int _column;
        private readonly SortOrder _sorting = SortOrder.Ascending;

        public ListViewItemComparer(int column, SortOrder sorting)
        {
            this._column = column;
            this._sorting = sorting;
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (!(x is ListViewItem) || !(y is ListViewItem)) return 0;
            var xText = ((ListViewItem) x).SubItems[this._column].Text;
            var yText = ((ListViewItem) y).SubItems[this._column].Text;
            return (this._sorting.Equals(SortOrder.Ascending) ? 1 : -1)*
                   String.CompareOrdinal(xText, yText);
        }

        #endregion
    }
}