using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO;
using Neo.IO.Json;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using Array = Neo.VM.Types.Array;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class ContractTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Contract.cs");
        }

        [TestMethod]
        public void Test_GetCallFlags()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("getCallFlags").Pop();
            StackItem wantResult = 0b00001111;
            Assert.AreEqual(wantResult, result);
        }
    }
}
