using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.IsTrue(engine.AddEntryScript(true, Utils.Extensions.TestContractRoot + "Contract_Debug.cs").Success);

            var result = engine.ExecuteTestCaseStandard("testElse");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());
            Assert.AreEqual(1, engine.Notifications.Count);
            Assert.AreEqual("Debug", engine.Notifications[0].EventName);
            Assert.AreEqual(1, engine.Notifications[0].State.Count);
            Assert.AreEqual("Debug compilation", engine.Notifications[0].State[0].GetString());

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("testIf");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            // No debug

            engine = new TestEngine();
            Assert.IsTrue(engine.AddEntryScript(false, Utils.Extensions.TestContractRoot + "Contract_Debug.cs").Success);
            result = engine.ExecuteTestCaseStandard("testElse");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(2, result.Pop().GetInteger());
            Assert.AreEqual(0, engine.Notifications.Count);

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("testIf");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(2, result.Pop().GetInteger());
        }
    }
}
