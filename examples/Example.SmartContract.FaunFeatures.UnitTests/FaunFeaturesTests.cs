using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Example.SmartContract.FaunFeatures.UnitTests
{
    [TestClass]
    public class FaunFeaturesTests : TestBase<SampleFaunFeatures>
    {
        [TestInitialize]
        public void TestSetup()
        {
            var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
            TestBaseSetup(nef, manifest);
        }

        [TestMethod]
        public void TestHexEncodeDecode()
        {
            byte[] data = { 0x01, 0xab, 0xcd };
            var encoded = Contract.HexEncode(data);
            Assert.AreEqual("01abcd", encoded);
            var decoded = Contract.HexDecode(encoded);
            CollectionAssert.AreEqual(data, decoded);
        }

        [TestMethod]
        public void TestExecFeeFactors()
        {
            var execFeeFactor = Contract.ExecFeeFactor ?? BigInteger.Zero;
            Assert.IsTrue(execFeeFactor > 0);
        }
    }
}
