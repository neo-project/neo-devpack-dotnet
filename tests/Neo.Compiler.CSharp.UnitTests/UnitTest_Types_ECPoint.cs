using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.SmartContract.Testing.TestingStandards;
using System.Text;

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

            Engine.StringEncoder = Encoding.ASCII;

            Assert.AreEqual("0247003f2e3f3f3f2c4f3f3f623f3f3f27253f5b4f3f3f3f7f3f533f3f3f3f633f", Encoding.ASCII.GetBytes(Contract.Ecpoint2String()!)?.ToHexString());
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.EcpointReturn()?.ToString());
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", (Contract.Ecpoint2ByteArray() as VM.Types.Buffer)!.GetSpan().ToHexString());
        }
    }
}
