using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Attribute : TestBase2<Contract_AttributeChanged>
    {
        public UnitTest_Attribute() : base(Contract_AttributeChanged.Nef, Contract_AttributeChanged.Manifest) { }

        [TestMethod]
        public void AttributeTest()
        {
            Assert.AreEqual(Contract_AttributeChanged.Manifest.Name, "Contract_AttributeChanged");
            Assert.IsTrue(Contract.Test());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }
    }
}
