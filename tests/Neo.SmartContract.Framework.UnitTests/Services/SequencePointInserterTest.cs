using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Template.UnitTests.templates;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class SequencePointInserterTest : TestBase<Contract_SequencePointInserter>
    {
        public SequencePointInserterTest() : base(Contract_SequencePointInserter.Nef, Contract_SequencePointInserter.Manifest) { }

        [TestMethod]
        public void Test_SequencePointInserter()
        {
            TestCleanup.EnsureArtifactsUpToDate();

            var debug = TestCleanup.DebugInfos[typeof(Contract_SequencePointInserter)];
            Assert.IsNotNull(debug);

            var points = debug.Methods[0].SequencePoints.Select(u => u.Address).ToArray();

            // Ensure that all the instructions have sequence point

            var ip = 0;
            Script script = NefFile.Script;

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);

                if (ip != 0) // Avoid INITSLOT
                {
                    Assert.IsTrue(points.Contains(ip), $"Offset {ip} with '{instruction.OpCode}' is not in sequence points.");
                }

                ip += instruction.Size;
            }
        }

        [TestMethod]
        public void Test_If()
        {
            Assert.AreEqual(23, Contract.Test(1));
            Assert.AreEqual(45, Contract.Test(0));
        }
    }
}
