using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Switch
    {
        /// <summary>
        /// switch of more than 6 entries require a ComputeStringHash method
        /// </summary>
        [TestMethod]
        public void Test_SwitchLong()
        {
            EvaluationStack result;
            TestEngine testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_SwitchLong.cs");

            // Test cases

            for (int x = 0; x <= 20; x++)
            {
                testengine.Reset();
                result = testengine.ExecuteTestCaseStandard(x.ToString());
                Assert.AreEqual(result.Pop().GetBigInteger(), x + 1);
            }

            // Test default

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("default");
            Assert.AreEqual(result.Pop().GetBigInteger(), 99);
        }

        [TestMethod]
        public void Test_SwitchLong_Release()
        {
            EvaluationStack result;
            TestEngine testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_SwitchLong.cs", true);

            // Test cases

            for (int x = 0; x <= 20; x++)
            {
                testengine.Reset();
                result = testengine.ExecuteTestCaseStandard(x.ToString());
                Assert.AreEqual(result.Pop().GetBigInteger(), x + 1);
            }

            // Test default

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("default");
            Assert.AreEqual(result.Pop().GetBigInteger(), 99);
        }

        [TestMethod]
        public void Test_Switch6()
        {
            EvaluationStack result;
            TestEngine testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Switch6.cs");

            // Test cases

            for (int x = 0; x <= 5; x++)
            {
                testengine.Reset();
                result = testengine.ExecuteTestCaseStandard(x.ToString());
                Assert.AreEqual(result.Pop().GetBigInteger(), x + 1);
            }

            // Test default

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("default");
            Assert.AreEqual(result.Pop().GetBigInteger(), 99);
        }
    }
}
