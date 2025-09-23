using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.ContractCall.UnitTests
{
    [TestClass]
    public class ContractCallTests : ContractProjectTestBase
    {
        public ContractCallTests()
            : base("../Example.SmartContract.ContractCall/Example.SmartContract.ContractCall.csproj")
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            EnsureContractDeployed();
        }

        [TestMethod]
        public void ContractLoads()
        {
            EnsureContractDeployed();
        }
    }
}
