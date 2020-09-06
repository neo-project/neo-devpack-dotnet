using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Oracle
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Oracle.cs");
        }

        [TestMethod]
        public void TestHash()
        {
            // var attr = typeof(Oracle).GetCustomAttribute<ContractAttribute>();
            Assert.AreEqual(NativeContract.Oracle.Hash.ToString(), "0x3c05b488bf4cf699d0631bf80190896ebbf38c3b");
        }

        [TestMethod]
        public void TestGetOracleNodes()
        {
            var result = _engine.ExecuteTestCaseStandard("testGetOracleNodes");

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            // TODO: FIX UT
        }
    }
}
