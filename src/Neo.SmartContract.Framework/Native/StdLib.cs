// Copyright (C) 2015-2025 The Neo Project.
//
// StdLib.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;
using System.Numerics;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0")]
    public static class StdLib
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        /// <summary>
        /// Serializes an object to bytes with neo binary serilization format.
        /// The 'source' can be null, and if object is null, it will be treated as null item.
        /// <para>
        /// The execution will fail if:
        ///  1. the 'source object has cycle reference(for example, a list contains itself).
        ///  2. The serialized size exceeds the item size limit.
        /// </para>
        /// </summary>
        public extern static ByteString Serialize(object source);

        /// <summary>
        /// Deserializes an object from bytes with neo binary serilization format.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'source' is null.
        ///  2. The 'source' is not a valid neo binary serilization format.
        ///  3. The deserialized size exceeds the item size limit.
        /// </para>
        /// </summary>
        public extern static object Deserialize(ByteString source);

        /// <summary>
        /// Serializes an object to a JSON string.
        /// The object can be null, and if object is null, the result will be "null".
        /// <para>
        /// The execution will fail if:
        ///  1. the 'source object has cycle reference(for example, a list contains itself).
        ///  2. The serialized size exceeds the item size limit.
        /// </para>
        /// </summary>
        public extern static string JsonSerialize(object? obj);

        /// <summary>
        /// Deserializes an object from a JSON string.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'json' is null.
        ///  2. The 'json' is not a valid JSON string.
        ///  3. The deserialized size exceeds the item size limit.
        ///  4. The max nested depth exceeds the limit(The default value is 10).
        /// </para>
        /// </summary>
        public extern static object JsonDeserialize(string json);

        /// <summary>
        /// Decodes a base64-std encoded string to a byte-string.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'input' is null.
        ///  2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        ///  2. The 'input' is not a valid base64-encoded string.
        /// </para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static extern ByteString Base64Decode(string input);

        /// <summary>
        /// Encodes a byte-string/bytes to a base64std encoded string.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'input' is null.
        ///  2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern string Base64Encode(ByteString input);


        /// <summary>
        /// Decodes a base64Url-encoded string to a byte-string.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'input' is null.
        ///  2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        ///  3. The 'input' is not a valid base64Url-encoded string.
        /// </para>
        /// </summary>
        public static extern ByteString Base64UrlDecode(string input);

        /// <summary>
        /// Encodes a byte-string/bytes to a base64Url-encoded string.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'input' is null.
        ///  2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern string Base64UrlEncode(ByteString input);

        /// <summary>
        /// Encodes a byte-string/bytes to a base64-encoded string with checksum.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'input' is null.
        ///  2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        ///  3. The 'input' is not a valid base58-encoded string.
        /// </para>
        /// </summary>
        public static extern ByteString Base58Decode(string input);

        /// <summary>
        /// Encodes a byte-string/bytes to a base58-encoded string.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'input' is null.
        ///  2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern string Base58Encode(ByteString input);

        /// <summary>
        /// Encodes a byte-string/bytes to a base58-encoded string with checksum.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'input' is null.
        ///  2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern string Base58CheckEncode(ByteString input);

        /// <summary>
        /// Decodes a base58-encoded string with checksum to a byte-string.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'input' is null.
        ///  2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        ///  3. The 'input' is not a valid base58-encoded string with checksum.
        /// </para>
        /// </summary>
        public static extern ByteString Base58CheckDecode(string input);

        /// <summary>
        /// Encodes a byte-string/bytes to its equivalent lower case hexadecimal string.
        /// Available since HF_Faun.
        /// <para>
        /// The execution will fail:
        ///   1. The 'input' is null.
        ///   2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern string HexEncode(ByteString input);

        /// <summary>
        /// Decodes a case-insensitive hexadecimal string to a byte-string/bytes.
        /// Available since HF_Faun.
        /// <para>
        /// The execution will fail:
        ///  1. The 'input' is null.
        ///  2. The 'input.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        ///  3. The 'input' is not a valid hexadecimal string.
        /// </para>
        /// </summary>
        public static extern ByteString HexDecode(string input);


        /// <summary>
        /// Encodes a BigInteger to a string with specified base.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        /// </para>
        /// </summary>
        public static extern string Itoa(BigInteger value, int @base = 10);

        /// <summary>
        /// Encodes an int to a string with specified base.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        /// </para>
        /// </summary>
        public static extern string Itoa(int value, int @base = 10);

        /// <summary>
        /// Encodes an uint to a string with specified base.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        /// </para>
        /// </summary>
        public static extern string Itoa(uint value, int @base = 10);

        /// <summary>
        /// Encodes a long to a string with specified base.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        /// </para>
        /// </summary>
        public static extern string Itoa(long value, int @base = 10);

        /// <summary>
        /// Encodes an ulong to a string with specified base.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        /// </para>
        /// </summary>
        public static extern string Itoa(ulong value, int @base = 10);

        /// <summary>
        /// Encodes a short to a string with specified base.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        /// </para>
        /// </summary>
        public static extern string Itoa(short value, int @base = 10);

        /// <summary>
        /// Encodes an ushort to a string.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        /// </para>
        /// </summary>
        public static extern string Itoa(ushort value, int @base = 10);

        /// <summary>
        /// Encodes a byte to a string with specified base.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        /// </para>
        /// </summary>
        public static extern string Itoa(byte value, int @base = 10);

        /// <summary>
        /// Encodes a sbyte to a string with specified base.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        /// </para>
        /// </summary>
        public static extern string Itoa(sbyte value, int @base = 10);

        /// <summary>
        /// Decodes a string to a BigInteger with specified base.
        /// The value cannot start with '0x' or '0X' even if base is 16.
        /// <para>
        /// The execution will fail:
        ///  1. The 'value' is null.
        ///  2. The '@base' is not a valid base(only support 10 and 16).
        ///  3. The 'value.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        ///  4. The value is not a valid integer string with '@base'
        /// </para>
        /// </summary>
        public static extern BigInteger Atoi(string value, int @base = 10);

        /// <summary>
        ///  Compare two ByteString values. The result is
        ///  0 if the values are equal,
        ///  -1 if the first value is less than the second value,
        ///  1 if the first value is greater than the second value.
        /// <para>
        /// The execution will fail:
        ///  1. The 'str1' or 'str2' is null.
        ///  2. The 'str1.length' or 'str2.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern int MemoryCompare(ByteString str1, ByteString str2);

        /// <summary>
        /// Search for the first occurrence of a value in a byte-string. -1 if the 'value' not found
        /// <para>
        /// The execution will fail:
        ///  1. The 'mem' or 'value' is null.
        ///  2. The 'mem.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern int MemorySearch(ByteString mem, ByteString value);

        /// <summary>
        /// Search for the first occurrence of a value in a byte-string starting from a given index. -1 if the 'value' not found
        /// <para>
        /// The execution will fail:
        ///  1. The 'mem' or 'value' is null.
        ///  2. The 'mem.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern int MemorySearch(ByteString mem, ByteString value, int start);

        /// <summary>
        /// Search for the first occurrence of a value in a byte-string starting from a given index and searching backward. -1 if the 'value' not found
        /// <para>
        /// The execution will fail:
        ///  1. The 'mem' or 'value' is null.
        ///  2. The 'mem.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern int MemorySearch(ByteString mem, ByteString value, int start, bool backward);

        /// <summary>
        /// Split a string into an array of substrings based on a separator.
        /// <para>
        /// The execution will fail:
        ///  1. The 'str' or 'separator' is null.
        ///  2. The 'str.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern string[] StringSplit(string str, string separator);

        /// <summary>
        /// Split a string into an array of substrings based on a separator.
        /// If 'removeEmptyEntries' is true, empty entries will be removed from the result.
        /// <para>
        /// The execution will fail:
        ///  1. The 'str' or 'separator' is null.
        ///  2. The 'str.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        public static extern string[] StringSplit(string str, string separator, bool removeEmptyEntries);

        /// <summary>
        /// Get the character count of the string.
        /// <para>
        /// The execution will fail:
        ///  1. The 'str' is null.
        ///  2. The 'str.length' exceeds the MaxInputLength limits(the default value is 1024-byte).
        /// </para>
        /// </summary>
        /// <param name="str">The string to get the character count of.</param>
        /// <example>
        ///        string a = "A"; // return 1
        ///        string tilde = "ã"; // return 1
        ///        string duck = "🦆"; //return 1
        /// </example>
        public static extern int StrLen(string str);
    }
}
