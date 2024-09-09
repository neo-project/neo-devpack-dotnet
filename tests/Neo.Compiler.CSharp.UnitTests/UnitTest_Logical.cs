using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Logical : DebugAndTestBase<Contract_Logical>
    {
        [TestMethod]
        public void Test_TestConditionalLogicalAnd()
        {
            var result = Contract.TestConditionalLogicalAnd(true, true);
            Assert.AreEqual(true && true, result);
            AssertGasConsumed(1047180);

            result = Contract.TestConditionalLogicalAnd(true, false);
            Assert.AreEqual(true && false, result);
            AssertGasConsumed(1047180);

            result = Contract.TestConditionalLogicalAnd(false, true);
            Assert.AreEqual(false && true, result);
            AssertGasConsumed(1047150);

            result = Contract.TestConditionalLogicalAnd(false, false);
            Assert.AreEqual(false && false, result);
            AssertGasConsumed(1047150);
        }

        [TestMethod]
        public void Test_TestConditionalLogicalOr()
        {
            var result = Contract.TestConditionalLogicalOr(true, true);
            Assert.AreEqual(true || true, result);
            AssertGasConsumed(1047150);

            result = Contract.TestConditionalLogicalOr(true, false);
            Assert.AreEqual(true || false, result);
            AssertGasConsumed(1047150);

            result = Contract.TestConditionalLogicalOr(false, true);
            Assert.AreEqual(false || true, result);
            AssertGasConsumed(1047180);

            result = Contract.TestConditionalLogicalOr(false, false);
            Assert.AreEqual(false || false, result);
            AssertGasConsumed(1047180);
        }

        [TestMethod]
        public void Test_TestLogicalExclusiveOr()
        {
            foreach (var x in new bool[] { true, false })
                foreach (var y in new bool[] { true, false })
                {
                    var result = Contract.TestLogicalExclusiveOr(x, y);
                    Assert.AreEqual(x ^ y, result);
                    AssertGasConsumed(1047360);
                }
        }

        [TestMethod]
        public void Test_TestLogicalNegation()
        {
            var result = Contract.TestLogicalNegation(true);
            Assert.IsFalse(result);
            AssertGasConsumed(1047150);
            result = Contract.TestLogicalNegation(false);
            Assert.IsTrue(result);
            AssertGasConsumed(1047150);
        }

        [TestMethod]
        public void Test_TestLogicalAnd()
        {
            for (byte x = 0; x < 255; x++)
                for (byte y = 0; y < 255; y++)
                {
                    var result = Contract.TestLogicalAnd(x, y);
                    Assert.AreEqual(x & y, result);
                    AssertGasConsumed(1047360);
                }
        }

        [TestMethod]
        public void Test_TestLogicalOr()
        {
            for (byte x = 0; x < 255; x++)
                for (byte y = 0; y < 255; y++)
                {
                    var result = Contract.TestLogicalOr(x, y);
                    Assert.AreEqual(x | y, result);
                    AssertGasConsumed(1047360);
                }
        }
    }
}
