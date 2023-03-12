using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.UnitTest.Framework;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Debug
    {
        [TestMethod]
        public void TestDebug()
        {
            // Debug

            var engine = new TestEngine();
            Assert.IsTrue(engine.AddEntryScript(true, "./TestClasses/Contract_Debug.cs").Success);

            var result = engine.ExecuteTestCaseStandard("testElse");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("testIf");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            // No debug

            engine = new TestEngine();
            Assert.IsTrue(engine.AddEntryScript(false, "./TestClasses/Contract_Debug.cs").Success);
            result = engine.ExecuteTestCaseStandard("testElse");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(2, result.Pop().GetInteger());

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("testIf");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(2, result.Pop().GetInteger());
        }
    }
}
