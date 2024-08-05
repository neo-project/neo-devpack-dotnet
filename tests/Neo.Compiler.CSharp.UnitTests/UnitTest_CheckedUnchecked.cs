using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CheckedUnchecked : TestBase<Contract_CheckedUnchecked>
    {
        [TestMethod]
        public void TestAddChecked()
        {
            Assert.ThrowsException<TestException>(() => Contract.AddChecked(int.MaxValue, 1));
            Assert.AreEqual(1063020, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestAddUnchecked()
        {
            Assert.AreEqual(int.MinValue, Contract.AddUnchecked(int.MaxValue, 1));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCastChecked()
        {
            Assert.ThrowsException<TestException>(() => Contract.CastChecked(-1));
            Assert.AreEqual(1062540, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(int.MinValue));
            Assert.AreEqual(1062540, Engine.FeeConsumed.Value);

            Assert.AreEqual(2147483647, Contract.CastChecked(int.MaxValue));
            Assert.AreEqual(1047330, Engine.FeeConsumed.Value);

            Assert.AreEqual(0, Contract.CastChecked(ulong.MinValue));
            Assert.AreEqual(1047330, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(ulong.MaxValue));
            Assert.AreEqual(1062780, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(long.MinValue));
            Assert.AreEqual(1062540, Engine.FeeConsumed.Value);

            Assert.ThrowsException<TestException>(() => Contract.CastChecked(long.MaxValue));
            Assert.AreEqual(1062690, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestCastUnchecked()
        {
            Assert.AreEqual(uint.MaxValue, Contract.CastUnchecked(-1));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);


            Assert.AreEqual(2147483648, Contract.CastUnchecked(int.MinValue));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);

            Assert.AreEqual(2147483647, Contract.CastUnchecked(int.MaxValue));
            Assert.AreEqual(1047330, Engine.FeeConsumed.Value);

            Assert.AreEqual(0, Contract.CastUnchecked(ulong.MinValue));
            Assert.AreEqual(1047330, Engine.FeeConsumed.Value);

            Assert.AreEqual(4294967295, Contract.CastUnchecked(ulong.MaxValue));
            Assert.AreEqual(1047690, Engine.FeeConsumed.Value);

            Assert.AreEqual(0, Contract.CastUnchecked(long.MinValue));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);

            Assert.AreEqual(4294967295, Contract.CastUnchecked(long.MaxValue));
            Assert.AreEqual(1047600, Engine.FeeConsumed.Value);
        }
    }
}
