// ------------------------------------------
// <copyright file="Action.cs" company="Pedro Sequeira">
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

using PS.Learning.Core.Domain.Agents;
using PS.Utilities.IO.Serialization;

namespace PS.Learning.Core.Domain.Actions
{
    public class Action : XmlResource, IAction
    {
        #region Public Constructors

        public Action(string id, IAgent agent)
        {
            this.Agent = agent;
            this.IdToken = id;
        }

        #endregion Public Constructors

        #region Public Properties

        public IAgent Agent { get; }
        public uint ID { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override bool Equals(object obj)
        {
            return
                ReferenceEquals(obj, this) ||
                ((obj is Action) && obj.ToString().Equals(this.ToString()));
        }

        public virtual double Execute()
        {
            return 0;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override void InitElements()
        {
        }

        public override string ToString()
        {
            return this.IdToken;
        }

        #endregion Public Methods
    }
}