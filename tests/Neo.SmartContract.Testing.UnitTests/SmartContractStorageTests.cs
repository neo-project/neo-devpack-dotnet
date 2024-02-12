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
            // Engine an contract creation

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
            // Engine an contract creation

            TestEngine engine = new(true);

            // Check previous data

            Assert.AreEqual(100000000000, engine.Native.NEO.RegisterPrice);

            var storage = engine.Native.NEO.Storage.Export();

            // Alter data

            storage[storage.Properties.First().Key.ToString()][Convert.ToBase64String(_registerPricePrefix)] = Convert.ToBase64String(BigInteger.MinusOne.ToByteArray());
            engine.Native.NEO.Storage.Import(storage);

            // Check altered data

            Assert.AreEqual(BigInteger.MinusOne, engine.Native.NEO.RegisterPrice);
        }
    }
}
