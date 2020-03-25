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
            Assert.AreEqual("0", methodsABI[0].GetDictItem("offset").ToString());
            Assert.AreEqual("32", methodsABI[1].GetDictItem("offset").ToString());
            Assert.AreEqual("39", methodsABI[2].GetDictItem("offset").ToString());
            Assert.AreEqual("46", methodsABI[3].GetDictItem("offset").ToString());
            Assert.AreEqual("53", methodsABI[4].GetDictItem("offset").ToString());
        }

        [TestMethod]
        public void UnitTest_TestABIOffsetWithOptimizer()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ABIOffset.cs", true, true);
            var abi = testengine.ScriptEntry.finialABI;

            var methodsABI = abi["methods"].AsList();
            Assert.AreEqual("0", methodsABI[0].GetDictItem("offset").ToString());
            Assert.AreEqual("16", methodsABI[1].GetDictItem("offset").ToString());
            Assert.AreEqual("23", methodsABI[2].GetDictItem("offset").ToString());
            Assert.AreEqual("30", methodsABI[3].GetDictItem("offset").ToString());
            Assert.AreEqual("37", methodsABI[4].GetDictItem("offset").ToString());
        }
    }
}
