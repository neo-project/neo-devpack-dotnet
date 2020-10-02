using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Struct
    {
        [TestMethod]
        public void Test_Struct1()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Struct.cs", releaseMode: true, false);
            var result = testengine.ExecuteTestCaseStandard("test1", 4, 5, 6);

            var item = result.Pop<Struct>();
            Assert.AreEqual(4, item[0]);
            Assert.AreEqual(5, item[1]);
            Assert.AreEqual(6, item[2]);
        }
    }
}
