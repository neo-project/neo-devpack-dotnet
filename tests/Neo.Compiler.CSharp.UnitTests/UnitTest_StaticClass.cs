using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticClass : DebugAndTestBase<Contract_StaticClass>
    {
        [TestMethod]
        public void Test_StaticClass()
        {
            Assert.AreEqual(2, Contract.TestStaticClass());
            Assert.AreEqual(1055910, Engine.FeeConsumed.Value);
        }
    }
}
