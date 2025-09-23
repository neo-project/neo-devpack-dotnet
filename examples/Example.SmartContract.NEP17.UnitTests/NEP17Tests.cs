using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.NEP17.UnitTests
{
    [TestClass]
    public class NEP17Tests : ContractProjectTestBase
    {
        public NEP17Tests()
            : base("../Example.SmartContract.NEP17/Example.SmartContract.NEP17.csproj")
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
            Assert.AreEqual(Contract.Symbol, "SampleNep17Token");
            Assert.AreEqual(Contract.Decimals, 8);
        }
    }
}
