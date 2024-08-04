using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Instance : TestBase2<Contract_Instance>
    {
        public UnitTest_Instance() : base(Contract_Instance.Nef, Contract_Instance.Manifest) { }

        [TestMethod]
        public void TestFunc()
        {
            Assert.AreEqual(3, Contract.Sum(2));
            Assert.AreEqual(1640160, Engine.FeeConsumed.Value);
            Assert.AreEqual(4, Contract.Sum(3));
            Assert.AreEqual(1640160, Engine.FeeConsumed.Value);
            Assert.AreEqual(8, Contract.Sum2(3));
            Assert.AreEqual(1678110, Engine.FeeConsumed.Value);
        }
    }
}
