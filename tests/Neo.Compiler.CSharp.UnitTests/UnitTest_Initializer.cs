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
            Assert.AreEqual(1596420, Engine.FeeConsumed.Value);
            Assert.AreEqual(12, Contract.Sum1(5, 7));
            Assert.AreEqual(2149290, Engine.FeeConsumed.Value);
            Assert.AreEqual(12, Contract.Sum2(5, 7));
            Assert.AreEqual(2149650, Engine.FeeConsumed.Value);
        }
    }
}
