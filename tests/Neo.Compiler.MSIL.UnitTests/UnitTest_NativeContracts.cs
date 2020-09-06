using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.SmartContract;
using Neo.VM;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class Contract_NativeContracts
    {
        private TestSnapshot snapshot = new TestSnapshot();

        [TestInitialize]
        public void Test_Init()
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall(ApplicationEngine.Neo_Native_Deploy);
                script = sb.ToArray();
            }

            typeof(TestSnapshot).GetProperty("PersistingBlock").SetValue(snapshot, new Network.P2P.Payloads.Block() { Index = 0 });
            var testengine = new TestEngine(TriggerType.System, null, snapshot);
            testengine.LoadScript(script);
            Assert.AreEqual(VMState.HALT, testengine.Execute());
        }

        [TestMethod]
        public void Test_NEO()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            var result = testengine.ExecuteTestCaseStandard("nEOName");

            Assert.AreEqual(VM.VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual("NEO", entry.GetString());
        }

        [TestMethod]
        public void Test_GAS()
        {
            var testengine = new TestEngine(TriggerType.Application, null, snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            var result = testengine.ExecuteTestCaseStandard("gASName");

            Assert.AreEqual(VM.VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual("GAS", entry.GetString());
        }
    }
}
