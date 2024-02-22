using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.VM;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class SequencePointInserterTest
    {
        private TestEngine.TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            var system = TestBlockchain.TheNeoSystem;
            var snapshot = system.GetSnapshot().CreateSnapshot();

            testengine = new TestEngine.TestEngine(snapshot: snapshot);
            Assert.IsTrue(testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_SequencePointInserter.cs").Success);
        }

        [TestMethod]
        public void Test_SequencePointInserter()
        {
            var points = (testengine.DebugInfo["methods"][0]["sequence-points"] as JArray).Select(u => u.GetString()).ToArray();

            // Ensure that all the instructions have sequence point

            var ip = 0;
            Script script = testengine.Nef.Script;

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);

                if (ip != 0) // Avoid INITSLOT
                {
                    Assert.IsTrue(points.Any(u => u.StartsWith($"{ip}[")), $"Offset {ip} with '{instruction.OpCode}' is not in sequence points.");
                }

                ip += instruction.Size;
            }
        }

        [TestMethod]
        public void Test_If()
        {
            testengine.Reset();
            var ret = testengine.ExecuteTestCaseStandard("test", 1);
            Assert.AreEqual(1, ret.Count);
            Assert.AreEqual(23, ret.Pop().GetInteger());

            testengine.Reset();
            ret = testengine.ExecuteTestCaseStandard("test", 0);
            Assert.AreEqual(1, ret.Count);
            Assert.AreEqual(45, ret.Pop().GetInteger());
        }
    }
}
