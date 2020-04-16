using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.SmartContract;
using System;
using System.Collections.Generic;
using System.Numerics;

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
            Console.WriteLine("result:" + value.Type + "  "+ value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.ToBigInteger().ToString());
            Assert.AreEqual(num.ToBigInteger(), 3);
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
            Console.WriteLine("result = " + num.ToBigInteger().ToString());
            Assert.AreEqual(num.ToBigInteger(), 4);
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
            Console.WriteLine("result = " + num.ToBigInteger().ToString());
            Assert.AreEqual(num.ToBigInteger(), 4);
        }

    }
}
