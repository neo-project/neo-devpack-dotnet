using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class CallbackTest
    {
        [TestMethod]
        public void Test_CreatePointer_Optimized()
        {
            // TODO: Require https://github.com/neo-project/neo-devpack-dotnet/pull/260/files
            Test_CreatePointer(true);
        }

        [TestMethod]
        public void Test_CreatePointer()
        {
            // TODO: Require https://github.com/neo-project/neo-vm/pull/317
            Test_CreatePointer(false);
        }

        public void Test_CreatePointer(bool optimized)
        {
            var engine = new TestEngine(TriggerType.Application);
            engine.AddEntryScript("./TestClasses/Contract_Callback.cs", true, optimized);

            var result = engine.ExecuteTestCaseStandard("createCallback");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Pointer));
            Assert.AreEqual(6, ((Pointer)item).Position);

            // Test pointer

            engine.Reset();
            engine.ExecuteTestCaseStandard(((Pointer)item).Position);
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, ((Integer)item).GetBigInteger());
        }
    }
}
