using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.Ledger;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;
using System;

namespace Neo.Compiler.MSIL.SmartContractFramework.Services.Neo
{
    [TestClass]
    public class NativeTest
    {
        private const byte Prefix_FeePerByte = 10;

        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Native.cs");
        }

        [TestMethod]
        public void Test_NEO()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("NEO_Decimals");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetBigInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("NEO_Name");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("NEO", item.GetString());
        }

        [TestMethod]
        public void Test_GAS()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("GAS_Decimals");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(8, item.GetBigInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("GAS_Name");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("GAS", item.GetString());
        }

        [TestMethod]
        public void Test_Policy()
        {
            _engine.Reset();
            _engine.Snapshot.Storages.Add(CreateStorageKey(NativeContract.Policy.Hash, Prefix_FeePerByte),
                new StorageItem() { Value = BitConverter.GetBytes(1024L) }
                );

            var result = _engine.ExecuteTestCaseStandard("Policy_GetFeePerByte");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(1024L, item.GetBigInteger());
        }

        StorageKey CreateStorageKey(UInt160 hash, byte prefix, byte[] key = null)
        {
            StorageKey storageKey = new StorageKey
            {
                ScriptHash = hash,
                Key = new byte[sizeof(byte) + (key?.Length ?? 0)]
            };
            storageKey.Key[0] = prefix;
            if (key != null)
                Buffer.BlockCopy(key, 0, storageKey.Key, 1, key.Length);
            return storageKey;
        }
    }
}
