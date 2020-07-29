using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class FeatureTest
    {
        [TestMethod]
        public void TestFeature()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Feature.cs");

            // Check that we have only one Feature

            Assert.AreEqual(1, testengine.ScriptEntry.converterIL.outModule.attributes.Count);

            // Check that was the SmartContract Feature

            Assert.AreEqual(ContractFeatures.Payable, testengine.ScriptEntry.converterIL.outModule.attributes
                .Where(u => u.AttributeType.FullName == "Neo.SmartContract.Framework.FeaturesAttribute")
                .Select(u => (ContractFeatures)u.ConstructorArguments.FirstOrDefault().Value)
                .FirstOrDefault());
        }
    }
}
