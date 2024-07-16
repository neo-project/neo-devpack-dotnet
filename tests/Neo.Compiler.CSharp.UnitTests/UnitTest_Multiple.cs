using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Multiple
    {
        [TestMethod]
        public void Test_MultipleContracts()
        {
            var engine = new TestEngine(true);
            var a = engine.Deploy<Contract_MultipleA>(Contract_MultipleA.Nef, Contract_MultipleA.Manifest);
            var b = engine.Deploy<Contract_MultipleB>(Contract_MultipleB.Nef, Contract_MultipleB.Manifest);

            Assert.IsTrue(a.Test());
            Assert.IsFalse(b.Test());
        }
    }
}
