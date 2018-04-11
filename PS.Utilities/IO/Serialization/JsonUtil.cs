// ------------------------------------------
// <copyright file="JsonUtil.cs" company="Pedro Sequeira">
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
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using PS.Utilities.Core;

namespace PS.Utilities.IO.Serialization
{
    public static class JsonUtil
    {
        #region Public Fields

        public const BindingFlags ALL_MEMBERS =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public const BindingFlags DEFAULT_MEMBERS = BindingFlags.Instance | BindingFlags.Public;

        public static JsonSerializerSettings ArgsSettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new ArgsResolver()
            };

        public static JsonSerializerSettings CloneSettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CloneResolver(ALL_MEMBERS)
            };

        public static JsonSerializerSettings ConfigSettings =
            new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        public static JsonSerializerSettings PreserveReferencesSettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };

        public static JsonSerializerSettings TypeSpecifySettings =
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

        #endregion Public Fields

        #region Public Methods

        public static T CloneJson<T>(this T obj)
        {
            var objStr = SerializeJson(obj, CloneSettings, Formatting.None, true);
            return DeserializeJson<T>(objStr, CloneSettings, true);
        }

        public static JsonSerializer CreateJsonSerializer(JsonSerializerSettings settings)
        {
            var jsonSerializer = new JsonSerializer();
            if (settings == null) return jsonSerializer;
            jsonSerializer.TypeNameHandling = settings.TypeNameHandling;
            jsonSerializer.TypeNameAssemblyFormatHandling = settings.TypeNameAssemblyFormatHandling;
            jsonSerializer.ReferenceLoopHandling = settings.ReferenceLoopHandling;
            jsonSerializer.PreserveReferencesHandling = settings.PreserveReferencesHandling;
            jsonSerializer.ContractResolver = settings.ContractResolver;
            return jsonSerializer;
        }

        public static T DeserializeJson<T>(
            string objStr, JsonSerializerSettings settings = null, bool nested = false)
        {
            using (var stringReader = new StringReader(objStr))
                return DeserializeJson<T>(new JsonTextReader(stringReader), settings, nested);
        }

        public static T DeserializeJsonFile<T>(
            string fileName, JsonSerializerSettings settings = null, bool nested = false, bool binary = false)
        {
            if (!File.Exists(fileName)) return default(T);
            if (binary)
                using (var reader = new BinaryReader(File.OpenRead(fileName)))
                    return DeserializeJson<T>(new BsonReader(reader), settings, nested);

            using (var streamReader = new StreamReader(fileName))
                return DeserializeJson<T>(new JsonTextReader(streamReader), settings, nested);
        }

        public static void SaveToXmlFile<T>(string filePath, T obj)
        {
            var jsonSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var xmlDocument = JsonConvert.DeserializeXmlNode(
                JsonConvert.SerializeObject(new NestedObject<T>(obj), Formatting.Indented, jsonSettings));
            xmlDocument.Save(filePath);
        }

        public static string SerializeJson<T>(
            this T obj, JsonSerializerSettings settings = null, Formatting formatting = Formatting.Indented,
            bool nest = false)
        {
            using (var stringWriter = new StringWriter())
            {
                SerializeJson(obj, new JsonTextWriter(stringWriter) { Formatting = formatting }, settings, nest);
                return stringWriter.ToString();
            }
        }

        public static void SerializeJsonFile<T>(
            this T obj, string fileName, JsonSerializerSettings settings = null,
            Formatting formatting = Formatting.Indented, bool nest = false, bool binary = false)
        {
            if (!PathUtil.VerifyFilePathCreation(fileName)) return;

            if (binary)
                using (var writer = new BinaryWriter(File.OpenWrite(fileName)))
                    SerializeJson(obj, new BsonWriter(writer), settings, nest);
            else
                using (var sw = new StreamWriter(fileName))
                    SerializeJson(obj, new JsonTextWriter(sw) { Formatting = formatting }, settings, nest);
        }

        #endregion Public Methods

        #region Private Methods

        private static T DeserializeJson<T>(
            JsonReader reader, JsonSerializerSettings settings = null, bool nested = false)
        {
            var objectType = nested ? typeof(NestedObject<T>) : typeof(T);
            var serializer = CreateJsonSerializer(settings);
            var obj = serializer.Deserialize(reader, objectType);
            return nested ? ((NestedObject<T>)obj).Value : (T)obj;
        }

        private static void SerializeJson<T>(
            this T obj, JsonWriter writer, JsonSerializerSettings settings = null, bool nest = false)
        {
            var jsonSerializer = CreateJsonSerializer(settings);
            jsonSerializer.Serialize(writer, nest ? (object)new NestedObject<T>(obj) : obj);
        }

        #endregion Private Methods

        #region Private Classes

        private sealed class ArgsResolver : DefaultContractResolver
        {
            #region Protected Methods

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                //only properties with an "arg option" attribute are (de)serialized
                var attribute = Attribute.GetCustomAttribute(member, typeof(BaseOptionAttribute), true);
                return attribute != null ? base.CreateProperty(member, memberSerialization) : null;
            }

            #endregion Protected Methods
        }

        private sealed class CloneResolver : DefaultContractResolver
        {
            #region Private Fields

            private readonly BindingFlags _bindingFlags;

            #endregion Private Fields

            #region Public Constructors

            public CloneResolver(BindingFlags bindingFlags = DEFAULT_MEMBERS)
            {
                this._bindingFlags = bindingFlags;
            }

            #endregion Public Constructors

            #region Protected Methods

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = ReflectionUtil.GetProperties(type).Where(p => p.CanWrite)
                    .Select(p => this.CreateProperty(p, memberSerialization))
                    .Union(ReflectionUtil.GetFields(type).Select(f => this.CreateProperty(f, memberSerialization)))
                    .ToList();

                RemoveOverridenProperties(props);

                props.ForEach(p =>
                              {
                                  p.Writable = true;
                                  p.Readable = true;
                              });
                return props;
            }

            #endregion Protected Methods

            #region Private Methods

            private static void RemoveOverridenProperties(List<JsonProperty> props)
            {
                var hash = new HashSet<string>();
                foreach (var prop in props.ToList())
                    if (hash.Contains(prop.ToString()))
                        props.Remove(prop);
                    else
                        hash.Add(prop.ToString());
            }

            #endregion Private Methods
        }

        private sealed class NestedObject<T>
        {
            #region Public Constructors

            public NestedObject(T value)
            {
                this.Value = value;
            }

            #endregion Public Constructors

            #region Public Properties

            [JsonProperty(TypeNameHandling = TypeNameHandling.All, Order = 1000000)]
            public T Value { get; }

            #endregion Public Properties
        }

        #endregion Private Classes
    }
}