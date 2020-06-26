using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Invoke
    {
        [TestMethod]
        public void Test_Invoke()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Invoke.nef");
            var result = testengine.GetMethod("Main").Run();

            Integer wantresult = 42;
            Assert.IsTrue(wantresult.Equals(result));
        }

        [TestMethod]
        public void Test_InvokeCsNef()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_InvokeCsNef.nef");
            var result = testengine.GetMethod("returnInteger").Run();

            Integer wantresult = 42;
            Assert.IsTrue(wantresult.Equals(result));
        }
    }
}
