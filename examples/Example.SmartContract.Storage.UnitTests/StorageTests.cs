using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.Storage.UnitTests
{
    [TestClass]
    public class StorageTests : TestBase<SampleStorage>
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

        }
    }
}
