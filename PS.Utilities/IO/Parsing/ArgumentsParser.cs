// ------------------------------------------
// <copyright file="ArgumentsParser.cs" company="Pedro Sequeira">
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
//    Project: PS.Utilities.IO

//    Last updated: 12/18/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using CommandLine;
using PS.Utilities.Collections;
using PS.Utilities.Core;
using PS.Utilities.Core.Conversion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PS.Utilities.IO.Parsing
{
    public static class ArgumentsParser
    {
        #region Public Methods

        public static string[] GetArgs(IArgumentsParsable optionsObject, bool longNameFormat = true)
        {
            //get all properties of option attribute type
            var options = ReflectionUtil.GetProperties<BaseOptionAttribute>(optionsObject);
            if (options == null) return null;

            //asks object to update before getting args
            optionsObject.OnGetArguments();

            //gets the values of such properties in the object
            var list = new List<string>();
            foreach (var option in options)
            {
                var property = (BaseOptionAttribute)option.Key;
                var propertyValue = option.Value.GetValue(optionsObject);
                list.AddRange(GetArgs(property, propertyValue, longNameFormat));
            }
            return list.ToArray();
        }

        public static bool Parse(string[] args, IArgumentsParsable optionsObject)
        {
            //parses arguments and then tries to invoke OnParsed method
            using (var parser = new Parser(settings =>
                                           {
                                               settings.CaseSensitive = false;
                                               settings.HelpWriter = Console.Out;
                                               settings.ParsingCulture = CultureInfo.InvariantCulture;
                                               settings.IgnoreUnknownArguments = true;
                                           }))
            {
                optionsObject.OnParsingArguments();
                var result = parser.ParseArguments(args, optionsObject);
                optionsObject.OnArgumentsParsed();
                return result;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static IEnumerable<string> GetArgs(
            BaseOptionAttribute property, object propertyValue, bool longNameFormat)
        {
            var argList = new List<string>();
            if (propertyValue == null) return argList;

            var argName = GetArgumentName(property, propertyValue, longNameFormat);
            if (string.IsNullOrWhiteSpace(argName)) return argList;

            argList.Add(argName);

            var argValue = GetArgumentValue(propertyValue, argList);
            argList[0] += argValue;

            return argList;
        }

        private static string GetArgs(string[] values, string argValue, List<string> argList)
        {
            if ((values == null) || (values.Length == 0)) return argValue;
            argValue = values[0];
            if (values.Length > 1)
                argList.AddRange(values.SubArray(1, values.Length - 1));
            return argValue;
        }

        private static string GetArgumentName(BaseOptionAttribute property, object propertyValue, bool longNameFormat)
        {
            var isBoolean = propertyValue is Boolean;

            //if property value is false, don't write it
            if (isBoolean && !(Boolean)propertyValue) return string.Empty;

            //if property value is true, just write name
            return longNameFormat
                ? $"--{property.LongName}{(isBoolean ? string.Empty : "=")}"
                : $"-{property.ShortName}";
        }

        private static string GetArgumentValue(object propertyValue, List<string> argList)
        {
            //if property is bool, don't write value
            if (propertyValue is Boolean) return string.Empty;

            var argValue = propertyValue.ToString();
            if (propertyValue is IValueConvertible<string[]>)
            {
                var values = ((IValueConvertible<string[]>)propertyValue).ToValue();
                argValue = GetArgs(values, argValue, argList);
            }
            else if (!(propertyValue is String) && (propertyValue is IEnumerable))
            {
                var values = ((IEnumerable)propertyValue).OfType<object>().Select(obj => obj.ToString()).ToArray();
                argValue = GetArgs(values, argValue, argList);
            }
            return argValue;
        }

        #endregion Private Methods
    }
}