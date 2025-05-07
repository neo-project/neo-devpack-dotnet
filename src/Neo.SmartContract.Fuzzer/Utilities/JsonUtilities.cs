using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Neo.SmartContract.Fuzzer.Utilities
{
    /// <summary>
    /// Provides utility methods for converting between Neo VM StackItems and JSON.
    /// </summary>
    public static class JsonUtilities
    {
        /// <summary>
        /// Converts a StackItem to a JSON representation.
        /// </summary>
        /// <param name="item">The StackItem to convert.</param>
        /// <returns>A JsonNode representing the StackItem.</returns>
        public static JsonNode ConvertStackItemToJson(StackItem item)
        {
            if (item == null || item is Null)
                return null;

            if (item is VM.Types.Boolean b)
                return JsonValue.Create(b.GetBoolean());

            if (item is Integer i)
                return JsonValue.Create(i.GetInteger().ToString());

            if (item is ByteString bs)
                return JsonValue.Create(Convert.ToBase64String(bs.GetSpan().ToArray()));

            if (item is VM.Types.Buffer buffer)
                return JsonValue.Create(Convert.ToBase64String(buffer.GetSpan().ToArray()));

            if (item is VM.Types.Array array)
            {
                var jsonArray = new JsonArray();
                foreach (var element in array)
                {
                    jsonArray.Add(ConvertStackItemToJson(element));
                }
                return jsonArray;
            }

            if (item is Map map)
            {
                var jsonObject = new JsonObject();
                foreach (var pair in map)
                {
                    string key;
                    if (pair.Key is ByteString bs2)
                    {
                        key = System.Text.Encoding.UTF8.GetString(bs2.GetSpan().ToArray());
                    }
                    else
                    {
                        key = pair.Key.ToString();
                    }
                    jsonObject.Add(key, ConvertStackItemToJson(pair.Value));
                }
                return jsonObject;
            }

            if (item is Struct s)
            {
                var structArray = new JsonArray();
                foreach (var element in s)
                {
                    structArray.Add(ConvertStackItemToJson(element));
                }
                return structArray;
            }

            if (item is InteropInterface)
                return JsonValue.Create("InteropInterface");

            if (item is Pointer)
                return JsonValue.Create("Pointer");

            return JsonValue.Create(item.ToString());
        }

        /// <summary>
        /// Converts a JSON element to a StackItem.
        /// </summary>
        /// <param name="element">The JSON element to convert.</param>
        /// <returns>A StackItem representing the JSON element.</returns>
        public static StackItem ConvertJsonElementToStackItem(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                    return StackItem.Null;
                case JsonValueKind.True:
                    return StackItem.True;
                case JsonValueKind.False:
                    return StackItem.False;
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out long longValue))
                    {
                        return new Integer(longValue);
                    }
                    // Convert decimal to long for Integer constructor
                    return new Integer((long)element.GetDecimal());
                case JsonValueKind.String:
                    string str = element.GetString() ?? string.Empty;
                    try
                    {
                        // Try to parse as Base64
                        byte[] bytes = Convert.FromBase64String(str);
                        return new ByteString(bytes);
                    }
                    catch
                    {
                        // If not Base64, treat as regular string
                        return new ByteString(System.Text.Encoding.UTF8.GetBytes(str));
                    }
                case JsonValueKind.Array:
                    var array = new VM.Types.Array();
                    foreach (var item in element.EnumerateArray())
                    {
                        array.Add(ConvertJsonElementToStackItem(item));
                    }
                    return array;
                case JsonValueKind.Object:
                    var map = new Map();
                    foreach (var property in element.EnumerateObject())
                    {
                        var key = new ByteString(System.Text.Encoding.UTF8.GetBytes(property.Name));
                        var value = ConvertJsonElementToStackItem(property.Value);
                        map[key] = value;
                    }
                    return map;
                default:
                    throw new ArgumentException($"Unsupported JSON value kind: {element.ValueKind}");
            }
        }

        /// <summary>
        /// Converts a JSON node to a StackItem.
        /// </summary>
        /// <param name="node">The JSON node to convert.</param>
        /// <returns>A StackItem representing the JSON node.</returns>
        public static StackItem ConvertJsonNodeToStackItem(JsonNode node)
        {
            if (node == null)
                return StackItem.Null;

            if (node is JsonValue value)
            {
                if (value.TryGetValue<bool>(out bool boolValue))
                {
                    return boolValue ? StackItem.True : StackItem.False;
                }
                if (value.TryGetValue<long>(out long longValue))
                {
                    return new Integer(longValue);
                }
                if (value.TryGetValue<double>(out double doubleValue))
                {
                    return new Integer((long)doubleValue);
                }
                if (value.TryGetValue<string>(out string stringValue))
                {
                    if (stringValue == null)
                        return new ByteString(System.Array.Empty<byte>());

                    try
                    {
                        // Try to parse as Base64
                        byte[] bytes = Convert.FromBase64String(stringValue);
                        return new ByteString(bytes);
                    }
                    catch
                    {
                        // If not Base64, treat as regular string
                        return new ByteString(System.Text.Encoding.UTF8.GetBytes(stringValue));
                    }
                }
                return StackItem.Null;
            }
            else if (node is JsonArray array)
            {
                var vmArray = new VM.Types.Array();
                foreach (var item in array)
                {
                    // Skip null items
                    if (item != null)
                    {
                        vmArray.Add(ConvertJsonNodeToStackItem(item));
                    }
                    else
                    {
                        vmArray.Add(StackItem.Null);
                    }
                }
                return vmArray;
            }
            else if (node is JsonObject obj)
            {
                var map = new Map();
                foreach (var property in obj)
                {
                    if (property.Key != null)
                    {
                        var key = new ByteString(System.Text.Encoding.UTF8.GetBytes(property.Key));
                        var propertyValue = ConvertJsonNodeToStackItem(property.Value);
                        map[key] = propertyValue;
                    }
                }
                return map;
            }

            return StackItem.Null;
        }
    }
}
