using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Polymorphism : TestBase<Contract_Polymorphism>
    {
        public UnitTest_Polymorphism() : base(Contract_Polymorphism.Nef, Contract_Polymorphism.Manifest) { }

        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(14, Contract.Sum(5, 9));
            Assert.AreEqual(40, Contract.Mul(5, 8));
            Assert.AreEqual("test", Contract.Test());
            Assert.AreEqual("base.test", Contract.Test2());
        }
    }
}
