using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.Event.UnitTests
{
    [TestClass]
    public class EventTests : ContractProjectTestBase
    {
        public EventTests()
            : base("../Example.SmartContract.Event/Example.SmartContract.Event.csproj")
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            EnsureContractDeployed();
        }

        [TestMethod]
        public void Main_ReturnsFalse()
        {
            EnsureContractDeployed();
            Assert.IsFalse(Contract.Main());
        }
    }
}
