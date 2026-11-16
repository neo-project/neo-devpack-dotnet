// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_NaiveSerializedSet.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    /// <summary>
    /// GAS baseline only: a set stored as one serialized list under a single key.
    /// </summary>
    public class Contract_NaiveSerializedSet : SmartContract
    {
        private static readonly byte[] Key = new byte[] { 0xF1 };

        private static List<UInt160> Load()
        {
            var raw = Storage.Get(Storage.CurrentContext, Key);
            if (raw is null) return new List<UInt160>();
            return (List<UInt160>)StdLib.Deserialize(raw);
        }

        private static void Save(List<UInt160> list)
        {
            Storage.Put(Storage.CurrentContext, Key, StdLib.Serialize(list));
        }

        private static int IndexOf(List<UInt160> list, UInt160 value)
        {
            for (var i = 0; i < list.Count; i++)
                if (list[i] == value) return i;
            return -1;
        }

        public static bool Add(UInt160 value)
        {
            var list = Load();
            if (IndexOf(list, value) >= 0) return false;
            list.Add(value);
            Save(list);
            return true;
        }

        public static bool Remove(UInt160 value)
        {
            var list = Load();
            var index = IndexOf(list, value);
            if (index < 0) return false;
            var last = list.Count - 1;
            list[index] = list[last];
            list.RemoveAt(last);
            Save(list);
            return true;
        }

        public static bool Contains(UInt160 value) => IndexOf(Load(), value) >= 0;

        public static BigInteger Length() => Load().Count;

        public static UInt160 At(BigInteger index) => Load()[(int)index];
    }
}
