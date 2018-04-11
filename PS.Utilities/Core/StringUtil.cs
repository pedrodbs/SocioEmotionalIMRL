// ------------------------------------------
// <copyright file="StringUtil.cs" company="Pedro Sequeira">
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

//    Last updated: 02/03/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.CodeDom;
using System.IO;
using System.Text;
using Microsoft.CSharp;

namespace PS.Utilities.Core
{
    public static class StringUtil
    {
        public static string ToLiteral(this string input)
        {
            var writer = new StringWriter();
            var provider = new CSharpCodeProvider();
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
            return writer.GetStringBuilder().ToString().Replace("\"", "");
        }

        public static string Repeat(this string str, uint num)
        {
            var sb = new StringBuilder();
            for (var i = 0u; i < num; i++)
                sb.Append(str);
            return sb.ToString();
        }

        public static string Repeat(this char c, uint num)
        {
            return new string(c, (int) num);
        }

        public static bool Equals(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return false;
            s1 = StripUTF8String(s1);
            s2 = StripUTF8String(s2);

            return s1.Length == s2.Length && String.Equals(s1, s2);
        }

        private static string StripUTF8String(this string str)
        {
            var preamble = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble())[0];
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var t = str[i];
                if (!t.Equals(preamble))
                    sb.Append(t);
            }
            return sb.ToString();
        }

        public static string GetExcelColumnName(this int columnNumber)
        {
            var dividend = columnNumber + 1;
            var columnName = String.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1)%26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo)/26;
            }

            return columnName;
        }
    }
}