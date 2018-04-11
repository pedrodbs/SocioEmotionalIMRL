// ------------------------------------------
// <copyright file="GPMotivationManager.cs" company="Pedro Sequeira">
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

//    Last updated: 05/23/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using AForge;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.IMRL.Domain.Managers.Motivation;

namespace PS.Learning.IMRL.EC.Domain
{
    public abstract class GPMotivationManager : IntrinsicMotivationManager
    {
        protected readonly double[] constants;
        protected string translatedExpression;

        protected GPMotivationManager(IGPAgent agent, double[] constants)
            : base(agent)
        {
            this.constants = constants;
        }

        protected abstract string[] VariablesNames { get; }

        protected int NumVariables
        {
            get { return this.VariablesNames.Length; }
        }

        public new IGPAgent Agent
        {
            get { return base.Agent as IGPAgent; }
        }

        public string TranslatedExpression
        {
            get { return this.translatedExpression ?? this.GetTranslatedExpression(); }
        }

        protected long NumInputElements
        {
            get { return this.NumVariables + this.constants.Length; }
        }

        public override double GetIntrinsicReward(uint prevState, uint action, uint nextState)
        {
            //just execute the program with given input
            var variables = this.GetRewardFeatures(prevState, action, nextState).ToArray();
            var expression = this.Agent.Chromosome.ToString();
            var rwd = PolishExpression.Evaluate(expression, variables);

            return (double.IsNaN(rwd) || double.IsInfinity(rwd)) ? 0 : rwd;
        }

        protected void CopyConstants(DenseVector vector)
        {
            //just copy constants into given vector
            Array.Copy(constants, 0, vector.ToArray(), this.VariablesNames.Length, this.constants.Length);
        }

        protected virtual string GetTranslatedExpression()
        {
            return Util.GetTranslatedExpression(this.Agent.Chromosome.ToString(), this.constants, this.VariablesNames);
        }
    }
}