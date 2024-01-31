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
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_SupportedStandards.cs");
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP-10", "NEP-5" });
        }

        [TestMethod]
        public void TestStandardAttributeEnum()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_SupportedStandardsEnum.cs");
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP-11", "NEP-17" });
        }
    }
}
