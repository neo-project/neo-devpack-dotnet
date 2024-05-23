using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Math : TestBase<Contract_Math>
    {
        public UnitTest_Math() : base(Contract_Math.Nef, Contract_Math.Manifest) { }

        [TestMethod]
        public void max_test()
        {
            Assert.AreEqual(2, Contract.Max(1, 2));
            Assert.AreEqual(3, Contract.Max(3, 1));
        }

        [TestMethod]
        public void min_test()
        {
            Assert.AreEqual(1, Contract.Min(1, 2));
            Assert.AreEqual(1, Contract.Min(3, 1));
        }

        [TestMethod]
        public void sign_test()
        {
            Assert.AreEqual(1, Contract.Sign(1));
            Assert.AreEqual(-1, Contract.Sign(-1));
            Assert.AreEqual(0, Contract.Sign(0));
        }

        [TestMethod]
        public void abs_test()
        {
            Assert.AreEqual(1, Contract.Abs(1));
            Assert.AreEqual(1, Contract.Abs(-1));
            Assert.AreEqual(0, Contract.Abs(0));
        }
    }
}
