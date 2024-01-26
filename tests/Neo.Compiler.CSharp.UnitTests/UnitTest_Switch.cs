using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
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
            using var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract_SwitchLong));

            // Test cases

            for (int x = 0; x <= 20; x++)
            {
                testengine.Reset();
                result = testengine.ExecuteTestCaseStandard("testMain", x.ToString());
                Assert.AreEqual(result.Pop().GetInteger(), x + 1);
            }

            // Test default

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testMain", 21.ToString());
            Assert.AreEqual(result.Pop().GetInteger(), 99);
        }

        [TestMethod]
        public void Test_SwitchLongLong()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract_SwitchLongLong));

            var resulta = testengine.ExecuteTestCaseStandard("testMain", "a").Pop();
            var awant = 2;
            testengine.Reset();
            var resultb = testengine.ExecuteTestCaseStandard("testMain", "b").Pop();
            var bwant = 0;
            testengine.Reset();
            var resultc = testengine.ExecuteTestCaseStandard("testMain", "c").Pop();
            var cwant = 2;
            testengine.Reset();
            var resultd = testengine.ExecuteTestCaseStandard("testMain", "d").Pop();
            var dwant = -1;
            testengine.Reset();
            var resulte = testengine.ExecuteTestCaseStandard("testMain", "e").Pop();
            var ewant = 1;
            testengine.Reset();
            var resultf = testengine.ExecuteTestCaseStandard("testMain", "f").Pop();
            var fwant = 3;
            testengine.Reset();
            var resultg = testengine.ExecuteTestCaseStandard("testMain", "g").Pop();
            var gwant = 3;

            // Test default

            Assert.AreEqual(resulta.GetInteger(), awant);
            Assert.AreEqual(resultb.GetInteger(), bwant);
            Assert.AreEqual(resultc.GetInteger(), cwant);
            Assert.AreEqual(resultd.GetInteger(), dwant);
            Assert.AreEqual(resulte.GetInteger(), ewant);
            Assert.AreEqual(resultf.GetInteger(), fwant);
            Assert.AreEqual(resultg.GetInteger(), gwant);
        }

        [TestMethod]
        public void Test_SwitchInteger()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract_SwitchInteger));

            var result1 = testengine.ExecuteTestCaseStandard("testMain", 1).Pop();
            var onewant = 2;
            testengine.Reset();
            var result2 = testengine.ExecuteTestCaseStandard("testMain", 2).Pop();
            var twowant = 3;
            testengine.Reset();
            var result3 = testengine.ExecuteTestCaseStandard("testMain", 3).Pop();
            var threewant = 6;
            testengine.Reset();
            var result0 = testengine.ExecuteTestCaseStandard("testMain", 0).Pop();
            var zerowant = 0;

            // Test default

            Assert.AreEqual(result1.GetInteger(), onewant);
            Assert.AreEqual(result2.GetInteger(), twowant);
            Assert.AreEqual(result3.GetInteger(), threewant);
            Assert.AreEqual(result0.GetInteger(), zerowant);
        }

        [TestMethod]
        public void Test_SwitchLong_Release()
        {
            EvaluationStack result;
            using var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract_SwitchLong));

            // Test cases

            for (int x = 0; x <= 20; x++)
            {
                testengine.Reset();
                result = testengine.ExecuteTestCaseStandard("testMain", x.ToString());
                Assert.AreEqual(result.Pop().GetInteger(), x + 1);
            }

            // Test default

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testMain", 21.ToString());
            Assert.AreEqual(result.Pop().GetInteger(), 99);
        }

        [TestMethod]
        public void Test_Switch6()
        {
            EvaluationStack result;
            using var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract_Switch6));

            // Test cases

            for (int x = 0; x <= 5; x++)
            {
                testengine.Reset();
                result = testengine.ExecuteTestCaseStandard("testMain", x.ToString());
                Assert.AreEqual(result.Pop().GetInteger(), x + 1);
                testengine.Reset();
                result = testengine.ExecuteTestCaseStandard("main2", x.ToString());
                Assert.AreEqual(result.Pop().GetInteger(), x + 1);
            }

            // Test default

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testMain", 6.ToString());
            Assert.AreEqual(result.Pop().GetInteger(), 99);
            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("main2", 6.ToString());
            Assert.AreEqual(result.Pop().GetInteger(), 99);
        }
    }
}
