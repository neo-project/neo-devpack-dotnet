using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Logical : TestBase<Contract_Logical>
    {
        public UnitTest_Logical() : base(Contract_Logical.Nef, Contract_Logical.Manifest) { }

        [TestMethod]
        public void Test_TestConditionalLogicalAnd()
        {
            foreach (var x in new bool[] { true, false })
                foreach (var y in new bool[] { true, false })
                {
                    var result = Contract.TestConditionalLogicalAnd(x, y);
                    Assert.AreEqual(x && y, result);
                    // TODO: 1047210
                    Assert.AreEqual(1047180, Engine.FeeConsumed.Value);
                }
        }

        [TestMethod]
        public void Test_TestConditionalLogicalOr()
        {
            foreach (var x in new bool[] { true, false })
                foreach (var y in new bool[] { true, false })
                {
                    var result = Contract.TestConditionalLogicalOr(x, y);
                    Assert.AreEqual(x || y, result);
                    // TODO: 1047210
                    Assert.AreEqual(1047150, Engine.FeeConsumed.Value);
                }
        }

        [TestMethod]
        public void Test_TestLogicalExclusiveOr()
        {
            foreach (var x in new bool[] { true, false })
                foreach (var y in new bool[] { true, false })
                {
                    var result = Contract.TestLogicalExclusiveOr(x, y);
                    Assert.AreEqual(x ^ y, result);
                    Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
                }
        }

        [TestMethod]
        public void Test_TestLogicalNegation()
        {
            var result = Contract.TestLogicalNegation(true);
            Assert.IsFalse(result);
            Assert.AreEqual(1047150, Engine.FeeConsumed.Value);
            result = Contract.TestLogicalNegation(false);
            Assert.IsTrue(result);
            Assert.AreEqual(1047150, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TestLogicalAnd()
        {
            for (byte x = 0; x < 255; x++)
                for (byte y = 0; y < 255; y++)
                {
                    var result = Contract.TestLogicalAnd(x, y);
                    Assert.AreEqual(x & y, result);
                    Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
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
                    Assert.AreEqual(1047360, Engine.FeeConsumed.Value);
                }
        }
    }
}
