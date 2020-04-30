using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class CallbackTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(TriggerType.Application);
            _engine.AddEntryScript("./TestClasses/Contract_Callback.cs", true, true);
        }

        [TestMethod]
        public void Test_CreatePointer()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("createCallback");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Pointer));
            Assert.AreEqual(6, ((Pointer)item).Position);

            // test it

            _engine.Reset();
            _engine.ExecuteTestCaseStandard(((Pointer)item).Position + 2/*TODO: Require optimization because it's absolute*/);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetBigInteger());
        }
    }
}
