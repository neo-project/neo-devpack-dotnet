using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class StorageTest
    {
        private void Put(TestEngine testengine, string method, byte[] prefix, byte[] key, byte[] value)
        {
            var result = testengine.ExecuteTestCaseStandard(method, new ByteArray(key), new ByteArray(value));
            var rItem = result.Pop();

            Assert.IsInstanceOfType(rItem, typeof(Integer));
            Assert.AreEqual(1, rItem.GetBigInteger());
            Assert.AreEqual(1,
                testengine.Snapshot.Storages.GetChangeSet()
                .Count(a =>
                    a.Key.Key.SequenceEqual(Concat(prefix, key)) &&
                    a.Item.Value.SequenceEqual(value) &&
                    !a.Item.IsConstant
                    ));
        }

        private byte[] Get(TestEngine testengine, string method, byte[] prefix, byte[] key)
        {
            var result = testengine.ExecuteTestCaseStandard(method, new ByteArray(key));
            Assert.AreEqual(1, result.Count);
            var rItem = result.Pop();
            Assert.IsInstanceOfType(rItem, typeof(ByteArray));
            Assert.AreEqual(1, testengine.Snapshot.Storages.GetChangeSet().Count(a => a.Key.Key.SequenceEqual(Concat(prefix, key))));
            return rItem.GetByteArray();
        }

        private void Delete(TestEngine testengine, string method, byte[] prefix, byte[] key)
        {
            var result = testengine.ExecuteTestCaseStandard(method, new ByteArray(key));
            Assert.AreEqual(1, result.Count);
            var rItem = result.Pop();
            Assert.IsInstanceOfType(rItem, typeof(ByteArray));
            Assert.AreEqual(0, testengine.Snapshot.Storages.GetChangeSet().Count(a => a.Key.Key.SequenceEqual(Concat(prefix, key))));
        }

        private byte[] Concat(byte[] prefix, params byte[] key)
        {
            var l = new List<byte>(prefix);
            l.AddRange(key);

            return l.ToArray();
        }

        TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Storage.cs");
            Assert.AreEqual(1, testengine.ScriptEntry.converterIL.outModule.attributes
                .Count(u => u.AttributeType.Name == "StorageAttribute"));

            testengine.Snapshot.Contracts.Add(testengine.EntryScriptHash, new Ledger.ContractState()
            {
                Script = testengine.EntryContext.Script,
                Manifest = new Manifest.ContractManifest()
                {
                    Features = Manifest.ContractFeatures.HasStorage
                }
            });
        }

        [TestMethod]
        public void Test_Byte()
        {
            var prefix = new byte[] { 0xAA };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Put(testengine, "TestPutByte", prefix, key, value);

            // Get

            testengine.Reset();
            var getVal = Get(testengine, "TestGetByte", prefix, key);
            CollectionAssert.AreEqual(value, getVal);

            // Delete

            testengine.Reset();
            Delete(testengine, "TestDeleteByte", prefix, key);
        }

        [TestMethod]
        public void Test_ByteArray()
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Put(testengine, "TestPutByteArray", prefix, key, value);

            // Get

            testengine.Reset();
            var getVal = Get(testengine, "TestGetByteArray", prefix, key);
            CollectionAssert.AreEqual(value, getVal);

            // Delete

            testengine.Reset();
            Delete(testengine, "TestDeleteByteArray", prefix, key);
        }

        [TestMethod]
        public void Test_String()
        {
            var prefix = new byte[] { 0x61, 0x61 };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            testengine.Reset();
            Put(testengine, "TestPutString", prefix, key, value);

            // Get

            testengine.Reset();
            var getVal = Get(testengine, "TestGetString", prefix, key);
            CollectionAssert.AreEqual(value, getVal);

            // Delete

            testengine.Reset();
            Delete(testengine, "TestDeleteString", prefix, key);
        }

        [TestMethod]
        public void Test_ReadOnly()
        {
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("TestPutReadOnly", new ByteArray(key), new ByteArray(value));
            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
            Assert.AreEqual(0, result.Count);
        }
    }
}
