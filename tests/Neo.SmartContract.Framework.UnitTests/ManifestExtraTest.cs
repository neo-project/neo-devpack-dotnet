using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class ManifestExtraTest
    {
        [TestMethod]
        public void TestExtraAttribute()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ExtraAttribute.cs");

            var extra = testengine.Manifest.Extra;

            Assert.AreEqual("Neo", extra["Author"].GetString());
            Assert.AreEqual("dev@neo.org", extra["E-mail"].GetString());
        }
    }
}
