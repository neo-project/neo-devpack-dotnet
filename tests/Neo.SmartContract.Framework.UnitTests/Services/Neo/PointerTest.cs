using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class PointerTest
    {
        [TestMethod]
        public void Test_CreatePointer_Optimized()
        {
            Test_CreatePointer(true);
        }

        [TestMethod]
        public void Test_CreatePointer()
        {
            Test_CreatePointer(false);
        }

        public void Test_CreatePointer(bool optimized)
        {
            var engine = new TestEngine(TriggerType.Application);
            engine.AddEntryScript("./TestClasses/Contract_Pointers.cs", true, optimized);

            var result = engine.ExecuteTestCaseStandard("createFuncPointer");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Pointer));
            Assert.AreEqual(optimized ? 6 : 8, ((Pointer)item).Position);

            // Test pointer

            engine.Reset();
            engine.ExecuteTestCaseStandard(((Pointer)item).Position);
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetBigInteger());
        }

        [TestMethod]
        public void Test_ExecutePointer_Optimized()
        {
            Test_ExecutePointer(true);
        }

        [TestMethod]
        public void Test_ExecutePointer()
        {
            Test_ExecutePointer(false);
        }

        public void Test_ExecutePointer(bool optimized)
        {
            var engine = new TestEngine(TriggerType.Application);
            engine.AddEntryScript("./TestClasses/Contract_Pointers.cs", true, optimized);

            var result = engine.ExecuteTestCaseStandard("callFuncPointer");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetBigInteger());
        }
    }
}
