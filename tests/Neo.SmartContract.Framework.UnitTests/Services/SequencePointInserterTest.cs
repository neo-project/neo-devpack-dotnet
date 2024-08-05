using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class SequencePointInserterTest : DebugAndTestBase<Contract_SequencePointInserter>
    {
        [TestMethod]
        public void Test_SequencePointInserter()
        {
            var debug = TestCleanup.CachedContracts[typeof(Contract_SequencePointInserter)].DbgInfo;
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
