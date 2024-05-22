using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inline : TestBase<Contract_Inline>
    {
        public UnitTest_Inline() : base(Contract_Inline.Nef, Contract_Inline.Manifest) { }

        [TestMethod]
        public void Test_Inline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("inline"));
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_with_one_parameters"));
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("inline_with_multi_parameters"));
        }

        [TestMethod]
        public void Test_NoInline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("not_inline"));
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("not_inline_with_one_parameters"));
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("not_inline_with_multi_parameters"));
        }

        [TestMethod]
        public void Test_NestedInline()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_nested"));
        }
    }
}
