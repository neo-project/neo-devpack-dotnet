using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Returns
    {
        [TestMethod]
        public void Test_OneReturn()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Returns.cs");

            var result = testengine.GetMethod("subtract").Run(5, 9);

            StackItem wantresult = new byte[] { 14 };
            Assert.IsTrue(wantresult.Equals(result));
        }

        [TestMethod]
        public void Test_DoubleReturnA()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Returns.cs");

            var result = testengine.GetMethod("div").RunEx(9, 5);

            Assert.AreEqual(2, result.Count);

            Assert.IsTrue(result.TryPop(out Integer r1));
            Assert.AreEqual(4, r1);

            Assert.IsTrue(result.TryPop(out Integer r2));
            Assert.AreEqual(1, r2);
        }

        [TestMethod]
        public void Test_DoubleReturnB()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Returns.cs");

            var result = testengine.GetMethod("mix").RunEx(9, 5);

            Assert.AreEqual(1, result.Count);

            Assert.IsTrue(result.TryPop(out Integer r1));
            Assert.AreEqual(-4, r1);
        }
    }
}
