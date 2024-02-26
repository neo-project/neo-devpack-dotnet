using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.TestEngine;
using Neo.VM;
using Neo.Wallets;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class OracleTest
    {

        private TestEngine.TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine.TestEngine();
            _engine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_IOracle.cs");
        }

        [TestMethod]
        public void Test_OracleResponse()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("onOracleResponse", "http://127.0.0.1", "test", 0x14, "{}");
            Assert.AreEqual(VMState.FAULT, _engine.State);

        }

    }
}
