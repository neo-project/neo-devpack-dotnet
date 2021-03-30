using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_EntryPoints
    {
        [TestMethod]
        public void Test_MultipleContracts()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_MultipleContracts.cs");
            Assert.AreEqual("Contract_a",testengine.ScriptEntry.manifest["name"].AsString());
        }
    }
}
