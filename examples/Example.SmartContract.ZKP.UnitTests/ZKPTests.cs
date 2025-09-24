using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.ZKP.UnitTests
{
    [TestClass]
    public class ZKPTests : ContractProjectTestBase
    {
        public ZKPTests()
            : base("../Example.SmartContract.ZKP/Example.SmartContract.ZKP.csproj")
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
