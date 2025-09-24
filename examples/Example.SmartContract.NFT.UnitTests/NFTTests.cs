using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.NFT.UnitTests
{
    [TestClass]
    public class NFTTests : ContractProjectTestBase
    {
        public NFTTests()
            : base("../Example.SmartContract.NFT/Example.SmartContract.NFT.csproj")
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            EnsureContractDeployed();
        }

        [TestMethod]
        public void TestClaim()
        {
            EnsureContractDeployed();
            Contract.Claim(7772);
        }
    }
}
