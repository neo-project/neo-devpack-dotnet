using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class ManifestExtraTest
    {
        [TestMethod]
        public void TestExtraAttribute()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ExtraAttribute.cs");
            Assert.AreEqual(2, testengine.ScriptEntry.converterIL.outModule.attributes.Count);

            var extraAttributes = testengine.ScriptEntry.converterIL.outModule.attributes.Where(u => u.AttributeType.Name == "ManifestExtraAttribute").Select(attribute => attribute.ConstructorArguments).ToList();

            Assert.AreEqual("Author", extraAttributes[0][0].Value);
            Assert.AreEqual("Neo", extraAttributes[0][1].Value);
            Assert.AreEqual("E-mail", extraAttributes[1][0].Value);
            Assert.AreEqual("dev@neo.org", extraAttributes[1][1].Value);
        }
    }
}
