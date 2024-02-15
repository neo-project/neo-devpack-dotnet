using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class ManifestAttributeTest
{

    [TestMethod]
    public void TestManifestAttribute()
    {
        var testEngine = new TestEngine.TestEngine();
        testEngine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_ManifestAttribute.cs");

        var extra = testEngine.Manifest!.Extra;

        Assert.AreEqual(5, extra.Count);
        // [Author("core-dev")]
        // [Email("core@neo.org")]
        // [Version("v3.6.3")]
        // [Description("This is a test contract.")]
        // [ManifestExtra("ExtraKey", "ExtraValue")]
        Assert.AreEqual("core-dev", extra["Author"]!.GetString());
        Assert.AreEqual("core@neo.org", extra["E-mail"]!.GetString());
        Assert.AreEqual("v3.6.3", extra["Version"]!.GetString());
        Assert.AreEqual("This is a test contract.", extra["Description"]!.GetString());
        Assert.AreEqual("ExtraValue", extra["ExtraKey"]!.GetString());
    }
}
