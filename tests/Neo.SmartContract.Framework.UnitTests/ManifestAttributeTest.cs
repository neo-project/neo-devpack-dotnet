using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class ManifestAttributeTest
{
    public ManifestAttributeTest()
    {
        // Ensure also Contract_ExtraAttribute
        TestCleanup.TestInitialize(typeof(Contract_ManifestAttribute));
    }

    [TestMethod]
    public void TestManifestAttribute()
    {
        var extra = Contract_ManifestAttribute.Manifest!.Extra;

        Assert.AreEqual(6, extra.Count);
        // ["nef"]["optimizations"]
        // [Author("core-dev")]
        // [Email("dev@neo.org")]
        // [Version("v3.6.3")]
        // [Description("This is a test contract.")]
        // [ManifestExtra("ExtraKey", "ExtraValue")]
        Assert.AreEqual("core-dev", extra["Author"]!.GetString());
        Assert.AreEqual("dev@neo.org", extra["E-mail"]!.GetString());
        Assert.AreEqual("v3.6.3", extra["Version"]!.GetString());
        Assert.AreEqual("This is a test contract.", extra["Description"]!.GetString());
        Assert.AreEqual("ExtraValue", extra["ExtraKey"]!.GetString());
    }
}
