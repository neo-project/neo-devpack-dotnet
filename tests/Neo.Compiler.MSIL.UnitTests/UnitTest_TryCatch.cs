using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using System;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_TryCatch
    {
        [TestMethod]
        public void Test_TryCatch_Succ()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("try01");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 3);
        }

        [TestMethod]
        public void Test_TryCatch_ThrowByCall()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("try03");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 4);
        }

        [TestMethod]
        public void Test_TryCatch_Throw()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("try02");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 4);
        }

        [TestMethod]
        public void Test_TryNest()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("tryNest");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 4);
        }

        [TestMethod]
        public void Test_TryFinally()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("tryFinally");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 3);
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("tryFinallyAndRethrow");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            Assert.AreEqual(testengine.State, VM.VMState.FAULT);
        }

        [TestMethod]
        public void Test_TryCatch()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("tryCatch");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 3);
        }

        [TestMethod]
        public void Test_TryWithTwoFinally()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("tryWithTwoFinally");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 9);
        }
    }
}
