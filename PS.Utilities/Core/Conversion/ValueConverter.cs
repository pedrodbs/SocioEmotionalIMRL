// ------------------------------------------
// <copyright file="ValueConverter.cs" company="Pedro Sequeira">
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

//    Last updated: 03/30/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.ComponentModel;

namespace PS.Utilities.Core.Conversion
{
    public class ValueConverter
    {
        #region Public Methods

        public static bool Convert<TValue, TSource>(out TValue value, TSource source)
        {
            value = default(TValue);
            var valueType = typeof(TValue);

            if (valueType.IsAssignableFrom(typeof(IValueConvertible<TSource>)))
            {
                ((IValueConvertible<TSource>)value).FromValue(source);
                return true;
            }

            var converter = TypeDescriptor.GetConverter(valueType);
            if (!converter.CanConvertFrom(typeof(TSource))) return false;
            value = (TValue)converter.ConvertFrom(source);

            return true;
        }

        #endregion Public Methods
    }
}