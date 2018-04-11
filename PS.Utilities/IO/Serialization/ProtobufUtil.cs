// ------------------------------------------
// ProtobufUtil.cs, PS.Utilities
// 
// Last updated: 2016/02/01
// 
// Pedro Sequeira
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ProtoBuf.Meta;

namespace PS.Utilities.IO.Serialization
{
    public static class ProtobufUtil
    {
        #region Static Fields & Constants

        private static int _count = 1000;
        private static readonly HashSet<Type> RegisteredTypes = new HashSet<Type>();
        private static readonly RuntimeTypeModel ProtobufModel = TypeModel.Create();

        #endregion

        #region Public methods

        public static T CloneProtoAuto<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                RegisterType(GetBaseType(typeof (T)));
                ProtobufModel.Serialize(ms, obj);
                ms.Position = 0;
                var clone = default(T);
                return (T) ProtobufModel.Deserialize(ms, clone, typeof (T));
            }
        }

        #endregion

        #region Private Methods

        private static Type GetBaseType(Type type)
        {
            var baseType = type;
            while ((baseType != null) && (baseType != typeof (object)))
            {
                type = baseType;
                baseType = baseType.BaseType;
            }
            return type;
        }

        private static IEnumerable<Type> GetDerivedTypes(Type type)
        {
            foreach (var t in Assembly.GetAssembly(type).GetTypes())
            {
                if ((t == type) || !t.IsClass || (type.IsGenericType ^ t.IsGenericType)) continue;

                var baseTypes = type.IsInterface ? t.GetInterfaces() : new[] {t.BaseType};
                var sameGenericInterface = false;
                foreach (var baseType in baseTypes)
                {
                    if (baseType == type)
                        yield return t;

                    sameGenericInterface |= baseType.Name == type.Name;
                }

                if (!type.IsGenericType || !sameGenericInterface) continue;
                yield return t.MakeGenericType(type.GenericTypeArguments);
            }
        }

        private static bool IgnoreType(Type type)
        {
            return
                type.IsPrimitive ||
                RegisteredTypes.Contains(type) ||
                type == typeof (object) ||
                RuntimeTypeModel.Default.CanSerialize(type);
        }

        private static bool RegisterType(Type type)
        {
            //verifies basic primitives and already registered types
            if (IgnoreType(type)) return false;

            RegisteredTypes.Add(type);

            //adds fields information
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var fields = fieldInfos.Select(f => f.Name).OrderBy(name => name);
            ProtobufModel.Add(type, true).Add(fields.ToArray());

            //registers fields types
            foreach (var fieldInfo in fieldInfos)
                RegisterType(fieldInfo.FieldType);

            //registers derived types
            var subTypes = GetDerivedTypes(type);
            foreach (var subType in subTypes)
                if (RegisterType(subType))
                    ProtobufModel.Add(type, true).AddSubType(_count++, subType);

            return true;
        }

        #endregion
    }
}