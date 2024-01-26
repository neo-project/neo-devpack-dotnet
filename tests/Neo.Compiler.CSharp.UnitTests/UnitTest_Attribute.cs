using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Attribute
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            Assert.IsTrue(_engine.AddEntryScript<Contract_Attribute>().Success);
        }

        [TestMethod]
        public void test()
        {
            _engine.Reset();
            Assert.AreEqual(_engine.Manifest.Name, "attr");
            var result = _engine.ExecuteTestCaseStandard("test");

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.IsTrue(result.Pop().GetBoolean());
        }
    }
}
