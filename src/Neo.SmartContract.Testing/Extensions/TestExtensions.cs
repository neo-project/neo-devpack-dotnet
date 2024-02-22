using Akka.Util;
using Neo.Cryptography.ECC;
using Neo.SmartContract.Testing.Attributes;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Neo.SmartContract.Testing.Extensions
{
    public static class TestExtensions
    {
        private static readonly Dictionary<Type, Dictionary<int, PropertyInfo>> _propertyCache = new();
        private static readonly Dictionary<Type, FieldInfo[]> _fieldCache = new();

        /// <summary>
        /// Convert Array stack item to dotnet array
        /// </summary>
        /// <param name="state">Item</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Object</returns>
        public static object?[]? ConvertTo(this VM.Types.Array state, ParameterInfo[] parameters)
        {
            if (parameters.Length > 0)
            {
                object?[] args = new object[parameters.Length];

                for (int x = 0; x < parameters.Length; x++)
                {
                    args[x] = state[x].ConvertTo(parameters[x].ParameterType);
                }

                return args;
            }

            return null;
        }

        /// <summary>
        /// Convert stack item to dotnet
        /// </summary>
        /// <param name="stackItem">Item</param>
        /// <param name="type">Type</param>
        /// <returns>Object</returns>
        public static object? ConvertTo(this StackItem stackItem, Type type)
        {
            if (stackItem is null || stackItem.IsNull) return null;

            return type switch
            {
                _ when type == typeof(object) => stackItem,
                _ when type == typeof(string) => Utility.StrictUTF8.GetString(stackItem.GetSpan()),
                _ when type == typeof(byte[]) => stackItem.GetSpan().ToArray(),

                _ when type == typeof(bool) => stackItem.GetBoolean(),
                _ when type == typeof(bool?) => stackItem.GetBoolean(),
                _ when type == typeof(byte) => (byte)stackItem.GetInteger(),
                _ when type == typeof(byte?) => (byte)stackItem.GetInteger(),
                _ when type == typeof(sbyte) => (sbyte)stackItem.GetInteger(),
                _ when type == typeof(sbyte?) => (sbyte)stackItem.GetInteger(),
                _ when type == typeof(short) => (short)stackItem.GetInteger(),
                _ when type == typeof(short?) => (short)stackItem.GetInteger(),
                _ when type == typeof(ushort) => (ushort)stackItem.GetInteger(),
                _ when type == typeof(ushort?) => (ushort)stackItem.GetInteger(),
                _ when type == typeof(int) => (int)stackItem.GetInteger(),
                _ when type == typeof(int?) => (int)stackItem.GetInteger(),
                _ when type == typeof(uint) => (uint)stackItem.GetInteger(),
                _ when type == typeof(uint?) => (uint)stackItem.GetInteger(),
                _ when type == typeof(long) => (long)stackItem.GetInteger(),
                _ when type == typeof(long?) => (long)stackItem.GetInteger(),
                _ when type == typeof(ulong) => (ulong)stackItem.GetInteger(),
                _ when type == typeof(ulong?) => (ulong)stackItem.GetInteger(),

                _ when type.IsEnum => Enum.ToObject(type, (int)stackItem.GetInteger()),
                _ when type == typeof(BigInteger) => stackItem.GetInteger(),
                _ when type == typeof(BigInteger?) => stackItem.GetInteger(),
                _ when type == typeof(UInt160) => new UInt160(stackItem.GetSpan().ToArray()),
                _ when type == typeof(UInt256) => new UInt256(stackItem.GetSpan().ToArray()),
                _ when type == typeof(ECPoint) => ECPoint.FromBytes(stackItem.GetSpan().ToArray(), ECCurve.Secp256r1),
                _ when typeof(IInteroperable).IsAssignableFrom(type) => CreateInteroperable(stackItem, type),
                _ when stackItem is InteropInterface it && it.GetInterface().GetType() == type => it.GetInterface(),

                _ when stackItem is VM.Types.Array ar => type switch
                {
                    _ when type == typeof(IList<object>) => new List<object>(ar.SubItems), // SubItems in StackItem type
                    _ when type == typeof(List<object>) => new List<object>(ar.SubItems), // SubItems in StackItem type
                    _ when type.IsArray => CreateTypeArray(ar.SubItems, type.GetElementType()!),
                    _ when type.IsClass => CreateObject(ar.SubItems, type),
                    _ when type.IsValueType => CreateValueType(ar.SubItems, type),
                    _ => throw new FormatException($"Impossible to convert {stackItem} to {type}"),
                },
                _ when stackItem is Map mp => type switch
                {
                    _ when type == typeof(IDictionary<object, object>) => ToDictionary(mp), // SubItems in StackItem type
                    _ when type == typeof(Dictionary<object, object>) => ToDictionary(mp), // SubItems in StackItem type
                    _ => throw new FormatException($"Impossible to convert {stackItem} to {type}"),
                },

                _ => throw new FormatException($"Impossible to convert {stackItem} to {type}"),
            };
        }

        private static object CreateObject(IEnumerable<StackItem> subItems, Type type)
        {
            var index = 0;
            var obj = Activator.CreateInstance(type) ?? throw new FormatException($"Impossible create {type}");

            // Cache the object properties by offset

            if (!_propertyCache.TryGetValue(type, out var cache))
            {
                cache = new Dictionary<int, PropertyInfo>();

                foreach (var property in type.GetProperties())
                {
                    var fieldOffset = property.GetCustomAttribute<FieldOrderAttribute>();
                    if (fieldOffset is null) continue;
                    if (!property.CanWrite) continue;

                    cache.Add(fieldOffset.Order, property);
                }

                if (cache.Count == 0)
                {
                    // Without FieldOrderAttribute, by order

                    foreach (var property in type.GetProperties())
                    {
                        if (!property.CanWrite) continue;
                        cache.Add(index, property);
                        index++;
                    }
                    index = 0;
                }

                _propertyCache[type] = cache;
            }

            // Fill the object

            foreach (var item in subItems)
            {
                if (cache.TryGetValue(index, out var property))
                {
                    property.SetValue(obj, ConvertTo(item, property.PropertyType));
                }
                else
                {
                    throw new FormatException($"Error converting {type}, the property with the offset {index} was not found.");
                }

                index++;
            }

            return obj;
        }

        private static IDictionary<object, object> ToDictionary(Map map)
        {
            Dictionary<object, object> dictionary = new();

            foreach (var entry in map)
            {
                dictionary.Add(entry.Key, entry.Value);
            }

            return dictionary;
        }

        private static object CreateValueType(IEnumerable<StackItem> objects, Type valueType)
        {
            var arr = objects.ToArray();
            var value = Activator.CreateInstance(valueType);

            // Cache the object properties by offset

            if (!_fieldCache.TryGetValue(valueType, out var cache))
            {
                cache = valueType.GetFields().ToArray();
                _fieldCache[valueType] = cache;
            }

            if (cache.Length != arr.Length)
            {
                throw new FormatException($"Error converting {valueType}, field count doesn't match.");
            }

            for (int x = 0; x < arr.Length; x++)
            {
                cache[x].SetValue(value, ConvertTo(arr[x], cache[x].FieldType));
            }

            return value;
        }

        private static object CreateTypeArray(IEnumerable<StackItem> objects, Type elementType)
        {
            var obj = objects.ToArray();

            if (elementType != typeof(object))
            {
                var arr = System.Array.CreateInstance(elementType, obj.Length);

                for (int x = 0; x < arr.Length; x++)
                {
                    arr.SetValue(ConvertTo(obj[x], elementType), x);
                }

                return arr;
            }

            return obj;
        }

        private static object CreateInteroperable(StackItem stackItem, Type type)
        {
            var interoperable = (IInteroperable)Activator.CreateInstance(type)!;
            interoperable.FromStackItem(stackItem);
            return interoperable;
        }
    }
}
