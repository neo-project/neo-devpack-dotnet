using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework.UnitTests.Old.Utils;

namespace Neo.SmartContract.Framework.UnitTests.Old
{
    [TestClass]
    public class SupportedStandardsTest
    {
        [TestMethod]
        public void TestAttribute()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript(Extensions.TestContractRoot + "Contract_SupportedStandards.cs");
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP-10", "NEP-5" });
        }

        [TestMethod]
        public void TestStandardNEP11AttributeEnum()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript(Extensions.TestContractRoot + "Contract_SupportedStandard11Enum.cs");
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP-11" });
        }

        [TestMethod]
        public void TestStandardNEP17AttributeEnum()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript(Extensions.TestContractRoot + "Contract_SupportedStandard17Enum.cs");
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP-17" });
        }
    }
}
