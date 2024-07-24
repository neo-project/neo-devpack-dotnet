using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StringTest : TestBase<Contract_String>
    {
        public StringTest() : base(Contract_String.Nef, Contract_String.Manifest) { }

        [TestMethod]
        public void TestStringAdd()
        {
            // ab => 3
            Assert.AreEqual(3, Contract.TestStringAdd("a", "b"));
            Assert.AreEqual(1357650, Engine.FeeConsumed.Value);

            // hello => 4
            Assert.AreEqual(4, Contract.TestStringAdd("he", "llo"));
            Assert.AreEqual(1356600, Engine.FeeConsumed.Value);

            // world => 5
            Assert.AreEqual(5, Contract.TestStringAdd("wo", "rld"));
            Assert.AreEqual(1357860, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestStringAddInt()
        {
            Assert.AreEqual("Neo3", Contract.TestStringAddInt("Neo", 3));
            Assert.AreEqual(2460540, Engine.FeeConsumed.Value);
        }
    }
}
