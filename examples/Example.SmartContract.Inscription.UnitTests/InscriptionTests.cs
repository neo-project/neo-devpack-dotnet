using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.Inscription.UnitTests
{
    [TestClass]
    public class InscriptionTests : TestBase<SampleInscription>
    {
        [TestInitialize]
        public void TestSetup()
        {
            var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
            TestBaseSetup(nef, manifest);
        }

        [TestMethod]
        public void Test()
        {
            //var msg = "Hello";
            //Contract.AddInscription(UInt160.Zero, msg); //Need witness
            //Assert.AreEqual(Contract.GetInscription(UInt160.Zero), msg);
        }
    }
}
