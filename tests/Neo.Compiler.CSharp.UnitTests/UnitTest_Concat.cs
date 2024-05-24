using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Concat : TestBase<Contract_Concat>
    {
        public UnitTest_Concat() : base(Contract_Concat.Nef, Contract_Concat.Manifest) { }

        [TestMethod]
        public void TestStringAdd1()
        {
            Assert.AreEqual("ahello", Contract.TestStringAdd1("a"));
        }

        [TestMethod]
        public void TestStringAdd2()
        {
            Assert.AreEqual("abhello", Contract.TestStringAdd2("a", "b"));
        }

        [TestMethod]
        public void TestStringAdd3()
        {
            Assert.AreEqual("abchello", Contract.TestStringAdd3("a", "b", "c"));
        }

        [TestMethod]
        public void TestStringAdd4()
        {
            Assert.AreEqual("abcdhello", Contract.TestStringAdd4("a", "b", "c", "d"));
        }
    }
}
