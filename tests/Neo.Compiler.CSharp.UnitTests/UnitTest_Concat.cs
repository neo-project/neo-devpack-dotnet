using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Concat : DebugAndTestBase<Contract_Concat>
    {
        [TestMethod]
        public void TestStringAdd1()
        {
            Assert.AreEqual("ahello", Contract.TestStringAdd1("a"));
            Assert.AreEqual(1354680, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestStringAdd2()
        {
            Assert.AreEqual("abhello", Contract.TestStringAdd2("a", "b"));
            Assert.AreEqual(1662180, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestStringAdd3()
        {
            Assert.AreEqual("abchello", Contract.TestStringAdd3("a", "b", "c"));
            Assert.AreEqual(1969680, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestStringAdd4()
        {
            Assert.AreEqual("abcdhello", Contract.TestStringAdd4("a", "b", "c", "d"));
            Assert.AreEqual(2277180, Engine.FeeConsumed.Value);
        }
    }
}
