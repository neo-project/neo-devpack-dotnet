using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Initializer : TestBase<Contract_Initializer>
    {
        public UnitTest_Initializer() : base(Contract_Initializer.Nef, Contract_Initializer.Manifest) { }

        [TestMethod]
        public void Initializer_Test()
        {
            Assert.AreEqual(3, Contract.Sum());
            Assert.AreEqual(12, Contract.Sum1(5,7));
            Assert.AreEqual(12, Contract.Sum2(5, 7));
        }
    }
}
