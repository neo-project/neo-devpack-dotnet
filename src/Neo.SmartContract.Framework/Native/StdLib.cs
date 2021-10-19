// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
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
        public static extern UInt160 Hash { [ContractHash] get; }

        public extern static ByteString Serialize(object source);

        public extern static object Deserialize(ByteString source);

        public extern static string JsonSerialize(object obj);

        public extern static object JsonDeserialize(string json);

        public static extern ByteString Base64Decode(string input);

        public static extern string Base64Encode(ByteString input);

        public static extern ByteString Base58Decode(string input);

        public static extern string Base58Encode(ByteString input);

        public static extern string Base58CheckEncode(ByteString input);

        public static extern ByteString Base58CheckDecode(string input);

        public static extern string Itoa(BigInteger value, int @base = 10);

        public static extern string Itoa(int value, int @base = 10);

        public static extern string Itoa(uint value, int @base = 10);

        public static extern string Itoa(long value, int @base = 10);

        public static extern string Itoa(ulong value, int @base = 10);

        public static extern string Itoa(short value, int @base = 10);

        public static extern string Itoa(ushort value, int @base = 10);

        public static extern string Itoa(byte value, int @base = 10);

        public static extern string Itoa(sbyte value, int @base = 10);

        public static extern BigInteger Atoi(string value, int @base = 10);

        public static extern int MemoryCompare(ByteString str1, ByteString str2);

        public static extern int MemorySearch(ByteString mem, ByteString value);

        public static extern int MemorySearch(ByteString mem, ByteString value, int start);

        public static extern int MemorySearch(ByteString mem, ByteString value, int start, bool backward);

        public static extern string[] StringSplit(string str, string separator);

        public static extern string[] StringSplit(string str, string separator, bool removeEmptyEntries);
    }
}
