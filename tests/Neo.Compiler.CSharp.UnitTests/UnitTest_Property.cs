using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property : TestBase<Contract_Property>
    {
        public UnitTest_Property() : base(Contract_Property.Nef, Contract_Property.Manifest) { }

        [TestMethod]
        public void TestABIOffsetWithoutOptimizer()
        {
            var property = Contract_Property.Manifest.Abi.Methods[0];
            Assert.AreEqual("symbol", property.Name);
        }
    }
}
