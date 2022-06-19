using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.IO.Json;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ABI_NoReentry
{
    [TestMethod]
    public void UnitTest_TestRestricted()
    {
        var testEngine = new TestEngine();
        testEngine.AddEntryScript("./TestClasses/Contract_ABINoReentry.cs");

        var methodsABI = testEngine.Manifest["abi"]["methods"] as JArray;
        Assert.IsFalse(methodsABI[0]["noreentry"].AsBoolean());
        Assert.IsTrue(methodsABI[1]["noreentry"].AsBoolean());
        Assert.IsFalse(methodsABI[2]["noreentry"].AsBoolean());
    }
}
