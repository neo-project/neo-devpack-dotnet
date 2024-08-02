using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class HasCallATests
    {
        [TestMethod]
        public void Test_HasCallA()
        {
            Assert.IsFalse(EntryPoint.HasCallA(Contract_Polymorphism.Nef));
            Assert.IsFalse(EntryPoint.HasCallA(Contract_TryCatch.Nef));
        }
    }
}
