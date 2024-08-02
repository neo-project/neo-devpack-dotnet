using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_IndexOrRange : TestBase<Contract_IndexOrRange>
    {
        public UnitTest_IndexOrRange() : base(Contract_IndexOrRange.Nef, Contract_IndexOrRange.Manifest) { }

        [TestMethod]
        public void Test_Main()
        {
            var logs = new Queue<string>();
            Contract.OnRuntimeLog += (sender, log) => logs.Enqueue(log);
            Contract.TestMain();
            Assert.AreEqual(32096940, Engine.FeeConsumed.Value);

            // Check logs
            Assert.AreEqual(18, logs.Count);
            Assert.AreEqual("10", logs.Dequeue());
            Assert.AreEqual("3", logs.Dequeue());
            Assert.AreEqual("8", logs.Dequeue());
            Assert.AreEqual("2", logs.Dequeue());
            Assert.AreEqual("2", logs.Dequeue());
            Assert.AreEqual("7", logs.Dequeue());
            Assert.AreEqual("3", logs.Dequeue());
            Assert.AreEqual("2", logs.Dequeue());
            Assert.AreEqual("1", logs.Dequeue());
            Assert.AreEqual("123456789", logs.Dequeue());
            Assert.AreEqual("123", logs.Dequeue());
            Assert.AreEqual("3456789", logs.Dequeue());
            Assert.AreEqual("45", logs.Dequeue());
            Assert.AreEqual("89", logs.Dequeue());
            Assert.AreEqual("123456", logs.Dequeue());
            Assert.AreEqual("45", logs.Dequeue());
            Assert.AreEqual("67", logs.Dequeue());
            Assert.AreEqual("1", logs.Dequeue());
        }
    }
}
