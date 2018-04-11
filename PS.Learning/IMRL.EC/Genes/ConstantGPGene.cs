// ------------------------------------------
// <copyright file="ConstantGPGene.cs" company="Pedro Sequeira">
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

//    Last updated: 2/6/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using AForge.Genetic;

namespace PS.Learning.IMRL.EC.Genes
{
    public class ConstantGPGene : IGPGene
    {
        public ConstantGPGene(double value)
        {
            this.Value = value;
        }

        public double Value { get; set; }

        #region IGPGene Members

        public IGPGene Clone()
        {
            return new ConstantGPGene(this.Value);
        }

        public void Generate()
        {
        }

        public void Generate(GPGeneType type)
        {
        }

        public IGPGene CreateNew()
        {
            return this.Clone();
        }

        public IGPGene CreateNew(GPGeneType type)
        {
            return this.Clone();
        }

        public GPGeneType GeneType
        {
            get { return GPGeneType.Argument; }
        }

        public int ArgumentsCount
        {
            get { return 0; }
        }

        public int MaxArgumentsCount
        {
            get { return 0; }
        }

        #endregion

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}