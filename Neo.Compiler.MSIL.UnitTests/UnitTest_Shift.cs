using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;

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
            testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("testfunc");
        }
    }
}
