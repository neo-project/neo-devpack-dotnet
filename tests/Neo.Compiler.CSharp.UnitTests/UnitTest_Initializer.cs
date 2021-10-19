using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Initializer
    {
        [TestMethod]
        public void Initializer_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Initializer.cs");

            var result = testengine.ExecuteTestCaseStandard("sum");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(3, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("sum1", 5, 7);

            value = result.Pop().GetInteger();
            Assert.AreEqual(12, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("sum2", 5, 7);

            value = result.Pop().GetInteger();
            Assert.AreEqual(12, value);
        }
    }
}
