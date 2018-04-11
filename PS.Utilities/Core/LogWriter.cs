// ------------------------------------------
// <copyright file="LogWriter.cs" company="Pedro Sequeira">
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

//    Last updated: 2/21/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.IO;

namespace PS.Utilities.Core
{
    public class LogWriter : IDisposable
    {
        private readonly StreamWriter _streamWriter;

        public LogWriter(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            this._streamWriter = new StreamWriter(fileName) {AutoFlush = true};
            this.WriteConsole = true;
        }

        public bool WriteConsole { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            this.Close();
        }

        #endregion

        public void WriteLine(string line)
        {
            if (this._streamWriter == null) return;

            //add timestamp and prints line to file
            line = $"[{DateTime.Now}] - {line}";
            if (this.WriteConsole) Console.WriteLine(line);
            this._streamWriter.WriteLine(line);
        }

        public void Close()
        {
            this._streamWriter.Close();
            this._streamWriter.Dispose();
        }
    }
}