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

            // TODO: check logs
            Assert.AreEqual(18, logs.Count);
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
        }
    }
}
