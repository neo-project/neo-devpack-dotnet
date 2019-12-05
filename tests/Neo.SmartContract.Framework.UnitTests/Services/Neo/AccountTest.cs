using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.Ledger;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class AccountTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Account.cs");
        }

        [TestMethod]
        public void Test_AccountIsStandard()
        {
            var noStandard = new byte[20]
                  {
                    0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
                    0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF
                  };

            _engine.Snapshot.Contracts.Add(new UInt160(noStandard), new ContractState()
            {
                Script = new byte[0] { }
            });

            // Empty

            var result = _engine.ExecuteTestCaseStandard("AccountIsStandard", new ByteArray(new byte[0]));
            Assert.AreEqual(VM.VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            // Standard

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("AccountIsStandard", new ByteArray(new byte[20]));
            Assert.AreEqual(VM.VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(true, item.ToBoolean());

            // No standard

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("AccountIsStandard", new ByteArray(noStandard));
            Assert.AreEqual(VM.VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(false, item.ToBoolean());
        }
    }
}
