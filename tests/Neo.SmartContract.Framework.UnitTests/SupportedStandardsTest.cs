using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework.UnitTests.TestClasses;
using Neo.SmartContract.Framework.UnitTests.Utils;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SupportedStandardsTest
    {
        [TestMethod]
        public void TestAttribute()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript(typeof(Contract_SupportedStandards));
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP10", "NEP5" });
        }
    }
}
