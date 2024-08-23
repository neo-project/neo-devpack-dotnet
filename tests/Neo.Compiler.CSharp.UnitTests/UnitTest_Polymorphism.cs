using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Polymorphism : DebugAndTestBase<Contract_Polymorphism>
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(14, Contract.Sum(5, 9));
            AssertGasConsumed(1514550);
            Assert.AreEqual(40, Contract.Mul(5, 8));
            AssertGasConsumed(1531890);
            Assert.AreEqual("test", Contract.Test());
            AssertGasConsumed(1487760);
            Assert.AreEqual("base.test", Contract.Test2());
            AssertGasConsumed(1812540);
        }
    }
}
