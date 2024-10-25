using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types_BigInteger : DebugAndTestBase<Contract_Types_BigInteger>
    {
        [TestMethod]
        public void BigInteger_Test()
        {
            // Init

            Assert.AreEqual(BigInteger.Parse("100000000000000000000000000"), Contract.Attribute());
            AssertGasConsumed(984150);

            // static vars

            Assert.AreEqual(BigInteger.Zero, Contract.Zero());
            AssertGasConsumed(984060);
            Assert.AreEqual(BigInteger.One, Contract.One());
            AssertGasConsumed(984060);
            Assert.AreEqual(BigInteger.MinusOne, Contract.MinusOne());
            AssertGasConsumed(984060);

            // Parse

            Assert.AreEqual(456, Contract.Parse("456"));
            AssertGasConsumed(2032230);
            Assert.AreEqual(65, Contract.ConvertFromChar());
            AssertGasConsumed(984060);
        }
    }
}
