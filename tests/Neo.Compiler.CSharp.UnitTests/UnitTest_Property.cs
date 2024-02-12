using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property
    {
        private TestEngine testEngine;

        [TestInitialize]
        public void Init()
        {
            testEngine = new TestEngine();
            testEngine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Property.cs");
        }

        [TestMethod]
        public void TestABIOffsetWithoutOptimizer()
        {
            testEngine.Reset();
            var property = testEngine.Manifest.Abi.Methods[0];
            Assert.AreEqual("symbol", property.Name);
        }
    }
}
