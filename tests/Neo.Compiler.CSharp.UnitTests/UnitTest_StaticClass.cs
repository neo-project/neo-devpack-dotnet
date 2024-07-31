using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticClass : TestBase<Contract_StaticClass>
    {
        public UnitTest_StaticClass() : base(Contract_StaticClass.Nef, Contract_StaticClass.Manifest) { }

        [TestMethod]
        public void Test_StaticClass()
        {
            Assert.AreEqual(2, Contract.TestStaticClass());
            Assert.AreEqual(1055910, Engine.FeeConsumed.Value);
        }
    }
}
