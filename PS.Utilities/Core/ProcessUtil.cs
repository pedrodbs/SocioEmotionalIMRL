// ------------------------------------------
// <copyright file="ProcessUtil.cs" company="Pedro Sequeira">
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

//    Last updated: 03/28/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PS.Utilities.Core
{
    public static class ProcessUtil
    {
        public static uint GetProcessAffinity(Process process)
        {
            return (uint)process.ProcessorAffinity.ToInt32();
        }

        public static uint GetProcessAffinity()
        {
            return GetProcessAffinity(Process.GetCurrentProcess());
        }

        public static void SetMaximumProcessAffinity(Process process)
        {
            SetProcessAffinity(process, GetCPUCount());
        }

        public static void SetMaximumProcessAffinity()
        {
            SetMaximumProcessAffinity(Process.GetCurrentProcess());
        }

        public static void SetProcessAffinity(uint maxCPUsUsed)
        {
            SetProcessAffinity(Process.GetCurrentProcess(), maxCPUsUsed);
        }

        public static void SetProcessAffinity(Process process, uint maxCPUsUsed)
        {
            //sets maximum cores
            var numCPUs = System.Math.Max(1, System.Math.Min(maxCPUsUsed, GetCPUCount()));
            var affinity = 0;

            //builds affinity bit string
            for (var i = 0; i < numCPUs; i++)
                affinity += (int)System.Math.Pow(2, i);

            //sets affinity and class
            process.ProcessorAffinity = (IntPtr)affinity;
            process.PriorityClass = ProcessPriorityClass.Idle;
        }

        public static uint GetCPUCount()
        {
            return (uint)Environment.ProcessorCount;
        }

        public static void RunThreads(ThreadStart start)
        {
            //default is to run one thread per cpu available
            RunThreads(start, GetProcessAffinity());
        }

        public static void RunThreads(ThreadStart start, uint numThreads)
        {
            //creates start threads as specified
            var threads = new List<Thread>();
            for (var i = 0; i < numThreads; i++)
            {
                var thread = new Thread(start);
                threads.Add(thread);
                thread.Start();
            }

            //waits for each thread to finish
            foreach (var thread in threads)
                thread.Join();
        }
    }
}