// ------------------------------------------
// <copyright file="GPTestMeasure.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL.EC

//    Last updated: 02/06/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.EvolutionaryComputation.Testing;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Utilities.Collections;
using System.Collections.Generic;

namespace PS.Learning.IMRL.EC.Testing
{
    public class GPTestMeasure : ECTestMeasure
    {
        #region Public Properties

        public override string[] Header
            => base.Header.Append(new List<string> { "Translated Expression", "Length", "Depth" });

        public string TranslatedExpression { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override bool FromValue(string[] value)
        {
            if (!(this.Parameters is GPChromosome)) return base.FromValue(value);
            if ((value == null) || (value.Length < 9)) return false;
            var length = value.Length;
            this.TranslatedExpression = value[length - 3].Replace("\"", string.Empty);
            return base.FromValue(value.SubArray(0, length - 3));
        }

        public override string[] ToValue()
        {
            var value = base.ToValue();
            if (!(this.Parameters is GPChromosome)) return value;
            var gpChromosome = (GPChromosome)this.Parameters;
            return value.Append(new List<string>
                                {
                                    $"\"{this.TranslatedExpression}\"",
                                    gpChromosome.Length.ToString(),
                                    gpChromosome.Depth.ToString()
                                });
        }

        #endregion Public Methods
    }
}