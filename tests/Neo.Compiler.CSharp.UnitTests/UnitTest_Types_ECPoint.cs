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
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.IsValid(InvalidECPoint.InvalidType));
            Assert.AreEqual(1048620, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.IsValid(InvalidECPoint.Null));
            Assert.AreEqual(1048110, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.IsValid(Cryptography.ECC.ECPoint.Parse("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Cryptography.ECC.ECCurve.Secp256r1)));
            Assert.AreEqual(1048830, Engine.FeeConsumed.Value);

            Engine.StringInterpreter = new HexStringInterpreter();

            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.Ecpoint2String());
            Assert.AreEqual(984870, Engine.FeeConsumed.Value);
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.EcpointReturn()?.ToString());
            Assert.AreEqual(984870, Engine.FeeConsumed.Value);
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", (Contract.Ecpoint2ByteArray() as VM.Types.Buffer)!.GetSpan().ToHexString());
            Assert.AreEqual(1230630, Engine.FeeConsumed.Value);
        }
    }
}
