// ------------------------------------------
// <copyright file="SerializableDictionary.cs" company="Pedro Sequeira">
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

//    Last updated: 2/4/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PS.Utilities.IO.Serialization
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        #region Private Fields

        private const string KEYS_PROPERTY_NAME = "Keys";
        private const string VALUES_PROPERTY_NAME = "Values";

        [JsonProperty(KEYS_PROPERTY_NAME, IsReference = false)
        ]
        private List<TKey> _keys;

        [JsonProperty(VALUES_PROPERTY_NAME, IsReference = false)
        ]
        private List<TValue> _values;

        #endregion Private Fields

        #region Public Constructors

        public SerializableDictionary()
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Protected Constructors

        #region Private Methods

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (this._keys == null) return;
            if (this.Count == 0)
                for (var i = 0; i < this._keys.Count; i++)
                    this.Add(this._keys[i], this._values[i]);
            this._keys.Clear();
            this._values.Clear();
            this._keys = null;
            this._values = null;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            this._keys = this.Select(entry => entry.Key).ToList();
            this._values = this.Select(entry => entry.Value).ToList();
        }

        #endregion Private Methods
    }
}