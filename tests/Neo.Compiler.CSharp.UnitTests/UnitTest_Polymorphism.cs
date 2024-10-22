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
            AssertGasConsumed(1479870);
            Assert.AreEqual(40, Contract.Mul(5, 8));
            AssertGasConsumed(1479870);
            Assert.AreEqual("test", Contract.Test());
            AssertGasConsumed(1418400);
            Assert.AreEqual("base.test", Contract.Test2());
            AssertGasConsumed(1743180);
        }
    }
}
