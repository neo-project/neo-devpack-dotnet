using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class RegexTest : TestBase<Contract_Regex>
    {
        public RegexTest() : base(Contract_Regex.Nef, Contract_Regex.Manifest) { }

        [TestMethod]
        public void TestStartWith()
        {
            Assert.IsTrue(Contract.TestStartWith());
        }

        [TestMethod]
        public void TestIndexOf()
        {
            Assert.AreEqual(4, Contract.TestIndexOf());
        }

        [TestMethod]
        public void TestEndWith()
        {
            Assert.IsTrue(Contract.TestEndWith());
        }
        [TestMethod]
        public void TestContains()
        {
            Assert.IsTrue(Contract.TestContains());
        }

        [TestMethod]
        public void TestNumberOnly()
        {
            Assert.IsTrue(Contract.TestNumberOnly());
        }

        [TestMethod]
        public void TestAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestAlphabetOnly());
            Assert.IsTrue(Contract.TestLowerAlphabetOnly());
            Assert.IsTrue(Contract.TestUpperAlphabetOnly());
        }
    }
}
