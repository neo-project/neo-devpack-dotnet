using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Attributes;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class ExecutionEngineTest : TestBase<Contract_ExecutionEngine>
    {
        public class Transaction
        {
            [FieldOrder(0)]
            public UInt256? Hash { get; set; }

            [FieldOrder(1)]
            public byte Version { get; set; }

            [FieldOrder(2)]
            public uint Nonce { get; set; }

            [FieldOrder(3)]
            public UInt160? Sender { get; set; }

            [FieldOrder(4)]
            public long SystemFee { get; set; }

            [FieldOrder(5)]
            public long NetworkFee { get; set; }

            [FieldOrder(6)]
            public uint ValidUntilBlock { get; set; }

            [FieldOrder(7)]
            public ByteString? Script { get; set; }
        }

        public ExecutionEngineTest() : base(Contract_ExecutionEngine.Nef, Contract_ExecutionEngine.Manifest) { }

        [TestMethod]
        public void CallingScriptHashTest()
        {
            var hash = Contract.CallingScriptHash();
            Assert.AreEqual(Engine.Transaction.Script.Span.ToScriptHash().ToString(), new UInt160(hash!).ToString());
        }

        [TestMethod]
        public void EntryScriptHashTest()
        {
            var hash = Contract.EntryScriptHash();
            Assert.AreEqual(Engine.Transaction.Script.Span.ToScriptHash().ToString(), new UInt160(hash!).ToString());
        }

        [TestMethod]
        public void ExecutingScriptHashTest()
        {
            Assert.AreEqual(Contract.Hash.ToString(), new UInt160(Contract.ExecutingScriptHash()).ToString());
        }

        [TestMethod]
        public void ScriptContainerTest()
        {
            var item = Contract.ScriptContainer() as StackItem;
            var tx = item?.ConvertTo(typeof(Transaction)) as Transaction;

            Assert.AreEqual(Engine.Transaction.Hash.ToString(), tx?.Hash?.ToString());
        }

        [TestMethod]
        public void TransactionTest()
        {
            var item = Contract.Transaction() as StackItem;
            var tx = item?.ConvertTo(typeof(Transaction)) as Transaction;

            Assert.AreEqual(Engine.Transaction.Hash.ToString(), tx?.Hash?.ToString());
        }
    }
}
