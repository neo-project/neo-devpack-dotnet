using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Polymorphism : TestBase2<Contract_Polymorphism>
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(14, Contract.Sum(5, 9));
            Assert.AreEqual(1514550, Engine.FeeConsumed.Value);
            Assert.AreEqual(40, Contract.Mul(5, 8));
            Assert.AreEqual(1531890, Engine.FeeConsumed.Value);
            Assert.AreEqual("test", Contract.Test());
            Assert.AreEqual(1487760, Engine.FeeConsumed.Value);
            Assert.AreEqual("base.test", Contract.Test2());
            Assert.AreEqual(1812540, Engine.FeeConsumed.Value);
        }
    }
}
