// ------------------------------------------
// <copyright file="PartiallyObservableState.cs" company="Pedro Sequeira">
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
using System.Collections.Generic;

namespace PS.Learning.Core.Domain.States
{
    public class PartiallyObservableState : XmlResource, IState
    {
        #region Public Constructors

        public PartiallyObservableState(ObservableState observableState, NonObservableState nonObservableState)
        {
            this.ObservablePart = observableState;
            this.NonObservablePart = nonObservableState;
            this.IdToken = this.ObservablePart.IdToken + this.NonObservablePart.IdToken;
        }

        #endregion Public Constructors

        #region Public Properties

        public uint ID { get; set; }

        public NonObservableState NonObservablePart { get; set; }

        public ObservableState ObservablePart { get; set; }

        public Dictionary<string, object> StateFeatures { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        public override void InitElements()
        {
        }

        #endregion Public Methods

        #region Public Classes

        public class NonObservableState : XmlResource, IState
        {
            #region Public Properties

            public uint ID { get; set; }
            public Dictionary<string, object> StateFeatures { get; protected set; }

            #endregion Public Properties

            #region Public Methods

            public override void InitElements()
            {
                this.StateFeatures = new Dictionary<string, object>();
            }

            #endregion Public Methods
        }

        public class ObservableState : NonObservableState
        {
        }

        #endregion Public Classes
    }
}