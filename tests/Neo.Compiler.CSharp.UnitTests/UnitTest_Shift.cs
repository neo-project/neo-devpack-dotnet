using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Shift
    {
        [TestMethod]
        public void Test_Shift()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_shift.cs");
            //testengine.ScriptEntry.DumpNEF();
            var result = testengine.ExecuteTestCaseStandard("main");
            var list = ((VM.Types.Array)result.Pop()).Select(u => u.GetInteger()).ToList();

            CollectionAssert.AreEqual(new BigInteger[] { 16, 4 }, list);
        }

        [TestMethod]
        public void Test_Shift_BigInteger()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_shift_bigint.cs");
            var result = testengine.ExecuteTestCaseStandard("main");
            var list = ((VM.Types.Array)result.Pop()).Select(u => u.GetInteger()).ToList();

            CollectionAssert.AreEqual(new BigInteger[] { 8, 16, 4, 2 }, list);
        }
    }
}
