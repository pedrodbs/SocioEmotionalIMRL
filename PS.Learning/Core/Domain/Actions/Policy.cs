// ------------------------------------------
// <copyright file="Policy.cs" company="Pedro Sequeira">
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

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Random;
using PS.Utilities.Collections;
using System;
using System.Linq;

namespace PS.Learning.Core.Domain.Actions
{
    public class Policy : IPolicy
    {
        #region Protected Fields

        protected static readonly Random Random = new WH2006();

        #endregion Protected Fields

        #region Private Fields

        private readonly DenseVector _distribution;

        #endregion Private Fields

        #region Public Constructors

        public Policy(uint numActions, bool random)
        {
            this._distribution = new DenseVector((int)numActions);

            //just initialize a random or uniform policy
            for (var i = 0; i < this._distribution.Count; i++)
                this._distribution[i] = random ? Random.NextDouble() : 1;

            //normalize distribution
            this.Normalize();
        }

        public Policy(double[] distribution)
        {
            this._distribution = distribution;
        }

        #endregion Public Constructors

        #region Public Properties

        public uint NumActions
        {
            get { return (uint)this._distribution.Count; }
        }

        #endregion Public Properties

        #region Public Indexers

        public double this[uint actionIdx]
        {
            get { return this._distribution[(int)actionIdx]; }
            set { this._distribution[(int)actionIdx] = value; }
        }

        #endregion Public Indexers

        #region Public Methods

        public static implicit operator DenseVector(Policy policy)
        {
            return policy._distribution;
        }

        public static implicit operator Policy(DenseVector vector)
        {
            return new Policy(vector.ToArray());
        }

        public double GetActionProbability(uint actionID)
        {
            return this.NumActions > actionID ? this._distribution[(int)actionID] : 0f;
        }

        public void Normalize()
        {
            var min = this._distribution.Min();

            //verifies if some element is below 0, in which case all elements are shifted up
            if (min < 0)
                this.Add(-min);

            var sum = this._distribution.Sum();
            if (sum.Equals(1))
                //already normalized
                return;
            if (sum.Equals(0))
                //if all elements are zero, transform into uniform distribution
                this.Add(1d / this.NumActions);
            else
                //otherwise, normalize based on sum
                this.Multiply(1d / sum);
        }

        public override string ToString()
        {
            return this._distribution.Values.ToVectorString();
        }

        #endregion Public Methods

        #region Protected Methods

        protected void Add(double scalar)
        {
            for (var i = 0; i < this.NumActions; i++)
                this._distribution[i] += scalar;
        }

        protected void Multiply(double scalar)
        {
            for (var i = 0; i < this.NumActions; i++)
                this._distribution[i] *= scalar;
        }

        #endregion Protected Methods
    }
}