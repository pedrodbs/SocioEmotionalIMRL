// ------------------------------------------
// <copyright file="PerformanceMeasure.cs" company="Pedro Sequeira">
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
//    Project: Learning
//    Last updated: 12/9/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Threading;
using PS.Utilities.Core;

namespace PS.Learning.Core.Testing
{
    public class PerformanceMeasure : IResetable
    {
        #region Fields

        private long _memoryStart;
        private DateTime _timer;

        #endregion

        #region Propperties

        public long MemoryUsage { get; protected set; }

        public TimeSpan TimeElapsed { get; protected set; }

        #endregion

        #region Constructor

        public PerformanceMeasure()
        {
            this.TimeElapsed = new TimeSpan();
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            // "zero"s all measures
            this.MemoryUsage = 0;
            this.TimeElapsed = new TimeSpan();
        }

        public virtual void Start()
        {
            //starts measures (time and memory)
            this._timer = DateTime.Now;
            Thread.MemoryBarrier();
            this._memoryStart = GC.GetTotalMemory(true);
        }

        public virtual void Stop()
        {
            //stops timers and measures
            Thread.MemoryBarrier();
            var memoryEnd = GC.GetTotalMemory(true);

            this.TimeElapsed = DateTime.Now.Subtract(this._timer);
            this.MemoryUsage += memoryEnd - this._memoryStart;
        }

        #endregion
    }
}