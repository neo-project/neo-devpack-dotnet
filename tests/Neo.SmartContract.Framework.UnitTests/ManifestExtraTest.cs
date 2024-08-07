using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class ManifestExtraTest
    {
        public ManifestExtraTest()
        {
            // Ensure also Contract_ExtraAttribute

            TestCleanup.TestInitialize(typeof(Contract_ExtraAttribute));
        }

        [TestMethod]
        public void TestExtraAttribute()
        {
            var extra = Contract_ExtraAttribute.Manifest.Extra;

            Assert.AreEqual("Neo", extra["Author"]?.GetString());
            Assert.AreEqual("dev@neo.org", extra["E-mail"]?.GetString());
        }
    }
}
