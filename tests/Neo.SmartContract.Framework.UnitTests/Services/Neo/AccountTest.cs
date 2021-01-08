using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Extensions;
using Neo.Compiler.MSIL.UnitTests.Utils;
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

            _engine.Snapshot.ContractAdd(new ContractState()
            {
                Hash = new UInt160(noStandard),
                Nef = new NefFile() { Script = System.Array.Empty<byte>() }
            });

            // Empty

            var result = _engine.ExecuteTestCaseStandard("accountIsStandard", new VM.Types.ByteString(new byte[0]));
            Assert.AreEqual(VM.VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            // No standard

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("accountIsStandard", new VM.Types.ByteString(new byte[20]));
            Assert.AreEqual(VM.VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(false, item.GetBoolean());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("accountIsStandard", new VM.Types.ByteString(noStandard));
            Assert.AreEqual(VM.VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(false, item.GetBoolean());
        }
    }
}
