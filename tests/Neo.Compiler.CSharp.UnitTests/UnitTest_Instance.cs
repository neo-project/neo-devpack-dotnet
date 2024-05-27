using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Instance : TestBase<Contract_Instance>
    {
        public UnitTest_Instance() : base(Contract_Instance.Nef, Contract_Instance.Manifest) { }

        [TestMethod]
        public void TestFunc()
        {
            Assert.AreEqual(3, Contract.Sum(2));
            Assert.AreEqual(4, Contract.Sum(3));
            Assert.AreEqual(8, Contract.Sum2(3));
        }
    }
}
