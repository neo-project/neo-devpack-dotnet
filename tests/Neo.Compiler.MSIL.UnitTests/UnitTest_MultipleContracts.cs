using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using System;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_MultipleContracts
    {
        [TestMethod]
        public void Test_MultipleContracts()
        {
            var testengine = new TestEngine();
            Assert.ThrowsException<Exception>(() => testengine.AddEntryScript("./TestClasses/Contract_multilecontracts.cs"));
        }
    }
}
