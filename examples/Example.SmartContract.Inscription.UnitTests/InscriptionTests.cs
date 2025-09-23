using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.Inscription.UnitTests
{
    [TestClass]
    public class InscriptionTests : ContractProjectTestBase
    {
        public InscriptionTests()
            : base("../Example.SmartContract.Inscription/Example.SmartContract.Inscription.csproj")
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
            //var msg = "Hello";
            //Contract.AddInscription(UInt160.Zero, msg); //Need witness
            //Assert.AreEqual(Contract.GetInscription(UInt160.Zero), msg);
        }
    }
}
