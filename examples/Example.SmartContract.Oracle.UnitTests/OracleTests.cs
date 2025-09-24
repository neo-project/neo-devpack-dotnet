using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.Oracle.UnitTests
{
    [TestClass]
    public class OracleTests : ContractProjectTestBase
    {
        public OracleTests()
            : base("../Example.SmartContract.Oracle/Example.SmartContract.Oracle.csproj")
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
