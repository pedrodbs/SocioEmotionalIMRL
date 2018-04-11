// ------------------------------------------
// <copyright file="ArrayParameter.cs" company="Pedro Sequeira">
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

using Newtonsoft.Json;
using PS.Utilities.Collections;
using PS.Utilities.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PS.Learning.Core.Testing.Config.Parameters
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ArrayParameter : IArrayParameter
    {
        #region Private Fields

        private static readonly Range<double> DefaultRange = new Range<double>(-1, 1);
        private string[] _customHeader;

        #endregion Private Fields

        #region Public Constructors

        public ArrayParameter() : this(new double[1])
        {
        }

        public ArrayParameter(double[] array) : this(2, array)
        {
        }

        public ArrayParameter(double[] array, Range<double>[] domains)
            : this(2, array, domains)
        {
        }

        public ArrayParameter(uint numDecimalPlaces, double[] array)
            : this(numDecimalPlaces, array, null)
        {
        }

        public ArrayParameter(IArrayParameter baseParam)
            : this(baseParam.NumDecimalPlaces, baseParam.Array, baseParam.Domains)
        {
        }

        public ArrayParameter(uint numDecimalPlaces, double[] array, Range<double>[] domains)
        {
            this.NumDecimalPlaces = numDecimalPlaces;
            this.Array = (double[])array.Clone();
            this.Domains = domains ?? this.DefaultDomains;
        }

        #endregion Public Constructors

        #region Public Properties

        public double AbsoulteSum
        {
            get { return this.Array.Sum(val => Math.Abs(val)); }
        }

        [JsonProperty]
        public double[] Array { get; protected set; }

        [JsonProperty]
        public Range<double>[] Domains { get; }

        public string[] Header
        {
            get
            {
                if (this._customHeader != null) return this._customHeader;

                var length = this.Array.Length;
                var header = new string[length];
                for (var i = 0; i < length; i++)
                    header[i] = $"param{i}";
                return header;
            }
            set { this._customHeader = value; }
        }

        public uint Length
        {
            get { return (uint)this.Array.Length; }
        }

        [JsonProperty]
        public uint NumDecimalPlaces { get; set; }

        public double Sum
        {
            get { return this.Array.Sum(); }
        }

        #endregion Public Properties

        #region Protected Properties

        protected Range<double>[] DefaultDomains
        {
            get
            {
                var domains = new Range<double>[this.Length];
                domains.Initialize(DefaultRange);
                return domains;
            }
        }

        #endregion Protected Properties

        #region Public Indexers

        public double this[int index]
        {
            get { return this.Array[index]; }
            set { this.Array[index] = value; }
        }

        #endregion Public Indexers

        #region Public Methods

        public static bool operator !=(ArrayParameter left, ArrayParameter right)
        {
            return !Equals(left, right);
        }

        public static bool operator ==(ArrayParameter left, ArrayParameter right)
        {
            return Equals(left, right);
        }

        public static implicit operator double[] (ArrayParameter arrayParam)
        {
            return arrayParam.Array;
        }

        public void CapValuesToDomains()
        {
            //caps all param values according to domains
            for (var i = 0; i < this.Length; i++)
                if (this.Array[i] < this.Domains[i].min)
                    this.Array[i] = this.Domains[i].min;
                else if (this.Array[i] > this.Domains[i].max)
                    this.Array[i] = this.Domains[i].max;
        }

        public virtual object Clone()
        {
            return new ArrayParameter((IArrayParameter)this);
        }

        public virtual bool Equals(ITestParameters other)
        {
            if (!(other is ArrayParameter)) return false;
            return this.Equals((ArrayParameter)other);
        }

        public virtual bool Equals(ArrayParameter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.Length != other.Length) return false;

            for (var i = 0; i < this.Length; i++)
                if (!this.Array[i].Equals(other.Array[i]))
                    return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            return (obj is ArrayParameter) && this.Equals((ArrayParameter)obj);
        }

        public bool FromValue(string[] value)
        {
            var array = ArrayUtil.FromStringArray<double>(value);
            if (array != null)
                this.Array = array;
            return array != null;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public IEnumerator<double> GetEnumerator()
        {
            return this.Array.ToList().GetEnumerator();
        }

        public void NormalizeSum()
        {
            var sum = this.Array.Sum();
            if (sum.Equals(0)) return;

            for (var i = 0; i < this.Length; i++)
                this.Array[i] /= sum;
        }

        public void NormalizeUnitSum()
        {
            do
            {
                //normalize elements (abs values sum)
                var sum = this.AbsoulteSum;
                for (var i = 0; i < this.Length; i++)
                    this[i] /= sum;

                //checks if elements are within domain, caps if necessary
                for (var i = 0; i < this.Length; i++)
                {
                    var curDomain = this.Domains[i];
                    if (this[i] < curDomain.min)
                        this[i] = curDomain.min;
                    else if (this[i] > curDomain.max)
                        this[i] = curDomain.max;
                }

                //checks terminal case, where values were not changed
            } while (!this.AbsoulteSum.Equals(1d));
        }

        public void NormalizeVector()
        {
            var sumSquare = this.Array.Sum(param => param * param);
            var t = Math.Sqrt(sumSquare);

            if (t.Equals(0)) return;

            for (var i = 0; i < this.Length; i++)
                this.Array[i] /= t;
        }

        public virtual void Round()
        {
            for (var i = 0; i < this.Length; i++)
                this.Array[i] = Math.Round(this.Array[i], 2);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SetMidDomainValues()
        {
            //sets default values according to mid points in each param domain
            for (var i = 0; i < this.Length; i++)
                this.Array[i] = 0.5 * (this.Domains[i].max - this.Domains[i].min);
        }

        public override string ToString()
        {
            var format = "{0}{1:#.";
            for (var i = 0; i < this.NumDecimalPlaces; i++)
                format += "#";
            format += "}_";
            return this.Array.Aggregate(
                string.Empty, (current, value) => string.Format(format, current, value));
        }

        public string[] ToValue()
        {
            return this.Array.ToStringArray();
        }

        public string ToVectorString()
        {
            var format = "{0}{1:0.";
            for (var i = 0; i < this.NumDecimalPlaces; i++)
                format += "0";
            format += "},";
            var str = this.Aggregate("[", (current, value) => string.Format(format, current, value));
            str = str.Remove(str.Length - 1, 1);
            return str + "]";
        }

        public string ToScreenString()
        {
            return this.ToVectorString();
        }

        #endregion Public Methods
    }
}