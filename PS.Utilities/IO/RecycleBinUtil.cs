// ------------------------------------------
// <copyright file="RecycleBinUtil.cs" company="Pedro Sequeira">
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
using System.Runtime.InteropServices;

namespace PS.Utilities.IO
{
    public class RecybleBinUtil
    {
        #region FileOperationFlags enum

        /// <summary>
        ///   Possible flags for the SHFileOperation method.
        /// </summary>
        [Flags]
        public enum FileOperationFlags : ushort
        {
            /// <summary>
            ///   Do not show a dialog during the process
            /// </summary>
            Silent = 0x0004,
            /// <summary>
            ///   Do not ask the user to confirm selection
            /// </summary>
            NoConfirmation = 0x0010,
            /// <summary>
            ///   Delete the file to the recycle bin.  (Required flag to send a file to the bin
            /// </summary>
            AllowUndo = 0x0040,
            /// <summary>
            ///   Do not show the names of the files or folders that are being recycled.
            /// </summary>
            SimpleProgress = 0x0100,
            /// <summary>
            ///   Surpress errors, if any occur during the process.
            /// </summary>
            NoErrorUI = 0x0400,
            /// <summary>
            ///   Warn if files are too big to fit in the recycle bin and will need
            ///   to be deleted completely.
            /// </summary>
            WantNukeWarning = 0x4000,
        }

        #endregion

        #region FileOperationType enum

        /// <summary>
        ///   File Operation Function Type for SHFileOperation
        /// </summary>
        public enum FileOperationType : uint
        {
            /// <summary>
            ///   Move the objects
            /// </summary>
            Move = 0x0001,
            /// <summary>
            ///   Copy the objects
            /// </summary>
            Copy = 0x0002,
            /// <summary>
            ///   Delete (or recycle) the objects
            /// </summary>
            Delete = 0x0003,
            /// <summary>
            ///   Rename the object(s)
            /// </summary>
            Rename = 0x0004,
        }

        #endregion

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref SHFileOpStruct FileOp);

        /// <summary>
        ///   Send file to recycle bin
        /// </summary>
        /// <param name = "path">Location of directory or file to recycle</param>
        /// <param name = "flags">FileOperationFlags to add in addition to FOF_ALLOWUNDO</param>
        public static bool Send(string path, FileOperationFlags flags)
        {
            try
            {
                var fs = new SHFileOpStruct
                             {
                                 wFunc = FileOperationType.Delete,
                                 pFrom = path + '\0' + '\0',
                                 fFlags = FileOperationFlags.AllowUndo | flags
                             };

                // important to double-terminate the string.
                SHFileOperation(ref fs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///   Send file to recycle bin.  Display dialog, display warning if files are too big to fit (FOF_WANTNUKEWARNING)
        /// </summary>
        /// <param name = "path">Location of directory or file to recycle</param>
        public static bool Send(string path)
        {
            return Send(path, FileOperationFlags.NoConfirmation | FileOperationFlags.WantNukeWarning);
        }

        /// <summary>
        ///   Send file silently to recycle bin.  Surpress dialog, surpress errors, delete if too large.
        /// </summary>
        /// <param name = "path">Location of directory or file to recycle</param>
        public static bool SendSilent(string path)
        {
            return Send(path,
                        FileOperationFlags.NoConfirmation | FileOperationFlags.NoErrorUI |
                        FileOperationFlags.Silent);
        }

        #region Nested type: SHFileOpStruct

        /// <summary>
        ///   SHFILEOPSTRUCT for SHFileOperation from COM
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFileOpStruct
        {
            public readonly IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)] public FileOperationType wFunc;
            public string pFrom;
            public readonly string pTo;
            public FileOperationFlags fFlags;
            [MarshalAs(UnmanagedType.Bool)] public readonly bool fAnyOperationsAborted;
            public readonly IntPtr hNameMappings;
            public readonly string lpszProgressTitle;
        }

        #endregion
    }
}