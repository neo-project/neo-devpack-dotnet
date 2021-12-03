using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
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
            Assert.IsTrue(_engine.AddEntryScript("./TestClasses/Contract_Attribute.cs").Success);
        }

        [TestMethod]
        public void test()
        {
            _engine.Reset();
            Assert.AreEqual(_engine.Manifest["name"].GetString(), "attr");
            var result = _engine.ExecuteTestCaseStandard("test");

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.IsTrue(result.Pop().GetBoolean());
        }
    }
}
