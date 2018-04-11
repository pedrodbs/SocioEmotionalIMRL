// ------------------------------------------
// <copyright file="FlexibleGPGene.cs" company="Pedro Sequeira">
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
using System;
using System.Collections.Generic;
using System.Linq;
using AForge.Genetic;
using MathNet.Numerics.Random;

namespace PS.Learning.IMRL.EC.Genes
{
    [Serializable]
    public enum FunctionType
    {
        /// <summary>
        ///     Addition operator.
        /// </summary>
        Add,

        /// <summary>
        ///     Suntraction operator.
        /// </summary>
        Subtract,

        /// <summary>
        ///     Multiplication operator.
        /// </summary>
        Multiply,

        /// <summary>
        ///     Division operator.
        /// </summary>
        Divide,

        /// <summary>
        ///     Sine function.
        /// </summary>
        Sin,

        /// <summary>
        ///     Cosine function.
        /// </summary>
        Cos,

        /// <summary>
        ///     Natural logarithm function.
        /// </summary>
        Ln,

        /// <summary>
        ///     Exponent function.
        /// </summary>
        Exp,

        /// <summary>
        ///     Square root function.
        /// </summary>
        Sqrt
    }

    [Serializable]
    public class FlexibleGPGene : IGPGene
    {
        [NonSerialized] protected static readonly Random Rand = new WH2006(true);
        protected readonly List<FunctionType> allowedFunctions = new List<FunctionType>();
        protected readonly int variablesCount;
        protected int val;

        public FlexibleGPGene(List<FunctionType> allowedFunctions, int variablesCount)
            : this(allowedFunctions, variablesCount, true)
        {
        }

        public FlexibleGPGene(List<FunctionType> allowedFunctions, int variablesCount, GPGeneType type)
        {
            this.variablesCount = variablesCount;
            this.allowedFunctions = allowedFunctions;

            // generate the gene value
            this.Generate(type);
        }

        // Private constructor
        private FlexibleGPGene(List<FunctionType> allowedFunctions, int variablesCount, bool random)
        {
            this.variablesCount = variablesCount;
            this.allowedFunctions = allowedFunctions;

            // generate the gene value
            if (random) this.Generate();
        }

        public static HashSet<FunctionType> AllFunctions
        {
            get
            {
                //returns a list containing all possible functions
                var retVal = new HashSet<FunctionType>();
                var functionCount = Enum.GetNames(typeof (FunctionType)).Length;
                for (var i = 0; i < functionCount; i++)
                    retVal.Add((FunctionType) i);
                return retVal;
            }
        }

        #region IGPGene Members

        /// <summary>
        ///     Gene type.
        /// </summary>
        /// <remarks>
        ///     <para>The property represents type of a gene - function, argument, etc.</para>
        /// </remarks>
        public GPGeneType GeneType { get; private set; }

        /// <summary>
        ///     Arguments count.
        /// </summary>
        /// <remarks>
        ///     <para>Arguments count of a particular function gene.</para>
        /// </remarks>
        public int ArgumentsCount { get; private set; }

        /// <summary>
        ///     Maximum arguments count.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Maximum arguments count of a function gene supported by the class.
        ///         The property may be used by chromosomes' classes to allocate correctly memory for
        ///         functions' arguments, for example.
        ///     </para>
        /// </remarks>
        public int MaxArgumentsCount
        {
            get { return 2; }
        }

        /// <summary>
        ///     Clone the gene.
        /// </summary>
        /// <remarks>
        ///     <para>The method clones the chromosome returning the exact copy of it.</para>
        /// </remarks>
        public IGPGene Clone()
        {
            // create new gene with the same type and value
            return new FlexibleGPGene(this.allowedFunctions, this.variablesCount, false)
                   {
                       GeneType = this.GeneType,
                       val = this.val,
                       ArgumentsCount = this.ArgumentsCount
                   };
        }

        /// <summary>
        ///     Randomize gene with random type and value.
        /// </summary>
        /// <remarks>
        ///     <para>The method randomizes the gene, setting its type and value randomly.</para>
        /// </remarks>
        public void Generate()
        {
            // give more chance to function
            this.Generate((Rand.Next(4) == 3) ? GPGeneType.Argument : GPGeneType.Function);
        }

        /// <summary>
        ///     Randomize gene with random value.
        /// </summary>
        /// <param name="type">Gene type to set.</param>
        /// <remarks>
        ///     <para>
        ///         The method randomizes a gene, setting its value randomly, but type
        ///         is set to the specified one.
        ///     </para>
        /// </remarks>
        public void Generate(GPGeneType type)
        {
            // gene type
            this.GeneType = type;
            // gene value
            this.val = Rand.Next((type == GPGeneType.Function) ? this.allowedFunctions.Count : this.variablesCount);
            // arguments count
            this.ArgumentsCount = (type == GPGeneType.Argument)
                ? 0
                : (this.val <= (int) FunctionType.Divide) ? 2 : 1;
        }

        /// <summary>
        ///     Creates new gene with random type and value.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The method creates new randomly initialized gene .
        ///         The method is useful as factory method for those classes, which work with gene's interface,
        ///         but not with particular gene class.
        ///     </para>
        /// </remarks>
        public IGPGene CreateNew()
        {
            return new FlexibleGPGene(this.allowedFunctions, this.variablesCount);
        }

        /// <summary>
        ///     Creates new gene with certain type and random value.
        /// </summary>
        /// <param name="type">Gene type to create.</param>
        /// <remarks>
        ///     <para>
        ///         The method creates new gene with specified type, but random value.
        ///         The method is useful as factory method for those classes, which work with gene's interface,
        ///         but not with particular gene class.
        ///     </para>
        /// </remarks>
        public IGPGene CreateNew(GPGeneType type)
        {
            return new FlexibleGPGene(this.allowedFunctions, this.variablesCount, type);
        }

        #endregion

        public static string GetFunctionTypeString(FunctionType functionType)
        {
            // get function string representation
            switch (functionType)
            {
                case FunctionType.Add: // addition
                    return "+";
                case FunctionType.Subtract: // subtraction
                    return "-";
                case FunctionType.Multiply: // multiplication
                    return "*";
                case FunctionType.Divide: // division
                    return "/";
                case FunctionType.Sin: // sine
                    return "sin";
                case FunctionType.Cos: // cosine
                    return "cos";
                case FunctionType.Ln: // natural logarithm
                    return "ln";
                case FunctionType.Exp: // exponent
                    return "exp";
                case FunctionType.Sqrt: // square root
                    return "sqrt";
                default:
                    return "";
            }
        }

        public static FunctionType GetFunctionType(string functionTypeStr)
        {
            foreach (var functionType in AllFunctions)
                if (GetFunctionTypeString(functionType) == functionTypeStr)
                    return functionType;
            return AllFunctions.First();
        }

        public FlexibleGPGene CreateNew(string geneStr)
        {
            var geneFunc = (FlexibleGPGene) this.CreateNew();
            geneFunc.GeneType = this.GetGeneType(geneStr);
            if (geneFunc.GeneType == GPGeneType.Argument)
                int.TryParse(geneStr.Replace("$", ""), out geneFunc.val);
            else
                geneFunc.val = (int) GetFunctionType(geneStr);
            geneFunc.ArgumentsCount = (geneFunc.GeneType == GPGeneType.Argument)
                ? 0
                : (geneFunc.val <= (int) FunctionType.Divide) ? 2 : 1;
            return geneFunc;
        }

        protected GPGeneType GetGeneType(string geneStr)
        {
            return geneStr.StartsWith("$") ? GPGeneType.Argument : GPGeneType.Function;
        }

        /// <summary>
        ///     Get string representation of the gene.
        /// </summary>
        /// <returns>Returns string representation of the gene.</returns>
        public override string ToString()
        {
            // get function or argument string representation
            return GeneType == GPGeneType.Function
                ? GetFunctionTypeString(this.allowedFunctions[this.val])
                : string.Format("${0}", this.val);
        }
    }
}