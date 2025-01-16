using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MemberAccess : DebugAndTestBase<Contract_MemberAccess>
    {
        [TestMethod]
        public void Test_Main()
        {
            var logs = new Queue<string>();
            Contract.OnRuntimeLog += (sender, log) => logs.Enqueue(log);
            Contract.TestMain();
            AssertGasConsumed(6108390);

            // Check logs
            Assert.AreEqual(4, logs.Count);
            Assert.AreEqual("0", logs.Dequeue());
            Assert.AreEqual("msg", logs.Dequeue());
            Assert.AreEqual("hello", logs.Dequeue());
            Assert.AreEqual("", logs.Dequeue());
        }

        [TestMethod]
        public void Test_ComplexAssignment()
        {
            Contract.TestComplexAssignment();
            AssertGasConsumed(2964570);
        }

        [TestMethod]
        public void Test_StaticComplexAssignment()
        {
            Contract.TestStaticComplexAssignment();
            AssertGasConsumed(1237410);
        }
    }
}
