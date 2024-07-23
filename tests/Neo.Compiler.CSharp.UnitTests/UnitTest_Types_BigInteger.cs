using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types_BigInteger : TestBase<Contract_Types_BigInteger>
    {
        public UnitTest_Types_BigInteger() : base(Contract_Types_BigInteger.Nef, Contract_Types_BigInteger.Manifest) { }

        [TestMethod]
        public void BigInteer_Test()
        {
            // static vars

            Assert.AreEqual(BigInteger.Zero, Contract.Zero());
            Assert.AreEqual(1002044800, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.One, Contract.One());
            Assert.AreEqual(1003028920, Engine.FeeConsumed.Value);
            Assert.AreEqual(BigInteger.MinusOne, Contract.MinusOne());
            Assert.AreEqual(1004013040, Engine.FeeConsumed.Value);

            // Parse

            Assert.AreEqual(456, Contract.Parse("456"));
            Assert.AreEqual(1006045330, Engine.FeeConsumed.Value);
            Assert.AreEqual(65, Contract.ConvertFromChar());
            Assert.AreEqual(1007029450, Engine.FeeConsumed.Value);
        }
    }
}
