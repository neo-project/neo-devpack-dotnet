using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_BigInteger
    {
        [TestMethod]
        public void Test_Pow()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testPow", 2, 3);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(8, value);
        }

        [TestMethod]
        public void Test_Sqrt()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testSqrt", 4);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(2, value);
        }
    }
}
