using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System.Text;

namespace Example.SmartContract.NFT.UnitTests
{
    [TestClass]
    public class NFTTests : TestBase<SampleLootNFT>
    {

        [TestMethod]
        public void TestClaim()
        {
            Contract.Claim(7772);

            var tokenId = Encoding.UTF8.GetBytes("7772");
            Assert.AreEqual(Engine.Transaction.Sender, Contract.OwnerOf(tokenId));
            Assert.IsNotNull(Contract.Properties(tokenId));
        }

        [TestMethod]
        public void TestClaimCannotMintTheSameTokenTwice()
        {
            Contract.Claim(7772);
            var tokenId = Encoding.UTF8.GetBytes("7772");
            var originalOwner = Contract.OwnerOf(tokenId);

            Assert.ThrowsException<TestException>(() => Contract.Claim(7772));
            Assert.AreEqual(originalOwner, Contract.OwnerOf(tokenId));
            Assert.IsNotNull(Contract.Properties(tokenId));
        }
    }
}
