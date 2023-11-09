using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SupportedStandardsTest
    {
        [TestMethod]
        public void TestAttribute()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_SupportedStandards.cs");
            CollectionAssert.AreEqual(testengine.Manifest.SupportedStandards, new string[] { "NEP10", "NEP5" });
        }
    }
}
