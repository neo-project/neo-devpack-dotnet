using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Extensions
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript(typeof(Contract_Extensions));
        }

        [TestMethod]
        public void TestSum()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testSum", 3, 2);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(5, result.Pop().GetInteger());
        }
    }
}
