using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.Interpreters;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types_ECPoint : TestBase<Contract_Types_ECPoint>
    {
        public UnitTest_Types_ECPoint() : base(Contract_Types_ECPoint.Nef, Contract_Types_ECPoint.Manifest) { }

        [TestMethod]
        public void ECPoint_test()
        {
            Assert.IsFalse(Contract.IsValid(InvalidECPoint.InvalidLength));
            Assert.IsFalse(Contract.IsValid(InvalidECPoint.InvalidType));
            Assert.ThrowsException<TestException>(() => Contract.IsValid(InvalidECPoint.Null));
            Assert.IsTrue(Contract.IsValid(Cryptography.ECC.ECPoint.Parse("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Cryptography.ECC.ECCurve.Secp256r1)));

            Engine.StringInterpreter = new HexStringInterpreter();

            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.Ecpoint2String());
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.EcpointReturn()?.ToString());
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", (Contract.Ecpoint2ByteArray() as VM.Types.Buffer)!.GetSpan().ToHexString());
        }
    }
}
