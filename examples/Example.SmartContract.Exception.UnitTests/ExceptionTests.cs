using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.Exception.UnitTests
{
    [TestClass]
    public class ExceptionTests : ContractProjectTestBase
    {
        public ExceptionTests()
            : base("../Example.SmartContract.Exception/Example.SmartContract.Exception.csproj")
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            EnsureContractDeployed();
        }

        [TestMethod]
        public void TryBlocks_DoNotThrow()
        {
            EnsureContractDeployed();
            Contract.Try01();
            Contract.Try02();
            Contract.Try03();
            Contract.TryFinally();
            Contract.TryNest();
        }
    }
}
