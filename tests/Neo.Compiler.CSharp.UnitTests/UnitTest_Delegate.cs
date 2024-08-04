using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Delegate : TestBase2<Contract_Delegate>
    {
        public UnitTest_Delegate() : base(Contract_Delegate.Nef, Contract_Delegate.Manifest) { }

        [TestMethod]
        public void TestFunc()
        {
            Assert.AreEqual(5, Contract.SumFunc(2, 3));
            Assert.AreEqual(1065300, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestDelegate()
        {
            Contract.TestDelegate();
            Assert.AreEqual(3436020, Engine.FeeConsumed.Value);
        }
    }
}
