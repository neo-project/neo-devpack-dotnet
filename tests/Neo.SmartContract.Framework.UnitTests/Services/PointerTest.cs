using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM;
using Neo.VM.Types;
using System.Numerics;
using Neo.SmartContract.Framework.UnitTests.TestClasses;
using Neo.SmartContract.Framework.UnitTests.Utils;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class PointerTest
    {
        [TestMethod]
        public void Test_CreatePointer()
        {
            var engine = new TestEngine.TestEngine(TriggerType.Application);
            engine.AddEntryScript(typeof(Contract_Pointers));

            var result = engine.ExecuteTestCaseStandard("createFuncPointer");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Pointer));

            // Test pointer

            engine.Reset();
            engine.ExecuteTestCaseStandard(((Pointer)item).Position, 1);
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void Test_ExecutePointer()
        {
            var engine = new TestEngine.TestEngine(TriggerType.Application);
            engine.AddEntryScript(typeof(Contract_Pointers));

            var result = engine.ExecuteTestCaseStandard("callFuncPointer");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void Test_ExecutePointerWithArgs()
        {
            var engine = new TestEngine.TestEngine(TriggerType.Application);
            engine.AddEntryScript(typeof(Contract_Pointers));

            var result = engine.ExecuteTestCaseStandard("callFuncPointerWithArg");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            var wantResult = new BigInteger(new byte[] { 11, 22, 33 });
            Assert.AreEqual(wantResult, ((Integer)item).GetInteger());
        }
    }
}
