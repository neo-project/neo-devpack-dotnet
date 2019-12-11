using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_EntryPoints
    {
        [TestMethod]
        public void Test_MultipleContracts()
        {
            var testengine = new TestEngine();
            Assert.AreEqual(2, Assert.ThrowsException<EntryPointException>(() => testengine.AddEntryScript("./TestClasses/Contract_multilecontracts.cs")).Count);
        }

        [TestMethod]
        public void Test_NoEntryPoint()
        {
            var testengine = new TestEngine();
            Assert.AreEqual(0, Assert.ThrowsException<EntryPointException>(() => testengine.AddEntryScript("./TestClasses/NoContract.cs")).Count);
        }
    }
}
