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
            Assert.AreEqual(14 + 45 - 4, Contract.SumToBeOverriden(5, 9));
            AssertGasConsumed(1227660);
            Assert.AreEqual(14, Contract.Sum(5, 9));
            AssertGasConsumed(1233330);
            Assert.AreEqual(40, Contract.Mul(5, 8));
            AssertGasConsumed(1233330);
            Assert.AreEqual("testFinal", Contract.Test());
            AssertGasConsumed(1110810);
            Assert.AreEqual("testbase2.test2.test", Contract.Test2());
            AssertGasConsumed(2085150);
        }
    }
}
