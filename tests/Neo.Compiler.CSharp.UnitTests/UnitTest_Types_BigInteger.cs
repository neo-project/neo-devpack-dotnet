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
            Assert.AreEqual(BigInteger.One, Contract.One());
            Assert.AreEqual(BigInteger.MinusOne, Contract.MinusOne());

            // Parse

            Assert.AreEqual(456, Contract.Parse("456"));
            Assert.AreEqual(65, Contract.ConvertFromChar());
        }
    }
}
