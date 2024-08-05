using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.Wallets;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_UIntTypes : TestBase<Contract_UIntTypes>
    {
        [TestMethod]
        public void UInt160_ValidateAddress()
        {
            var address = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            // True

            Assert.IsTrue(Contract.ValidateAddress(address));
            Assert.AreEqual(1049340, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.InvalidType));
            Assert.AreEqual(1048770, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.ValidateAddress(InvalidUInt160.Null));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.InvalidType));
            Assert.AreEqual(1048770, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.InvalidLength));
            Assert.AreEqual(1048980, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void UInt160_equals_test()
        {
            var owner = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);
            var notOwner = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            Assert.IsTrue(Contract.CheckOwner(owner));
            Assert.AreEqual(1049040, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.CheckOwner(notOwner));
            Assert.AreEqual(1049040, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void UInt160_equals_zero_test()
        {
            var zero = UInt160.Zero;
            var notZero = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            Assert.IsTrue(Contract.CheckZeroStatic(zero));
            Assert.AreEqual(1049220, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.CheckZeroStatic(notZero));
            Assert.AreEqual(1049220, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void UInt160_byte_array_construct()
        {
            var notZero = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            Assert.AreEqual(notZero, Contract.ConstructUInt160(notZero.ToArray()));
            Assert.AreEqual(1294230, Engine.FeeConsumed.Value);
        }
    }
}
