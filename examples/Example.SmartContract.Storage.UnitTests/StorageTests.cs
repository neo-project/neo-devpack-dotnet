using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.Storage.UnitTests
{
    [TestClass]
    public class StorageTests : ContractProjectTestBase
    {
        public StorageTests()
            : base("../Example.SmartContract.Storage/Example.SmartContract.Storage.csproj")
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
