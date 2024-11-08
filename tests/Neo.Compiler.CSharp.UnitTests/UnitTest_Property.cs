using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property : DebugAndTestBase<Contract_Property>
    {
        [TestMethod]
        public void TestABIOffsetWithoutOptimizer()
        {
            var property = Contract_Property.Manifest.Abi.Methods[0];
            Assert.AreEqual("symbol", property.Name);
        }

        [TestMethod]
        public void TestStaticPropertyInc()
        {
            var res = Contract.TestStaticPropertyInc();
            Assert.AreEqual(3, res);
        }

        [TestMethod]
        public void TestPropertyInc()
        {
            var res = Contract.TestPropertyInc();
            Assert.AreEqual(3, res);
        }
    }
}
