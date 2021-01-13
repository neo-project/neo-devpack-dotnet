using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Invoke
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_InvokeCsNef.cs");
        }

        [TestMethod]
        public void Test_Return_Integer()
        {
            testengine.Reset();
            var result = testengine.GetMethod("returnInteger").Run();

            Integer wantresult = 42;
            Assert.IsTrue(wantresult.Equals(result));
        }

        [TestMethod]
        public void Test_Return_String()
        {
            testengine.Reset();
            var result = testengine.GetMethod("returnString").Run();

            ByteString wantresult = "hello world";
            Assert.IsTrue(wantresult.Equals(result));
        }

        [TestMethod]
        public void Test_Main()
        {
            testengine.Reset();
            var result = testengine.GetMethod("main").Run();

            Integer wantresult = 22;
            Assert.IsTrue(wantresult.Equals(result));
        }
    }
}
