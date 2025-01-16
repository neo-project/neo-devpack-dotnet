// Copyright (C) 2015-2024 The Neo Project.
//
// SmartContractStorageTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class SmartContractStorageTests
    {
        // Defines the prefix used to store the registration price in neo

        private readonly byte[] _registerPricePrefix = new byte[] { 13 };

        [TestMethod]
        public void TestAlterStorage()
        {
            // Create and initialize TestEngine

            TestEngine engine = new(true);

            // Check previous data

            Assert.AreEqual(100000000000, engine.Native.NEO.RegisterPrice);

            // Alter data

            engine.Native.NEO.Storage.Put(_registerPricePrefix, BigInteger.MinusOne);

            // Check altered data

            Assert.AreEqual(BigInteger.MinusOne, engine.Native.NEO.RegisterPrice);
        }

        [TestMethod]
        public void TestExportImport()
        {
            // Create and initialize TestEngine

            TestEngine engine = new(true);

            // Check previous data

            Assert.AreEqual(100000000000, engine.Native.NEO.RegisterPrice);

            var storage = engine.Native.NEO.Storage.Export();

            // Alter data

            storage[storage.Properties.First().Key]![Convert.ToBase64String(_registerPricePrefix)] = Convert.ToBase64String(BigInteger.MinusOne.ToByteArray());
            engine.Native.NEO.Storage.Import(storage);

            // Check altered data

            Assert.AreEqual(BigInteger.MinusOne, engine.Native.NEO.RegisterPrice);
        }
    }
}
