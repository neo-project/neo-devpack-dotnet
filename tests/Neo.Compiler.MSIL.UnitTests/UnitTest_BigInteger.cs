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
        public void Test_PowInt()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testPowInt", -2, 5);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(-32, value);
        }

        [TestMethod]
        public void Test_PowUInt()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testPowUInt", 3, 2);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(9, value);
        }

        [TestMethod]
        public void Test_PowLong()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testPowLong", 5, 2);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(25, value);
        }

        [TestMethod]
        public void Test_PowULong()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testPowULong", 5, 3);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(125, value);
        }

        [TestMethod]
        public void Test_PowBigInt()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testPowBigInteger", 2, 10);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(1024, value);
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

        [TestMethod]
        public void Test_SqrtInt()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testSqrtInt", 100);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(10, value);
        }

        [TestMethod]
        public void Test_SqrtUInt()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testSqrtUInt", 9);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(3, value);
        }

        [TestMethod]
        public void Test_SqrtLong()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testSqrtLong", 25);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(5, value);
        }

        [TestMethod]
        public void Test_SqrtULong()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
            var result = testengine.ExecuteTestCaseStandard("testSqrtULong", 36);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(6, value);
        }
    }
}
