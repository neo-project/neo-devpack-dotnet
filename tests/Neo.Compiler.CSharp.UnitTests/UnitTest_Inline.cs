using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inline : DebugAndTestBase<Contract_Inline>
    {
        [TestMethod]
        public void Test_Inline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("inline"));
            AssertGasConsumed(1048710);
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_with_one_parameters"));
            AssertGasConsumed(1050030);
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("inline_with_multi_parameters"));
            AssertGasConsumed(1051920);
        }

        [TestMethod]
        public void Test_NoInline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("not_inline"));
            AssertGasConsumed(1068030);
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("not_inline_with_one_parameters"));
            AssertGasConsumed(1071330);
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("not_inline_with_multi_parameters"));
            AssertGasConsumed(1073280);
        }

        [TestMethod]
        public void Test_NestedInline()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_nested"));
            AssertGasConsumed(1071990);
        }
    }
}
