using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.TestingEngine;
using Neo.Ledger;
using Neo.VM.Types;
using System;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class StorageTest
    {
        private void Put(TestEngine testengine, string method, byte[] prefix, byte[] key, byte[] value)
        {
            var result = testengine.ExecuteTestCaseStandard(method, new ByteString(key), new ByteString(value));
            Assert.AreEqual(VM.VMState.HALT, testengine.State);
            var rItem = result.Pop();

            Assert.IsInstanceOfType(rItem, typeof(Integer));
            Assert.AreEqual(1, ((Integer)rItem).GetInteger());
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
            var result = testengine.ExecuteTestCaseStandard(method, new ByteString(key));
            Assert.AreEqual(VM.VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);
            var rItem = result.Pop();
            Assert.IsInstanceOfType(rItem, typeof(ByteString));
            ReadOnlySpan<byte> data = rItem as ByteString;

            Assert.AreEqual(1, testengine.Snapshot.Storages.GetChangeSet().Count(a => a.Key.Key.SequenceEqual(Concat(prefix, key))));
            return data.ToArray();
        }

        private void Delete(TestEngine testengine, string method, byte[] prefix, byte[] key)
        {
            var result = testengine.ExecuteTestCaseStandard(method, new ByteString(key));
            Assert.AreEqual(VM.VMState.HALT, testengine.State);
            Assert.AreEqual(0, result.Count);
            Assert.AreEqual(0, testengine.Snapshot.Storages.GetChangeSet().Count(a => a.Key.Key.SequenceEqual(Concat(prefix, key))));
        }

        private byte[] Concat(byte[] prefix, byte[] key)
        {
            return global::Neo.Helper.Concat(prefix, key);
        }

        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            var _ = TestBlockchain.TheNeoSystem;
            var snapshot = Blockchain.Singleton.GetSnapshot();

            testengine = new TestEngine(snapshot: snapshot.Clone());
            testengine.AddEntryScript("./TestClasses/Contract_Storage.cs");
            Assert.AreEqual(ContractFeatures.HasStorage, testengine.ScriptEntry.converterIL.outModule.attributes
                .Where(u => u.AttributeType.FullName == "Neo.SmartContract.Framework.FeaturesAttribute")
                .Select(u => (ContractFeatures)u.ConstructorArguments.FirstOrDefault().Value)
                .FirstOrDefault());

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

            Put(testengine, "testPutByte", prefix, key, value);

            // Get

            testengine.Reset();
            var getVal = Get(testengine, "testGetByte", prefix, key);
            CollectionAssert.AreEqual(value, getVal);

            // Delete

            testengine.Reset();
            Delete(testengine, "testDeleteByte", prefix, key);
        }

        [TestMethod]
        public void Test_ByteArray()
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Put(testengine, "testPutByteArray", prefix, key, value);

            // Get

            testengine.Reset();
            var getVal = Get(testengine, "testGetByteArray", prefix, key);
            CollectionAssert.AreEqual(value, getVal);

            // Delete

            testengine.Reset();
            Delete(testengine, "testDeleteByteArray", prefix, key);
        }

        [TestMethod]
        public void Test_LongBytes()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testOver16Bytes");
            Assert.AreEqual(VM.VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            ByteString bs = result.Pop() as ByteString;
            var value = new byte[] { 0x3b, 0x00, 0x32, 0x03, 0x23, 0x23, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02 };

            Assert.AreEqual(new ByteString(value), bs);
        }

        [TestMethod]
        public void Test_String()
        {
            var prefix = new byte[] { 0x61, 0x61 };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            testengine.Reset();
            Put(testengine, "testPutString", prefix, key, value);

            // Get

            testengine.Reset();
            var getVal = Get(testengine, "testGetString", prefix, key);
            CollectionAssert.AreEqual(value, getVal);

            // Delete

            testengine.Reset();
            Delete(testengine, "testDeleteString", prefix, key);
        }

        [TestMethod]
        public void Test_ReadOnly()
        {
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testPutReadOnly", new ByteString(key), new ByteString(value));
            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
            Assert.AreEqual(0, result.Count);
        }
    }
}
