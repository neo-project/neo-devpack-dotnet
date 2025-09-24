using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.Modifier.UnitTests
{
    [TestClass]
    public class ModifierTests : ContractProjectTestBase
    {
        public ModifierTests()
            : base("../Example.SmartContract.Modifier/Example.SmartContract.Modifier.csproj")
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
