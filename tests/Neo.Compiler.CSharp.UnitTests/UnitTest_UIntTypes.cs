using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.Wallets;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_UIntTypes : DebugAndTestBase<Contract_UIntTypes>
    {
        [TestMethod]
        public void UInt160_ValidateAddress()
        {
            var address = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            // True

            Assert.IsTrue(Contract.ValidateAddress(address));
            AssertGasConsumed(1048050);

            // False

            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.InvalidType));
            AssertGasConsumed(1047450);
            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.Null));
            AssertGasConsumed(1047450);
            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.InvalidType));
            AssertGasConsumed(1047450);
            Assert.IsFalse(Contract.ValidateAddress(InvalidUInt160.InvalidLength));
            AssertGasConsumed(1047900);
        }

        [TestMethod]
        public void UInt160_equals_test()
        {
            var owner = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);
            var notOwner = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            Assert.IsTrue(Contract.CheckOwner(owner));
            AssertGasConsumed(1048440);
            Assert.IsFalse(Contract.CheckOwner(notOwner));
            AssertGasConsumed(1048440);
        }

        [TestMethod]
        public void UInt160_equals_zero_test()
        {
            var zero = UInt160.Zero;
            var notZero = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            Assert.IsTrue(Contract.CheckZeroStatic(zero));
            AssertGasConsumed(1048440);
            Assert.IsFalse(Contract.CheckZeroStatic(notZero));
            AssertGasConsumed(1048440);
        }

        [TestMethod]
        public void UInt160_byte_array_construct()
        {
            var notZero = "NYjzhdekseMYWvYpSoAeypqMiwMuEUDhKB".ToScriptHash(ProtocolSettings.Default.AddressVersion);

            Assert.AreEqual(notZero, Contract.ConstructUInt160(notZero.ToArray()));
            AssertGasConsumed(1293450);
        }
    }
}
