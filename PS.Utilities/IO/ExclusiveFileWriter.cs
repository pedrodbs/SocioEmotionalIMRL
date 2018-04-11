// ------------------------------------------
// <copyright file="ExclusiveFileWriter.cs" company="Pedro Sequeira">
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

using MathNet.Numerics.Random;
using System;
using System.IO;
using System.Threading;

namespace PS.Utilities.IO
{
    public class ExclusiveFileWriter
    {
        #region Private Fields

        private static readonly Random Random = new WH2006(true);

        #endregion Private Fields

        #region Public Methods

        public static void AppendLine(string filePath, string line)
        {
            var fileDir = Path.GetDirectoryName(filePath);
            if ((fileDir != null) && !Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);

            var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            var lockedLength = fileStream.Length;

            while (true)
                try
                {
                    //tries to lock file exclusively
                    fileStream.Lock(0, lockedLength);
                    break;
                }
                catch (IOException)
                {
                    //sleeps a while to try again
                    Thread.Sleep(Random.Next(500, 1500));
                }

            //sets position to end of file
            fileStream.Flush();
            Thread.Sleep(1000);
            fileStream.Seek(0, SeekOrigin.End);
            //fileStream.Position = fileStream.Length;

            var streamWriter = new StreamWriter(fileStream) { AutoFlush = true };
            streamWriter.WriteLine(line);

            //unlocks and closes stream
            fileStream.Unlock(0, lockedLength);
            streamWriter.Close();
            streamWriter.Dispose();
            fileStream.Close();
            fileStream.Dispose();
        }

        #endregion Public Methods
    }
}