using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.NFT.UnitTests
{
    [TestClass]
    public class NFTTests : TestBase<SampleLootNFT>
    {

        [TestInitialize]
        public void TestSetup()
        {
            TestCleanup.EnsureArtifactsUpToDateInternal();
            TestBaseSetup(SampleLootNFT.Nef, SampleLootNFT.Manifest);
        }

        [TestMethod]
        public void TestClaim()
        {
            Contract.Claim(7772);
        }
    }
}
