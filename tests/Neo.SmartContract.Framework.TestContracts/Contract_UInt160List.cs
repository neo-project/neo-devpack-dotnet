// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_UInt160List.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    /// <summary>
    /// Test contract exposing two independent <see cref="UInt160List"/> instances (list A and list B).
    /// </summary>
    public class Contract_UInt160List : SmartContract
    {
        private static UInt160List ListA => new UInt160List(0xE1);
        private static UInt160List ListB => new UInt160List(new byte[] { 0xE2, 0x00 });

        public static void Add(UInt160 value) => ListA.Add(value);
        public static UInt160 RemoveAt(BigInteger index) => ListA.RemoveAt(index);
        public static BigInteger Length() => ListA.Length;
        public static UInt160 At(BigInteger index) => ListA.At(index);

        public static UInt160[] Values()
        {
            var result = new UInt160[(int)ListA.Length];
            var it = ListA.Values();
            var i = 0;
            while (it.Next())
                result[i++] = (UInt160)it.Value;
            return result;
        }

        public static void AddB(UInt160 value) => ListB.Add(value);
        public static BigInteger LengthB() => ListB.Length;
        public static UInt160 AtB(BigInteger index) => ListB.At(index);

        public static void AddInvalid(byte[] value) => ListA.Add((UInt160)(ByteString)value);

        public static void AddThenAbort(UInt160 value)
        {
            ListA.Add(value);
            ExecutionEngine.Abort();
        }

        public static BigInteger CreateWithPrefixLength(BigInteger size)
        {
            var list = new UInt160List(new byte[(int)size]);
            return list.Length;
        }
    }
}
