// ------------------------------------------
// <copyright file="SocialCommonTestParameters.cs" company="Pedro Sequeira">
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

//    Last updated: 01/27/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using Newtonsoft.Json;
using PS.Learning.Core.Testing.Config.Parameters;

namespace PS.Learning.Social.Testing
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class SocialCommonTestParameters : ISocialTestParameters
    {
        [JsonConstructor]
        public SocialCommonTestParameters(uint numAgents, ITestParameters commomTestParameters)
        {
            this.NumAgents = numAgents;
            this.CommonTestParameters = commomTestParameters;
        }

        public SocialCommonTestParameters(SocialCommonTestParameters baseParams) :
            this(baseParams.NumAgents, (ITestParameters) baseParams.CommonTestParameters.Clone())
        {
        }

        #region ISocialTestParameters Members

        #region Implementation of IEquatable<ITestParameters>

        public bool Equals(ITestParameters other)
        {
            if (!(other is SocialCommonTestParameters)) return false;
            var otherSocialParams = (SocialCommonTestParameters) other;
            return otherSocialParams.NumAgents.Equals(this.NumAgents) &&
                   otherSocialParams.CommonTestParameters.Equals(this.CommonTestParameters);
        }

        #endregion

        public object Clone()
        {
            return new SocialCommonTestParameters(this);
        }

        public string[] ToValue()
        {
            return this.CommonTestParameters.ToValue();
        }

        public bool FromValue(string[] value)
        {
            return this.CommonTestParameters.FromValue(value);
        }

        public string[] Header
        {
            get { return this.CommonTestParameters.Header; }
        }

        public string ToScreenString()
        {
            return this.CommonTestParameters.ToScreenString();
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is SocialCommonTestParameters)) return false;
            return this.Equals((SocialCommonTestParameters) obj);
        }

        public override int GetHashCode()
        {
            return this.CommonTestParameters.GetHashCode();
        }

        public override string ToString()
        {
            return this.CommonTestParameters.ToString();
        }

        #region Implementation of ISocialTestParameters

        [JsonProperty]
        public ITestParameters CommonTestParameters { get; private set; }

        public ITestParameters this[uint agentIdx]
        {
            get { return this.CommonTestParameters; }
            set { this.CommonTestParameters = value; }
        }

        [JsonProperty]
        public uint NumAgents { get; private set; }


        #endregion
    }
}