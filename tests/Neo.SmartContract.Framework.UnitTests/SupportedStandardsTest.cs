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
            testengine.AddEntryScript<Contract_SupportedStandards>();
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP-10", "NEP-5" });
        }

        [TestMethod]
        public void TestStandardNEP11AttributeEnum()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript<Contract_SupportedStandard11Enum>();
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP-11" });
        }

        [TestMethod]
        public void TestStandardNEP17AttributeEnum()
        {
            var testengine = new TestEngine.TestEngine();
            testengine.AddEntryScript<Contract_SupportedStandard17Enum>();
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP-17" });
        }
    }
}
