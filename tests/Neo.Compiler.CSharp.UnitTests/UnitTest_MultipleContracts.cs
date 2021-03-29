using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using System;

namespace Neo.Compiler.CSharp.UnitTests.Utils
{
    [TestClass]
    public class UnitTest_EntryPoints
    {
        [TestMethod]
        public void Test_MultipleContracts()
        {
            using var testengine = new TestEngine();
            Assert.ThrowsException<Exception>(() => testengine.AddEntryScript("./TestClasses/Contract_MultipleContracts.cs"));
        }
    }
}
