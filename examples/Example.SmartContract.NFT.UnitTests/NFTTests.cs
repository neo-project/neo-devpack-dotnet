using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.NFT.UnitTests
{
    [TestClass]
    public class NFTTests : TestBase<SampleLootNFT>
    {
        public NFTTests() : base(SampleLootNFT.Nef, SampleLootNFT.Manifest)
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            TestCleanup.EnsureArtifactsUpToDateInternal();
        }

        [TestMethod]
        public void TestClaim()
        {
            Contract.Claim(7772);
        }
    }
}
