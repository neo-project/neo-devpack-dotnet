using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class Contract_NativeContracts
    {
        [TestMethod]
        public void Test_NEO()
        {
            var testengine = new TestEngine();
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
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_NativeContracts.cs");

            var result = testengine.ExecuteTestCaseStandard("gASName");

            Assert.AreEqual(VM.VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var entry = result.Pop();

            Assert.AreEqual("GAS", entry.GetString());
        }
    }
}
