using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.SampleRoyaltyNEP11Token.UnitTests
{
    [TestClass]
    public class SampleRoyaltyNEP11TokenTest : ContractProjectTestBase
    {
        public SampleRoyaltyNEP11TokenTest()
            : base("../Example.SmartContract.SampleRoyaltyNEP11Token/Example.SmartContract.SampleRoyaltyNEP11Token.csproj")
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            EnsureContractDeployed();
        }

        [TestMethod]
        public void Test()
        {
            EnsureContractDeployed();

        }
    }
}
