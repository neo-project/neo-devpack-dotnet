using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.Modifier.UnitTests
{
    [TestClass]
    public class ModifierTests : TestBase<SampleModifier>
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
