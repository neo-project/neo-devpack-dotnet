using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Delegate
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Delegate.cs");
        }

        [TestMethod]
        public void TestFunc()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("sumFunc", 2, 3).Pop();
            Assert.AreEqual(5, result.GetInteger());
        }
    }
}
