using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class OracleTest : TestBase<Contract_IOracle>
    {
        public OracleTest() : base(Contract_IOracle.Nef, Contract_IOracle.Manifest) { }

        [TestMethod]
        public void Test_OracleResponse()
        {
            Assert.ThrowsException<VMUnhandledException>(() => Contract.OnOracleResponse("http://127.0.0.1", "test", 0x14, "{}"));

            Engine.OnGetCallingScriptHash = (current, expected) => Engine.Native.Oracle.Hash;
            Contract.OnOracleResponse("http://127.0.0.1", "test", 0x14, "{}");
            AssertLogs("Oracle call!");
        }
    }
}
