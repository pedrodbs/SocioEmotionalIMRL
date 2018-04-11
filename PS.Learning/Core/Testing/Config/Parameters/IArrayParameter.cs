// ------------------------------------------
// <copyright file="IArrayParameter.cs" company="Pedro Sequeira">
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
//    Project: PS.Learning.Core

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Utilities.Core;
using System.Collections.Generic;

namespace PS.Learning.Core.Testing.Config.Parameters
{
    public interface IArrayParameter : ITestParameters, IEnumerable<double>
    {
        #region Public Properties

        double AbsoulteSum { get; }
        double[] Array { get; }
        Range<double>[] Domains { get; }
        uint Length { get; }
        uint NumDecimalPlaces { get; set; }
        double Sum { get; }

        #endregion Public Properties

        #region Public Indexers

        double this[int paramIdx] { get; set; }

        #endregion Public Indexers

        #region Public Methods

        void NormalizeSum();

        void NormalizeUnitSum();

        void NormalizeVector();

        void Round();

        void SetMidDomainValues();

        #endregion Public Methods
    }
}