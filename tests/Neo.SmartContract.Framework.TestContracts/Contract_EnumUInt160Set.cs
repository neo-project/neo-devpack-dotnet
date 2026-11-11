// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_EnumUInt160Set.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Services;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    /// <summary>
    /// Test contract exposing two independent <see cref="EnumUInt160Set"/> instances (set A and set B).
    /// </summary>
    public class Contract_EnumUInt160Set : SmartContract
    {
        private static EnumUInt160Set SetA => new EnumUInt160Set(0xE1);
        private static EnumUInt160Set SetB => new EnumUInt160Set(new byte[] { 0xE2, 0x00 });

        public static bool Add(UInt160 value) => SetA.Add(value);
        public static bool Remove(UInt160 value) => SetA.Remove(value);
        public static bool Contains(UInt160 value) => SetA.Contains(value);
        public static BigInteger Length() => SetA.Length;
        public static UInt160 At(BigInteger index) => SetA.At(index);

        public static UInt160[] Values()
        {
            var result = new UInt160[(int)SetA.Length];
            var it = SetA.Values();
            var i = 0;
            while (it.Next())
                result[i++] = (UInt160)it.Value;
            return result;
        }

        public static bool AddB(UInt160 value) => SetB.Add(value);
        public static bool RemoveB(UInt160 value) => SetB.Remove(value);
        public static bool ContainsB(UInt160 value) => SetB.Contains(value);
        public static BigInteger LengthB() => SetB.Length;
        public static UInt160 AtB(BigInteger index) => SetB.At(index);

        public static bool AddInvalid(byte[] value) => SetA.Add((UInt160)(ByteString)value);

        public static void AddThenAbort(UInt160 value)
        {
            SetA.Add(value);
            ExecutionEngine.Abort();
        }

        public static void RemoveThenAbort(UInt160 value)
        {
            SetA.Remove(value);
            ExecutionEngine.Abort();
        }

        public static BigInteger CreateWithPrefixLength(BigInteger size)
        {
            var set = new EnumUInt160Set(new byte[(int)size]);
            return set.Length;
        }
    }
}
