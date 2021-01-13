using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;
using System.Numerics;

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
            Assert.AreEqual(optimized ? 37 : 47, ((Pointer)item).Position);

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
            Assert.AreEqual(123, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void Test_ExecutePointerWithArgs_Optimized()
        {
            Test_ExecutePointerWithArgs(true);
        }

        [TestMethod]
        public void Test_ExecutePointerWithArgs()
        {
            Test_ExecutePointerWithArgs(false);
        }

        public void Test_ExecutePointerWithArgs(bool optimized)
        {
            var engine = new TestEngine(TriggerType.Application);
            engine.AddEntryScript("./TestClasses/Contract_Pointers.cs", true, optimized);

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
