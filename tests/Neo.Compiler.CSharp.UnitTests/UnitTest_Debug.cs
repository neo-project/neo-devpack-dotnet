using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;
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
            Assert.IsTrue(engine.AddEntryScript(typeof(Contract_Debug)).Success);

            var result = engine.ExecuteTestCaseStandard("testElse");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("testIf");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            // No debug

            engine = new TestEngine();
            Assert.IsTrue(engine.AddEntryScript(false, typeof(Contract_Debug)).Success);
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
