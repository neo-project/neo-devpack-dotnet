using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Foreach
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Foreach.cs");
        }

        [TestMethod]
        public void intForeach_test()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("intForeach");

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(10, result.Pop().GetInteger());
        }
    }
}
