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
            Assert.AreEqual(1003063810, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestIndexOf()
        {
            Assert.AreEqual(4, Contract.TestIndexOf());
            Assert.AreEqual(1003062820, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestEndWith()
        {
            Assert.IsTrue(Contract.TestEndWith());
            Assert.AreEqual(1003064680, Engine.FeeConsumed.Value);
        }
        [TestMethod]
        public void TestContains()
        {
            Assert.IsTrue(Contract.TestContains());
            Assert.AreEqual(1003063810, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestNumberOnly()
        {
            Assert.IsTrue(Contract.TestNumberOnly());
            Assert.AreEqual(1002112390, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestAlphabetOnly()
        {
            Assert.IsTrue(Contract.TestAlphabetOnly());
            Assert.AreEqual(1002280210, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestLowerAlphabetOnly());
            Assert.AreEqual(1003391800, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.TestUpperAlphabetOnly());
            Assert.AreEqual(1004487010, Engine.FeeConsumed.Value);
        }
    }
}
