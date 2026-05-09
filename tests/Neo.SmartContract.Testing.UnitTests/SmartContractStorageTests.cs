// Copyright (C) 2015-2026 The Neo Project.
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
using Neo.Json;
using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class SmartContractStorageTests
    {
        // Defines the prefix used to store the registration price in neo

        private readonly byte[] _registerPricePrefix = [13];

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

        [TestMethod]
        public void TestExportIncludesOnlyCurrentContractStorage()
        {
            TestEngine engine = new(true);

            var neoKey = "neo-export-scope";
            var gasKey = "gas-export-scope";

            engine.Native.NEO.Storage.Put(neoKey, BigInteger.One);
            engine.Native.GAS.Storage.Put(gasKey, BigInteger.One);

            var storage = engine.Native.NEO.Storage.Export();
            var prefix = (JObject)storage.Properties.Single().Value!;

            Assert.IsTrue(prefix.ContainsProperty(Convert.ToBase64String(Encoding.UTF8.GetBytes(neoKey))));
            Assert.IsFalse(prefix.ContainsProperty(Convert.ToBase64String(Encoding.UTF8.GetBytes(gasKey))));
        }

        [TestMethod]
        public void TestTypedReadHelpers()
        {
            TestEngine engine = new(true);

            engine.Native.NEO.Storage.Put("integer-key", BigInteger.MinusOne);
            engine.Native.NEO.Storage.Put("string-key", Encoding.UTF8.GetBytes("hello"));
            engine.Native.NEO.Storage.Put((byte)0x42, BigInteger.One);
            engine.Native.NEO.Storage.Put(new byte[] { 0x43 }, Encoding.UTF8.GetBytes("raw"));

            Assert.IsTrue(engine.Native.NEO.Storage.TryGet("integer-key", out var integerValue));
            CollectionAssert.AreEqual(BigInteger.MinusOne.ToByteArray(), integerValue.ToArray());
            Assert.IsFalse(engine.Native.NEO.Storage.TryGet("missing-key", out var missingValue));
            Assert.AreEqual(0, missingValue.Length);

            Assert.AreEqual(BigInteger.MinusOne, engine.Native.NEO.Storage.GetInteger("integer-key"));
            Assert.AreEqual("hello", engine.Native.NEO.Storage.GetString("string-key"));
            Assert.AreEqual(BigInteger.One, engine.Native.NEO.Storage.GetInteger((byte)0x42));
            Assert.AreEqual("raw", engine.Native.NEO.Storage.GetString(new byte[] { 0x43 }));
            Assert.AreEqual(BigInteger.Zero, engine.Native.NEO.Storage.GetInteger("missing-key"));
            Assert.AreEqual(string.Empty, engine.Native.NEO.Storage.GetString("missing-key"));
        }
    }
}
