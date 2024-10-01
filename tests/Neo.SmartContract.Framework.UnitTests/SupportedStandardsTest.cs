using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SupportedStandardsTest
    {
        public SupportedStandardsTest()
        {
            // Ensure also Contract_ExtraAttribute
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandards));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard11Enum));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard26));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard17Enum));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard27));
        }

        [TestMethod]
        public void TestAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandards.Manifest.SupportedStandards, new string[] { "NEP-10", "NEP-5" });
        }

        [TestMethod]
        public void TestStandardNEP11AttributeEnum()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard11Enum.Manifest.SupportedStandards, new string[] { "NEP-11" });
        }

        [TestMethod]
        public void TestStandardNEP17AttributeEnum()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard17Enum.Manifest.SupportedStandards, new string[] { "NEP-17" });
        }

        [TestMethod]
        public void TestStandardNEP11PayableAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard26.Manifest.SupportedStandards, new string[] { "NEP-26" });
        }

        [TestMethod]
        public void TestStandardNEP17PayableAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard27.Manifest.SupportedStandards, new string[] { "NEP-27" });
        }
    }
}
