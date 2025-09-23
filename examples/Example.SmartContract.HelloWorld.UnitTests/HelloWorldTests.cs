using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.HelloWorld.UnitTests
{
    [TestClass]
    public class HelloWorldTests : ContractProjectTestBase
    {
        public HelloWorldTests()
            : base("../Example.SmartContract.HelloWorld/Example.SmartContract.HelloWorld.csproj")
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            EnsureContractDeployed();
        }

        [TestMethod]
        public void SayHello_ReturnsExpectedGreeting()
        {
            EnsureContractDeployed();
            Assert.AreEqual("Hello, World!", Contract.SayHello);
        }
    }
}
