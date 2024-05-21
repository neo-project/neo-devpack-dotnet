using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SupportedStandardsTest
    {
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
            CollectionAssert.AreEqual(Contract_SupportedStandard11Payable.Manifest.SupportedStandards, new string[] { "NEP-25" });
        }

        [TestMethod]
        public void TestStandardNEP17PayableAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard17Payable.Manifest.SupportedStandards, new string[] { "NEP-26" });
        }
    }
}
