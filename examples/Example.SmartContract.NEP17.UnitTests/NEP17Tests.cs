using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.NEP17.UnitTests
{
    [TestClass]
    public class NEP17Tests : TestBase<SampleNep17Token>
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
            Assert.AreEqual(Contract.Symbol, "SampleNep17Token");
            Assert.AreEqual(Contract.Decimals, 8);
        }
    }
}
