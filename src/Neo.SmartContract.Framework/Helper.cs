// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using System;
using System.Numerics;
using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework
{
    public static class Helper
    {
        /// <summary>
        /// Converts byte to byte[] considering the byte as a BigInteger (0x00 at the end)
        /// </summary>
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.LEFT)]
        public extern static byte[] ToByteArray(this byte source);

        /// <summary>
        /// Converts sbyte to byte[].
        /// </summary>
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern static byte[] ToByteArray(this sbyte source);

        /// <summary>
        /// Converts string to byte[]. Examples: "hello" -> [0x68656c6c6f]; "" -> []; "Neo" -> [0x4e656f]
        /// </summary>
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern static byte[] ToByteArray(this string source);

        /// <summary>
        /// Converts byte[] to string. Examples: [0x68656c6c6f] -> "hello"; [] -> ""; [0x4e656f] -> "Neo"
        /// </summary>
        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        public extern static string ToByteString(this byte[] source);

        /// <summary>
        /// Returns true iff a <= x && x < b. Examples: x=5 a=5 b=15 is true; x=15 a=5 b=15 is false
        /// </summary>
        [OpCode(OpCode.WITHIN)]
        public extern static bool Within(this BigInteger x, BigInteger a, BigInteger b);

        /// <summary>
        /// Returns true iff a <= x && x < b. Examples: x=5 a=5 b=15 is true; x=15 a=5 b=15 is false
        /// </summary>
        [OpCode(OpCode.WITHIN)]
        public extern static bool Within(this int x, BigInteger a, BigInteger b);

        /// <summary>
        /// Converts and ensures parameter source is sbyte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP SIZE PUSH1 NUMEQUAL ASSERT
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public extern static sbyte AsSbyte(this BigInteger source);
        //{
        //    Assert(source.AsByteArray().Length == 1);
        //    return (sbyte) source;
        //}

        /// <summary>
        /// Converts and ensures parameter source is sbyte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP SIZE PUSH1 NUMEQUAL ASSERT
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public extern static sbyte AsSbyte(this int source);
        //{
        //    Assert(((BigInteger)source).AsByteArray().Length == 1);
        //    return (sbyte) source;
        //}

        /// <summary>
        /// Converts and ensures parameter source is byte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP SIZE PUSH1 NUMEQUAL ASSERT
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public extern static byte AsByte(this BigInteger source);
        //{
        //    Assert(source.AsByteArray().Length == 1);
        //    return (byte) source;
        //}

        /// <summary>
        /// Converts and ensures parameter source is byte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP SIZE PUSH1 NUMEQUAL ASSERT
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public extern static byte AsByte(this int source);
        //{
        //    Assert(((BigInteger)source).AsByteArray().Length == 1);
        //    return (byte) source;
        //}

        /// <summary>
        /// Converts parameter to sbyte from (big)integer range -128-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> -1 [0xff]; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "7f")]
        [OpCode(OpCode.JMPLE, "06")]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.SUB)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "80")]
        [OpCode(OpCode.PUSHINT16, "8000")]
        [OpCode(OpCode.WITHIN)]
        [OpCode(OpCode.ASSERT)]
        public static extern sbyte ToSbyte(this BigInteger source);

        /// <summary>
        /// Converts parameter to sbyte from (big)integer range -128-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> -1 [0xff]; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "7f")]
        [OpCode(OpCode.JMPLE, "06")]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.SUB)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "80")]
        [OpCode(OpCode.PUSHINT16, "8000")]
        [OpCode(OpCode.WITHIN)]
        [OpCode(OpCode.ASSERT)]
        public static extern sbyte ToSbyte(this int source);

        /// <summary>
        /// Converts parameter to byte from (big)integer range 0-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> fault; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.WITHIN)]
        [OpCode(OpCode.ASSERT)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "7f")]
        [OpCode(OpCode.JMPLE, "06")]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.SUB)]
        public static extern byte ToByte(this BigInteger source);

        /// <summary>
        /// Converts parameter to byte from (big)integer range 0-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> fault; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.WITHIN)]
        [OpCode(OpCode.ASSERT)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSHINT8, "7f")]
        [OpCode(OpCode.JMPLE, "06")]
        [OpCode(OpCode.PUSHINT16, "0001")]
        [OpCode(OpCode.SUB)]
        public static extern byte ToByte(this int source);

        [OpCode(OpCode.CAT)]
        public extern static byte[] Concat(this byte[] first, byte[] second);

        [OpCode(OpCode.CAT)]
        public extern static byte[] Concat(this byte[] first, ByteString second);

        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        public extern static ByteString Concat(this ByteString first, ByteString second);

        [OpCode(OpCode.SUBSTR)]
        public extern static byte[] Range(this byte[] source, int index, int count);

        /// <summary>
        /// Returns byte[] with first 'count' elements from 'source'. Faults if count < 0
        /// </summary>
        [OpCode(OpCode.LEFT)]
        public extern static byte[] Take(this byte[] source, int count);

        /// <summary>
        /// Returns string with first 'count' elements from string 'source'. Faults if count < 0
        /// </summary>
        /// <param name="source">The original string</param>
        /// <param name="count">the number of elements to return</param>
        /// <returns>The first 'count' elements</returns>
        /// <example>"Neo, 2" -> "Ne"</example>
        public static string Take(this string source, int count) => source[..count];

        /// <summary>
        /// Returns byte[] with last 'count' elements from 'source'. Faults if count < 0
        /// </summary>
        [OpCode(OpCode.RIGHT)]
        public extern static byte[] Last(this byte[] source, int count);

        /// <summary>
        /// Returns string with last 'count' elements from string 'source'. Faults if count < 0
        /// </summary>
        /// <param name="source">The original string</param>
        /// <param name="count">the number of elements to return</param>
        /// <returns>The last 'count' elements</returns>
        /// <example>"Neo, 2" -> "eo"</example>
        public static string Last(this string source, int count) => source[^count..];

        /// <summary>
        /// Returns a reversed copy of parameter 'source'.
        /// Example: [0a,0b,0c,0d,0e] -> [0e,0d,0c,0b,0a]
        /// </summary>
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.REVERSEITEMS)]
        public extern static byte[] Reverse(this Array source);

        /// <summary>
        /// Return a reversed copy of string parameter 'source'.
        /// </summary>
        /// <param name="source">The original string</param>
        /// <returns>The reversed string</returns>
        /// <example>NEO -> OEN</example>
        public static string Reverse(this string source) => source.ToByteArray().Reverse().ToByteString();

        /// <summary>
        /// Replace the character in the string with the specified character.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="oldChar">The old character to be replaced</param>
        /// <param name="newChar">The new character to replace</param>
        /// <returns>The new string with character replaced</returns>
        /// <example>Neb,b,o -> Neo</example>
        public static string Replace(this string source, byte oldChar, byte newChar)
        {
            var byteArray = source.ToByteArray();
            for (var i = 0; i < byteArray.Length; i++)
            {
                if (byteArray[i] == oldChar)
                {
                    byteArray[i] = newChar;
                }
            }
            return byteArray.ToByteString();
        }

        public static string Replace(this string source, string oldStr, string newStr)
        {
            var sourceBytes = source.ToByteArray();
            var oldBytes = oldStr.ToByteArray();
            var newBytes = newStr.ToByteArray();
            var result = Array.Empty<byte>();
            for (var i = 0; i < sourceBytes.Length; i++)
            {
                if (sourceBytes[i] == oldBytes[0])
                {
                    var match = true;
                    for (var j = 0; j < oldBytes.Length; j++)
                    {
                        if (sourceBytes[i + j] != oldBytes[j])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        result = result.Concat(newBytes);
                        i += oldBytes.Length - 1;
                        continue;
                    }
                }
                result = result.Concat(sourceBytes[i].ToByteArray());
            }
            return result.ToByteString();
        }


        /// <summary>
        ///  Returns true iff 'source' contains 'value'.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="value">The string to find</param>
        /// <returns>bool result of the check, true if end with</returns>
        public static bool EndsWith(this string source, string value)
        {
            var sourceBytes = source.ToByteArray();
            var valueBytes = value.ToByteArray();
            if (sourceBytes.Length < valueBytes.Length)
            {
                return false;
            }
            for (var i = 0; i < valueBytes.Length; i++)
            {
                if (sourceBytes[sourceBytes.Length - valueBytes.Length + i] != valueBytes[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  Returns the first index of the value iff 'source' contains 'value'.
        /// </summary>
        /// <param name="source">The string to search</param>
        /// <param name="value">The target character to search</param>
        /// <returns>The index of the first found value, -1 if not found</returns>
        public static int IndexOf(this string source, byte value)
        {
            var sourceBytes = source.ToByteArray();
            for (var i = 0; i < sourceBytes.Length; i++)
            {
                if (sourceBytes[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// Converts the string to uppercase
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUpper(this string source)
        {
            var sourceBytes = source.ToByteArray();
            for (var i = 0; i < sourceBytes.Length; i++)
            {
                if (sourceBytes[i] >= 0x61 && sourceBytes[i] <= 0x7a)
                {
                    sourceBytes[i] = (byte)(sourceBytes[i] - 0x20);
                }
            }
            return sourceBytes.ToByteString();
        }

        /// <summary>
        /// Converts the string to lowercase
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToLower(this string source)
        {
            var sourceBytes = source.ToByteArray();
            for (var i = 0; i < sourceBytes.Length; i++)
            {
                if (sourceBytes[i] >= 0x41 && sourceBytes[i] <= 0x5a)
                {
                    sourceBytes[i] = (byte)(sourceBytes[i] + 0x20);
                }
            }
            return sourceBytes.ToByteString();
        }


        /// <summary>
        /// Returns the square root of number x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [OpCode(OpCode.SQRT)]
        public extern static BigInteger Sqrt(this BigInteger x);

        [OpCode(OpCode.MODMUL)]
        public extern static BigInteger ModMultiply(this BigInteger x, BigInteger y, BigInteger modulus);

        public static BigInteger ModInverse(this BigInteger value, BigInteger modulus)
        {
            return BigInteger.ModPow(value, -1, modulus);
        }
    }
}
