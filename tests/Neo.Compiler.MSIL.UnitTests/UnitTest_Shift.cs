using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_Shift
    {
        [TestMethod]
        public void Test_Shift()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_shift.cs");

            testengine.ScriptEntry.DumpAVM();

            var result = testengine.ExecuteTestCaseStandard("testfunc");
        }
        [TestMethod]
        public void Test_Shift_BigInteger()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_shift_bigint.cs");

            testengine.ScriptEntry.DumpAVM();

            StackItem[] _params = new StackItem[] { "testfunc", new StackItem[0] };
            var result = testengine.ExecuteTestCase(_params);
        }
    }

}
