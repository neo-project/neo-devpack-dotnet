using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography.ECC;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM.Types;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class NativeTest : DebugAndTestBase<Contract_Native>
    {
        [TestMethod]
        public void Test_NEO()
        {
            Assert.AreEqual(0, Contract.NEO_Decimals());
            Assert.AreEqual(5_0000_0000, Contract.NEO_GetGasPerBlock());
            Assert.IsNull(Contract.NEO_GetAccountState(Bob.Account));
            Assert.AreEqual(100_000_000, Contract.NEO_BalanceOf(Engine.ValidatorsAddress));
            Assert.AreEqual(0, Contract.NEO_UnclaimedGas(Bob.Account, 0));
            Assert.IsFalse(Contract.NEO_Transfer(Bob.Account, Bob.Account, 0));

            // Before RegisterCandidate
            Assert.AreEqual(0, Contract.NEO_GetCandidates()?.Count);
            // RegisterCandidate
            Engine.Fee = 1005_0000_0000;
            var pubKey = ECPoint.Parse("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c", ECCurve.Secp256r1);
            Engine.SetTransactionSigners(WitnessScope.Global, pubKey);
            Assert.IsTrue(Contract.NEO_RegisterCandidate(pubKey));
            // After RegisterCandidate
            Assert.AreEqual(1, Contract.NEO_GetCandidates()?.Count);
            Assert.AreEqual(pubKey, ((Testing.Native.Models.Candidate)Contract.NEO_GetCandidates()?
                .Cast<StackItem>().First().ConvertTo(typeof(Testing.Native.Models.Candidate))!).PublicKey);
        }

        [TestMethod]
        public void Test_GAS()
        {
            Assert.AreEqual(8, Contract.GAS_Decimals());
        }

        [TestMethod]
        public void Test_Policy()
        {
            Assert.AreEqual(1000L, Contract.Policy_GetFeePerByte());
            Assert.IsFalse(Contract.Policy_IsBlocked(Alice.Account));
        }
    }
}
