using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using System;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_Switch
    {
        /// <summary>
        /// switch of more than 6 entries require a ComputeStringHash method
        /// </summary>
        [TestMethod]
        public void Test_InvalidSwitch()
        {
            var testengine = new TestEngine();
            Assert.ThrowsException<Exception>(() => testengine.AddEntryScript("./TestClasses/Contract_SwitchInvalid.cs"));
        }

        [TestMethod]
        public void Test_ValidSwitch()
        {
            TestEngine testengine;
            EvaluationStack result;

            // Test cases

            for (int x = 0; x <= 5; x++)
            {
                testengine = new TestEngine();
                testengine.AddEntryScript("./TestClasses/Contract_SwitchValid.cs");

                result = testengine.ExecuteTestCaseStandard(x.ToString());
                Assert.AreEqual(result.Pop().GetBigInteger(), x + 1);
            }

            // Test default

            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_SwitchValid.cs");

            result = testengine.ExecuteTestCaseStandard("default");
            Assert.AreEqual(result.Pop().GetBigInteger(), 99);
        }
    }
}
