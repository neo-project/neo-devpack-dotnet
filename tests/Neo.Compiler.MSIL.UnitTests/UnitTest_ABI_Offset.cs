using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;
using System;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Offset
    {
        [TestMethod]
        public void UnitTest_TestABIOffsetWithoutOptimizer()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ABIOffset.cs", true, false);
            var abi = testengine.ScriptEntry.finialABI;

            var methodsABI = abi["methods"].AsList();
            Assert.AreEqual("87", methodsABI[0].GetDictItem("offset").ToString());
            Assert.AreEqual("94", methodsABI[1].GetDictItem("offset").ToString());
            Assert.AreEqual("124", methodsABI[2].GetDictItem("offset").ToString());
        }

        [TestMethod]
        public void UnitTest_TestABIOffsetWithOptimizer()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ABIOffset.cs", true, true);
            var abi = testengine.ScriptEntry.finialABI;

            var methodsABI = abi["methods"].AsList();
            Assert.AreEqual("67", methodsABI[0].GetDictItem("offset").ToString());
            Assert.AreEqual("74", methodsABI[1].GetDictItem("offset").ToString());
            Assert.AreEqual("98", methodsABI[2].GetDictItem("offset").ToString());
        }
    }
}
