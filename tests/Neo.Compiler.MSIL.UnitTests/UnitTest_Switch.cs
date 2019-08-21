using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_Switch
    {
        [TestMethod]
        public void Test_Switch()
        {
            TestEngine testengine;
            RandomAccessStack<StackItem> result;

            // Test cases

            for (int x = 0; x < 20; x++)
            {
                testengine = new TestEngine();
                testengine.AddEntryScript("./TestClasses/Contract_Switch.cs");

                result = testengine.ExecuteTestCaseStandard(x.ToString());
                Assert.AreEqual(result.Pop().GetBigInteger(), x + 1);
            }

            // Test default

            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Switch.cs");

            result = testengine.ExecuteTestCaseStandard("default");
            Assert.AreEqual(result.Pop().GetBigInteger(), 99);
        }
    }
}
