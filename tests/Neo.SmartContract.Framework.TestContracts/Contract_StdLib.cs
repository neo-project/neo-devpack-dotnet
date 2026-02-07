// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_StdLib.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Native;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_StdLib : SmartContract
    {
        public static string base58CheckEncode(ByteString input)
        {
            return StdLib.Base58CheckEncode(input);
        }

        public static byte[] base58CheckDecode(string input)
        {
            return (byte[])StdLib.Base58CheckDecode(input);
        }

        public static byte[] base64Decode(string input)
        {
            return (byte[])StdLib.Base64Decode(input);
        }

        public static string base64Encode(byte[] input)
        {
            return StdLib.Base64Encode((ByteString)input);
        }

        public static byte[] base64UrlDecode(string input)
        {
            return (byte[])StdLib.Base64UrlDecode(input);
        }

        public static string base64UrlEncode(byte[] input)
        {
            return StdLib.Base64UrlEncode((ByteString)input);
        }

        public static byte[] base58Decode(string input)
        {
            return (byte[])StdLib.Base58Decode(input);
        }

        public static string base58Encode(byte[] input)
        {
            return StdLib.Base58Encode((ByteString)input);
        }

        public static BigInteger atoi(string value, int @base)
        {
            return StdLib.Atoi(value, @base);
        }

        public static string itoa(int value, int @base)
        {
            return StdLib.Itoa(value, @base);
        }

        public static int memoryCompare(ByteString str1, ByteString str2)
        {
            return StdLib.MemoryCompare(str1, str2);
        }

        public static int memorySearch1(ByteString mem, ByteString value)
        {
            return StdLib.MemorySearch(mem, value);
        }

        public static int memorySearch2(ByteString mem, ByteString value, int start)
        {
            return StdLib.MemorySearch(mem, value, start);
        }

        public static int memorySearch3(ByteString mem, ByteString value, int start, bool backward)
        {
            return StdLib.MemorySearch(mem, value, start, backward);
        }

        public static string[] stringSplit1(string str, string separator)
        {
            return StdLib.StringSplit(str, separator);
        }

        public static string[] stringSplit2(string str, string separator, bool removeEmptyEntries)
        {
            return StdLib.StringSplit(str, separator, removeEmptyEntries);
        }

        public static ByteString[] byteStringSplit(ByteString str, ByteString separator, bool removeEmptyEntries = false)
        {
            return StdLib.StringSplit(str, separator, removeEmptyEntries);
        }
    }
}
