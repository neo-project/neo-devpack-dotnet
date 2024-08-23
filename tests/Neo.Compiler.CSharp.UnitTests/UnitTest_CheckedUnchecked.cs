using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CheckedUnchecked : DebugAndTestBase<Contract_CheckedUnchecked>
    {
        [TestMethod]
        public void TestAddChecked()
        {
            Assert.ThrowsException<TestException>(() => Contract.AddChecked(int.MaxValue, 1));
            AssertGasConsumed(1063020);
        }

        [TestMethod]
        public void TestAddUnchecked()
        {
            Assert.AreEqual(int.MinValue, Contract.AddUnchecked(int.MaxValue, 1));
            AssertGasConsumed(1048350);
        }

        [TestMethod]
        public void TestCastChecked()
        {
            Assert.ThrowsException<TestException>(() => Contract.CastChecked(-1));
            AssertGasConsumed(1062540);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(int.MinValue));
            AssertGasConsumed(1062540);

            Assert.AreEqual(2147483647, Contract.CastChecked(int.MaxValue));
            AssertGasConsumed(1047330);

            Assert.AreEqual(0, Contract.CastChecked(ulong.MinValue));
            AssertGasConsumed(1047330);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(ulong.MaxValue));
            AssertGasConsumed(1062780);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(long.MinValue));
            AssertGasConsumed(1062540);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(long.MaxValue));
            AssertGasConsumed(1062690);
        }

        [TestMethod]
        public void TestCastUnchecked()
        {
            Assert.AreEqual(uint.MaxValue, Contract.CastUnchecked(-1));
            AssertGasConsumed(1047510);


            Assert.AreEqual(2147483648, Contract.CastUnchecked(int.MinValue));
            AssertGasConsumed(1047510);

            Assert.AreEqual(2147483647, Contract.CastUnchecked(int.MaxValue));
            AssertGasConsumed(1047330);

            Assert.AreEqual(0, Contract.CastUnchecked(ulong.MinValue));
            AssertGasConsumed(1047330);

            Assert.AreEqual(4294967295, Contract.CastUnchecked(ulong.MaxValue));
            AssertGasConsumed(1047690);

            Assert.AreEqual(0, Contract.CastUnchecked(long.MinValue));
            AssertGasConsumed(1047510);

            Assert.AreEqual(4294967295, Contract.CastUnchecked(long.MaxValue));
            AssertGasConsumed(1047600);
        }
    }
}
