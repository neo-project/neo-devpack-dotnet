using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Returns
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript<Contract_Returns>();
        }

        [TestMethod]
        public void Test_OneReturn()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("subtract", 5, 9);

            Integer wantresult = -4;
            Assert.AreEqual(wantresult, result.Pop());
        }

        [TestMethod]
        public void Test_DoubleReturnA()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("div", 9, 5);

            Assert.AreEqual(1, result.Count);
            var array = result.Pop() as Array;
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(4, array[1]);
        }

        [TestMethod]
        public void Test_VoidReturn()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("sum", 9, 5);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, testengine.Notifications.Count);
            Assert.AreEqual(14, result.Pop().GetInteger());
        }

        [TestMethod]
        public void Test_DoubleReturnB()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("mix", 9, 5);

            Assert.AreEqual(1, result.Count);

            var r1 = result.Pop<Integer>();
            Assert.AreEqual(-3, r1);
        }

        [TestMethod]
        public void Test_ByteStringAdd()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("byteStringAdd", "hello", "world");

            Assert.AreEqual(1, result.Count);

            var r1 = result.Pop<ByteString>();
            Assert.AreEqual("helloworld", r1);
        }
    }
}
