// ------------------------------------------
// <copyright file="SocialArrayParameter.cs" company="Pedro Sequeira">
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
//    Project: Learning.Social

//    Last updated: 06/19/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Linq;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Utilities.Collections;

namespace PS.Learning.Social.Testing
{
    [Serializable]
    public class SocialArrayParameter : ISocialArrayParameter
    {
        private readonly IArrayParameter[] _parameters;

        public SocialArrayParameter(IArrayParameter[] parameters)
        {
            this.NumAgents = (uint) parameters.Length;
            this._parameters = (IArrayParameter[]) parameters.Clone();
        }

        public SocialArrayParameter(uint numAgents, double[] baseParameters)
            : this(numAgents, (IArrayParameter) new ArrayParameter(baseParameters))
        {
        }

        public SocialArrayParameter(uint numAgents, IArrayParameter baseParameters)
        {
            this.NumAgents = numAgents;
            this._parameters = new IArrayParameter[numAgents];
            for (var i = 0; i < numAgents; i++)
                this._parameters[i] = baseParameters;
        }

        public IArrayParameter this[uint agentIdx]
        {
            get { return this._parameters[agentIdx]; }
            set { this._parameters[agentIdx] = value; }
        }

        #region ISocialArrayParameter Members

        public uint Length
        {
            get { return this._parameters[0].Length; }
        }

        public void NormalizeVector()
        {
            foreach (var arrayParameter in this._parameters)
                arrayParameter.NormalizeVector();
        }

        public void NormalizeSum()
        {
            foreach (var arrayParameter in this._parameters)
                arrayParameter.NormalizeSum();
        }

        public void NormalizeUnitSum()
        {
            foreach (var arrayParameter in this._parameters)
                arrayParameter.NormalizeUnitSum();
        }

        public uint NumDecimalPlaces
        {
            get
            {
                return (this._parameters == null) || (this._parameters[0] == null)
                    ? 0
                    : this._parameters[0].NumDecimalPlaces;
            }
            set
            {
                foreach (var arrayParameter in this._parameters)
                    arrayParameter.NumDecimalPlaces = value;
            }
        }

        public bool Equals(ITestParameters other)
        {
            if (!(other is SocialArrayParameter)) return false;
            return this.Equals((SocialArrayParameter) other);
        }

        public void Round()
        {
            foreach (var arrayParameter in this._parameters)
                arrayParameter.Round();
        }

        ITestParameters ISocialTestParameters.this[uint agentIdx]
        {
            get { return this[agentIdx]; }
            set { this[agentIdx] = value as ArrayParameter; }
        }

        public uint NumAgents { get; private set; }

        public virtual object Clone()
        {
            return new SocialArrayParameter(this._parameters);
        }

        public string[] ToValue()
        {
            var value = new string[this.NumAgents*this.Length];
            for (var i = 0; i < this._parameters.Length; i++)
            {
                var parameterValue = this._parameters[i].ToValue();
                Array.Copy(parameterValue, 0, value, i*parameterValue.Length, parameterValue.Length);
            }
            return value;
        }

        public bool FromValue(string[] value)
        {
            if ((value == null) || (value.Length < this.NumAgents))
                return false;

            var valueSplit = value.Split(this.NumAgents);
            for (var i = 0; i < this.NumAgents; i++)
            {
                var arrayParameter = new ArrayParameter();
                if (!arrayParameter.FromValue(valueSplit[i])) return false;
                this._parameters[i] = arrayParameter;
            }
            return true;
        }

        public string[] Header
        {
            get
            {
                var header = new string[this.NumAgents*this.Length];
                for (var i = 0; i < this._parameters.Length; i++)
                {
                    var parameterValue = this._parameters[i].Header;
                    for (var j = 0; j < parameterValue.Length; j++)
                        parameterValue[j] += string.Format("Ag{0}", i);
                    Array.Copy(parameterValue, 0, header, i*parameterValue.Length, parameterValue.Length);
                }
                return header;
            }
        }

        #endregion

        public bool Equals(SocialArrayParameter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.NumAgents == other.NumAgents && this.GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return (obj is SocialArrayParameter) && this.Equals((SocialArrayParameter) obj);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static bool operator ==(SocialArrayParameter left, SocialArrayParameter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SocialArrayParameter left, SocialArrayParameter right)
        {
            return !Equals(left, right);
        }

        protected static int[] GetHashCodes(SocialArrayParameter parameters)
        {
            var hashCodes = new int[parameters.NumAgents];
            for (var i = 0u; i < parameters.NumAgents; i++)
                hashCodes[i] = parameters[i].GetHashCode();
            Array.Sort(hashCodes);
            return hashCodes;
        }

        public override string ToString()
        {
            return this._parameters.Aggregate(
                string.Empty, (current, value) => string.Format("{0}{1}|", current, value));
        }

        public string ToScreenString()
        {
            return this._parameters.Aggregate(
                string.Empty, (current, value) => string.Format("{0}{1}|", current, value.ToScreenString()));
        }
    }
}