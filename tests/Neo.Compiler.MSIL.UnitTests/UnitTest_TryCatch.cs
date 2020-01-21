using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
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
        public void Test_TryCatch()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("testfunc");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  "+ value.ToString());

        }


    }
}
