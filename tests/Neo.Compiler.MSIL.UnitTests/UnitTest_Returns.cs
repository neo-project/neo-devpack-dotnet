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

            Integer wantresult = -4;
            Assert.IsTrue(wantresult.Equals(result));
        }

        [TestMethod]
        public void Test_DoubleReturnA()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Returns.cs");

            var result = testengine.GetMethod("div").RunEx(9, 5);

            Assert.AreEqual(1, result.Count);
            var array = result.Pop() as Array;
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(4, array[1]);
        }

        [TestMethod]
        public void Test_DoubleReturnB()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Returns.cs");

            var result = testengine.GetMethod("mix").RunEx(9, 5);

            Assert.AreEqual(1, result.Count);

            var r1 = result.Pop<Integer>();
            Assert.AreEqual(-3, r1);
        }
    }
}
