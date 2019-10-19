using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM.Types;
using System.Linq;

namespace Neo.Compiler.MSIL.SmartContractFramework.Services.Neo
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
                    a.Key.Key.SequenceEqual(prefix.Concat(key)) &&
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
            Assert.AreEqual(1, testengine.Snapshot.Storages.GetChangeSet().Count(a => a.Key.Key.SequenceEqual(prefix.Concat(key))));
            return rItem.GetByteArray();
        }

        private void Delete(TestEngine testengine, string method, byte[] prefix, byte[] key)
        {
            var result = testengine.ExecuteTestCaseStandard(method, new ByteArray(key));
            Assert.AreEqual(1, result.Count);
            var rItem = result.Pop();
            Assert.IsInstanceOfType(rItem, typeof(ByteArray));
            Assert.AreEqual(0, testengine.Snapshot.Storages.GetChangeSet().Count(a => a.Key.Key.SequenceEqual(prefix.Concat(key))));
        }

        [TestMethod]
        public void Test_Byte()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Storage.cs");
            testengine.Snapshot.Contracts.Add(testengine.EntryScriptHash, new Ledger.ContractState()
            {
                Script = testengine.EntryContext.Script,
                Manifest = new SmartContract.Manifest.ContractManifest()
                {
                    Features = SmartContract.Manifest.ContractFeatures.HasStorage
                }
            });

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
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Storage.cs");
            testengine.Snapshot.Contracts.Add(testengine.EntryScriptHash, new Ledger.ContractState()
            {
                Script = testengine.EntryContext.Script,
                Manifest = new SmartContract.Manifest.ContractManifest()
                {
                    Features = SmartContract.Manifest.ContractFeatures.HasStorage
                }
            });

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
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Storage.cs");
            testengine.Snapshot.Contracts.Add(testengine.EntryScriptHash, new Ledger.ContractState()
            {
                Script = testengine.EntryContext.Script,
                Manifest = new SmartContract.Manifest.ContractManifest()
                {
                    Features = SmartContract.Manifest.ContractFeatures.HasStorage
                }
            });

            var prefix = new byte[] { 0x61, 0x61 };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Put(testengine, "TestPutString", prefix, key, value);

            // Get

            testengine.Reset();
            var getVal = Get(testengine, "TestGetString", prefix, key);
            CollectionAssert.AreEqual(value, getVal);

            // Delete

            testengine.Reset();
            Delete(testengine, "TestDeleteString", prefix, key);
        }
    }
}
