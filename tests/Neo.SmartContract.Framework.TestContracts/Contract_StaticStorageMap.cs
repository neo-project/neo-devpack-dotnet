// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_StaticStorageMap.cs file belongs to the neo project and is free
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
    public class Contract_StaticStorageMap : SmartContract
    {
        private static StorageMap Data = new(Storage.CurrentContext, "data");
        private static readonly StorageMap ReadonlyData = new(Storage.CurrentContext, "readonlydata");

        public static void Put(string message)
        {
            Data.Put(message, 1);
        }

#pragma warning disable CS8604
        public static BigInteger Get(string msg)
        {
            return (BigInteger)Data.Get(msg);
        }

        public static void PutReadonly(string message)
        {
            ReadonlyData.Put(message, 2);
        }

        public static BigInteger GetReadonly(string msg)
        {
            return (BigInteger)ReadonlyData.Get(msg);
        }

        public static void Put2(string message)
        {
            var Data2 = new StorageMap(Storage.CurrentContext, "data");
            Data2.Put(message, 3);
        }

        public static BigInteger Get2(string msg)
        {
            var Data2 = new StorageMap(Storage.CurrentContext, "data");
            return (BigInteger)Data2.Get(msg);
        }

        public static void teststoragemap_Putbyteprefix(byte x)
        {
            var store = new StorageMap(Storage.CurrentContext, x);
            store.Put("test1", 123);
        }

        public static BigInteger teststoragemap_Getbyteprefix(byte x)
        {
            var store = new StorageMap(Storage.CurrentContext, x);
            return (BigInteger)store.Get("test1");
        }
#pragma warning restore CS8604
    }
}
