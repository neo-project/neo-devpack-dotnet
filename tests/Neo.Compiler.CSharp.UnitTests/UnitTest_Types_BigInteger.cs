using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types_BigInteger : TestBase2<Contract_Types_BigInteger>
    {
        [TestMethod]
        public void BigInteer_Test()
        {
            // static vars

            Assert.AreEqual(BigInteger.Zero, Contract.Zero());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.One, Contract.One());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.MinusOne, Contract.MinusOne());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);

            // Parse

            Assert.AreEqual(456, Contract.Parse("456"));
            Assert.AreEqual(2032230, Engine.FeeConsumed.Value);
            Assert.AreEqual(65, Contract.ConvertFromChar());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }
    }
}
