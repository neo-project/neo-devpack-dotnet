using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.SmartContractFramework.Services.Neo
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
            // Empty

            var result = _engine.ExecuteTestCaseStandard("AccountIsStandard", new ByteArray(new byte[0]));
            Assert.AreEqual(VM.VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);
            _engine.InvocationStack.Clear();

            // No standard

            result = _engine.ExecuteTestCaseStandard("AccountIsStandard", new ByteArray(new byte[20]));
            Assert.AreEqual(VM.VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(false, item.GetBoolean());
            _engine.InvocationStack.Clear();

            // Standard

            result = _engine.ExecuteTestCaseStandard("AccountIsStandard", new ByteArray(new byte[20]
                {
                    0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
                    0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF
                }));
            Assert.AreEqual(VM.VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(true, item.GetBoolean());
        }
    }
}
