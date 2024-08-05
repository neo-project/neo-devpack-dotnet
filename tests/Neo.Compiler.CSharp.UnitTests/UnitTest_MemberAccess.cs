using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MemberAccess() : TestBase<Contract_MemberAccess>(Contract_MemberAccess.Nef, Contract_MemberAccess.Manifest)
    {
        [TestMethod]
        public void Test_Main()
        {
            var logs = new Queue<string>();
            Contract.OnRuntimeLog += (sender, log) => logs.Enqueue(log);
            Contract.TestMain();
            Assert.AreEqual(6370920, Engine.FeeConsumed.Value);

            // Check logs
            Assert.AreEqual(4, logs.Count);
            Assert.AreEqual("0", logs.Dequeue());
            Assert.AreEqual("msg", logs.Dequeue());
            Assert.AreEqual("hello", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
        }
    }
}
