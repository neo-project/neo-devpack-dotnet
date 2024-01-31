using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Interfaces
    {
        [TestMethod]
        public void TestError()
        {
            var testengine = new TestEngine(snapshot: null);
            var context = testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Interfaces.cs");

            Assert.IsFalse(context.Success);
            Assert.AreEqual(1, context.Diagnostics.Count);
            Assert.AreEqual(DiagnosticId.InterfaceCall, context.Diagnostics[0].Id);
        }
    }
}
