// ------------------------------------------
// <copyright file="ColorInfo.cs" company="Pedro Sequeira">
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
//    Project: PS.Utilities.IO

//    Last updated: 12/18/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Globalization;
using System.Xml;

namespace PS.Utilities.IO.Serialization
{
    /// <summary>
    ///     Represents a color in ARGB space.
    /// </summary>
    [Serializable]
    public class ColorInfo : XmlResource
    {
        #region Protected Fields

        protected ColourValue colourValue = new ColourValue(0, 0, 0);

        #endregion Protected Fields

        #region Public Constructors

        public ColorInfo()
        {
        }

        public ColorInfo(byte r, byte g, byte b, byte a)
        {
            this.colourValue = new ColourValue(a, r, g, b);
        }

        public ColorInfo(byte r, byte g, byte b)
            : this(r, g, b, 255)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     Gets or sets the color's a component.
        /// </summary>
        public byte A
        {
            get { return this.colourValue.a; }
            set { this.colourValue.a = value; }
        }

        /// <summary>
        ///     Gets or sets the color's b component.
        /// </summary>
        public byte B
        {
            get { return this.colourValue.b; }
            set { this.colourValue.b = value; }
        }

        /// <summary>
        ///     Gets or sets the color's g component.
        /// </summary>
        public byte G
        {
            get { return this.colourValue.g; }
            set { this.colourValue.g = value; }
        }

        /// <summary>
        ///     Gets or sets the color's x component.
        /// </summary>
        public byte R
        {
            get { return this.colourValue.r; }
            set { this.colourValue.r = value; }
        }

        #endregion Public Properties

        #region Public Methods

        public override void DeserializeXML(XmlElement element)
        {
            this.R = byte.Parse(element.GetAttribute("r"), CultureInfo.InvariantCulture);
            this.G = byte.Parse(element.GetAttribute("g"), CultureInfo.InvariantCulture);
            this.B = byte.Parse(element.GetAttribute("b"), CultureInfo.InvariantCulture);
            this.A = byte.Parse(element.GetAttribute("a"), CultureInfo.InvariantCulture);
        }

        public override void InitElements()
        {
            this.colourValue = new ColourValue(0, 0, 0, 0);
        }

        public override void SerializeXML(XmlElement element)
        {
            element.SetAttribute("r", Convert.ToString(this.R, CultureInfo.InvariantCulture));
            element.SetAttribute("g", Convert.ToString(this.G, CultureInfo.InvariantCulture));
            element.SetAttribute("b", Convert.ToString(this.B, CultureInfo.InvariantCulture));
            element.SetAttribute("a", Convert.ToString(this.A, CultureInfo.InvariantCulture));
        }

        #endregion Public Methods

        #region Protected Structs

        [Serializable]
        protected struct ColourValue
        {
            #region Public Fields

            public byte a;
            public byte b;
            public byte g;
            public byte r;

            #endregion Public Fields

            #region Public Constructors

            public ColourValue(byte a, byte r, byte g, byte b)
            {
                this.a = a;
                this.b = b;
                this.r = r;
                this.g = g;
            }

            public ColourValue(byte r, byte g, byte b)
            {
                this.a = 255;
                this.b = b;
                this.r = r;
                this.g = g;
            }

            #endregion Public Constructors
        }

        #endregion Protected Structs
    }
}