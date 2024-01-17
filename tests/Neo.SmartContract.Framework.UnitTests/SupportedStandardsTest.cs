using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SupportedStandardsTest
    {
        [TestMethod]
        public void TestAttribute()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_SupportedStandards.cs");
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP10", "NEP5" });
        }
    }
}
