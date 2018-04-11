// ------------------------------------------
// <copyright file="ECTestMeasure.cs" company="Pedro Sequeira">
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
//    Project: Learning.EvolutionaryComputation

//    Last updated: 02/06/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using PS.Learning.Core.Testing;
using PS.Utilities.Collections;

namespace PS.Learning.EvolutionaryComputation.Testing
{
    public class ECTestMeasure : TestMeasure
    {
        public int GenerationNumber { get; set; }
        public int TimesGenerated { get; set; }
        public override string[] Header => base.Header.Append(new List<string> {"Generation", "Times Generated"});

        public override string ToString()
        {
            return this.Parameters.ToString();
        }

        public override string[] ToValue()
        {
            return base.ToValue().Append(
                new List<string> {this.GenerationNumber.ToString(), this.TimesGenerated.ToString()});
        }

        public override bool FromValue(string[] value)
        {
            if ((value == null) || (value.Length < 6)) return false;
            var length = value.Length;
            this.TimesGenerated = int.Parse(value[length - 1]);
            this.GenerationNumber = int.Parse(value[length - 2]);
            return base.FromValue(value.SubArray(0, length - 2));
        }
    }
}