using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework.UnitTests.TestClasses;
using Neo.SmartContract.Framework.UnitTests.Utils;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class ManifestExtraTest
    {
        [TestMethod]
        public void TestExtraAttribute()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript(typeof(Contract_ExtraAttribute));

            var extra = testengine.Manifest.Extra;

            Assert.AreEqual("Neo", extra["Author"].GetString());
            Assert.AreEqual("dev@neo.org", extra["E-mail"].GetString());
        }
    }
}
