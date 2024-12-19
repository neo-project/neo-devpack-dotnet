using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Initializer : DebugAndTestBase<Contract_Initializer>
    {
        [TestMethod]
        public void Initializer_Test()
        {
            Assert.AreEqual(3, Contract.Sum());
            AssertGasConsumed(1052100);
            Assert.AreEqual(12, Contract.Sum1(5, 7));
            AssertGasConsumed(1113210);
            Assert.AreEqual(12, Contract.Sum2(5, 7));
            AssertGasConsumed(1605330);
        }
    }
}
