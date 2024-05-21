using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Delegate : TestBase<Contract_Delegate>
    {
        public UnitTest_Delegate() : base(Contract_Delegate.Nef, Contract_Delegate.Manifest) { }

        [TestMethod]
        public void TestFunc()
        {
            Assert.AreEqual(5, Contract.SumFunc(2, 3));
        }
    }
}
