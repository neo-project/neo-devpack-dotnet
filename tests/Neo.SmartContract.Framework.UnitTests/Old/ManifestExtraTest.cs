using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework.UnitTests.Old.Utils;

namespace Neo.SmartContract.Framework.UnitTests.Old
{
    [TestClass]
    public class ManifestExtraTest
    {
        [TestMethod]
        public void TestExtraAttribute()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript(Extensions.TestContractRoot + "Contract_ExtraAttribute.cs");

            var extra = testengine.Manifest.Extra;

            Assert.AreEqual("Neo", extra["Author"].GetString());
            Assert.AreEqual("dev@neo.org", extra["E-mail"].GetString());
        }
    }
}
