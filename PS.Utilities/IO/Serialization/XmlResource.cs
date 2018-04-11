// ------------------------------------------
// <copyright file="XmlResource.cs" company="Pedro Sequeira">
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
//    Project: PS.Utilities

//    Last updated: 3/28/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace PS.Utilities.IO.Serialization
{
    /// <summary>
    ///   Class representing objects with a string identifier.
    /// </summary>
    [Serializable]
    public abstract class XmlResource : IComparable, IXmlSerializable, IDisposable
    {
        #region Private Fields

        private const string DESCRIPTION_TAG = "description";
        private const string ID_TAG = "id";

        #endregion Private Fields

        #region Protected Constructors

        protected XmlResource()
        {
        }

        /// <summary>
        ///   Creates a new XmlResource named with the given string
        /// </summary>
        /// <param name="idToken"> the object's identifier idToken </param>
        protected XmlResource(string idToken)
        {
            this.Description = "";
            this.IdToken = idToken;
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        ///   Gets or sets the object's description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///   Gets or sets the object's identifier idToken
        /// </summary>
        public string IdToken { get; set; }

        #endregion Public Properties

        #region Public Methods

        public int CompareTo(object obj)
        {
            try
            {
                return this.IdToken.CompareTo(((XmlResource)obj).IdToken);
            }
            catch (InvalidCastException)
            {
                return -1;
            }
        }

        public virtual void DeserializeXML(XmlElement element)
        {
            if (element == null) throw new ArgumentNullException("element");

            this.IdToken = element.GetAttribute(ID_TAG).ToLower();
            this.Description = element.GetAttribute(DESCRIPTION_TAG);
        }

        public virtual void Dispose()
        {
        }

        public override bool Equals(Object o)
        {
            if ((o == null) ||
                (!this.GetType().Equals(o.GetType())))
            {
                return false;
            }

            var dobj = (XmlResource)o;

            return dobj.IdToken.Equals(this.IdToken);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public abstract void InitElements();

        /// <summary>
        ///   Loads (deserializes) the resource object from the given XML file.
        /// </summary>
        /// <param name="xmlFileName"> the XML file name containing the object info. </param>
        /// <returns> the ResourceAsset loaded from the XML file. </returns>
        public virtual void LoadFromXmlFile(string xmlFileName)
        {
            this.LoadFromXmlFile(xmlFileName, this.GetType().Name);
        }

        /// <summary>
        ///   Loads (deserializes) the resource object from the given XML file.
        /// </summary>
        /// <param name="xmlFileName"> the XML file name containing the object info. </param>
        /// <param name="rootName"> the name for the root XML element. </param>
        public void LoadFromXmlFile(string xmlFileName, string rootName)
        {
            if (!File.Exists(xmlFileName)) return;

            var xd = new XmlDocument();
            try
            {
                xd.Load(xmlFileName);

                //loads root element
                var rootElement = (XmlElement)xd.SelectSingleNode(rootName);
                if (rootElement == null) return;

                this.DeserializeXML(rootElement);
            }
            catch (XmlException) //do nothing...
            {
            }
            catch (XPathException) //do nothing...
            {
            }
        }

        /// <summary>
        ///   Saves (serializes) the resource object to the given XML file.
        /// </summary>
        /// <param name="xmlFileName"> the XML file name to save the object info into. </param>
        public virtual void SaveToXmlFile(string xmlFileName)
        {
            this.SaveToXmlFile(xmlFileName, this.GetType().Name);
        }

        /// <summary>
        ///   Saves (serializes) the resource object to the given XML file.
        /// </summary>
        /// <param name="xmlFileName"> the XML file name to save the object info into. </param>
        /// <param name="rootName"> the name for the root XML element. </param>
        public void SaveToXmlFile(string xmlFileName, string rootName)
        {
            try
            {
                //creates xml document
                var xd = new XmlDocument();

                //root element
                var rootElement = xd.CreateElement(rootName);
                xd.CreateXmlDeclaration("1.0", Encoding.UTF8.EncodingName, "");
                xd.AppendChild(rootElement);

                //write contents
                this.SerializeXML(rootElement);

                //verifies file existence
                if (File.Exists(xmlFileName)) File.Delete(xmlFileName);

                //save the document to file
                var writer = new StreamWriter(xmlFileName, false, Encoding.UTF8);
                xd.Save(writer);
                writer.Close();
            }
            catch (XmlException) //do nothing...
            {
            }
            catch (IOException) //do nothing...
            {
            }
            catch (InvalidOperationException) //do nothing...
            {
            }
        }

        public virtual void SerializeXML(XmlElement element)
        {
            if (element == null) throw new ArgumentNullException("element");

            if (this.IdToken != null) element.SetAttribute(ID_TAG, this.IdToken.ToLower());
            if (this.Description != null) element.SetAttribute(DESCRIPTION_TAG, this.Description);
        }

        public override string ToString()
        {
            return "idToken - " + this.IdToken.ToUpper();
        }

        #endregion Public Methods
    }
}