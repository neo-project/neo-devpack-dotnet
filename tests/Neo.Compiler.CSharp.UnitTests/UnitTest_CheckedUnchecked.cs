using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CheckedUnchecked : TestBase<Contract_CheckedUnchecked>
    {
        public UnitTest_CheckedUnchecked() : base(Contract_CheckedUnchecked.Nef, Contract_CheckedUnchecked.Manifest) { }

        [TestMethod]
        public void TestAddChecked()
        {
            Assert.ThrowsException<TestException>(() => Contract.AddChecked(int.MaxValue, 1));
            Assert.AreEqual(1002123700, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestAddUnchecked()
        {
            Assert.AreEqual(int.MinValue, Contract.AddUnchecked(int.MaxValue, 1));
            Assert.AreEqual(1002109090, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCastChecked()
        {
            Assert.ThrowsException<TestException>(() => Contract.CastChecked(-1));
            Assert.AreEqual(1002123220, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCastUnchecked()
        {
            Assert.AreEqual(uint.MaxValue, Contract.CastUnchecked(-1));
            Assert.AreEqual(1002108250, Engine.FeeConsumed.Value);
        }
    }
}
