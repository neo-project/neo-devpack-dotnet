using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Invoke
    {
        public UnitTest_Invoke()
        {
            var option = new Program.Options();
            option.File = "./TestClasses/Contract_InvokeCsNef.cs";
            Program.Compile(option);
        }

        [TestMethod]
        public void Test_Return_Integer()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./Contract_InvokeCsNef.nef");
            var result = testengine.GetMethod("returnInteger").Run();

            Integer wantresult = 42;
            Assert.IsTrue(wantresult.Equals(result));
        }

        [TestMethod]
        public void Test_Return_String()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./Contract_InvokeCsNef.nef");
            var result = testengine.GetMethod("returnString").Run();

            ByteString wantresult = "hello world";
            Assert.IsTrue(wantresult.Equals(result));
        }

        [TestMethod]
        public void Test_Main()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./Contract_InvokeCsNef.nef");
            var result = testengine.GetMethod("main").Run();

            Integer wantresult = 22;
            Assert.IsTrue(wantresult.Equals(result));
        }
    }
}
