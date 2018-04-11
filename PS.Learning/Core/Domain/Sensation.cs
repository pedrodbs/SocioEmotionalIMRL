// ------------------------------------------
// <copyright file="Sensation.cs" company="Pedro Sequeira">
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

using PS.Utilities.IO.Serialization;
using System;
using System.Globalization;
using System.Xml;

namespace PS.Learning.Core.Domain
{
    public interface ISensation : IXmlSerializable
    {
        #region Public Properties

        double Reward { get; set; }

        #endregion Public Properties
    }

    [Serializable]
    public class Sensation : XmlResource, ISensation
    {
        #region Private Fields

        private const string REWARD_TAG = "reward";

        #endregion Private Fields

        #region Public Constructors

        public Sensation()
        {
        }

        public Sensation(double reward)
        {
            this.Reward = reward;
        }

        #endregion Public Constructors

        #region Public Properties

        public double Reward { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void DeserializeXML(XmlElement element)
        {
            base.DeserializeXML(element);

            this.Reward = double.Parse(element.GetAttribute(REWARD_TAG), CultureInfo.InvariantCulture);
        }

        public override void InitElements()
        {
            this.Reward = 0;
        }

        public override void SerializeXML(XmlElement element)
        {
            base.SerializeXML(element);

            element.SetAttribute(REWARD_TAG, this.Reward.ToString(CultureInfo.InvariantCulture));
        }

        #endregion Public Methods
    }
}