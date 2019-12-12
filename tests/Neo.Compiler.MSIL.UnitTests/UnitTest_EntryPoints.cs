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
            using (var testengine = new TestEngine())
            {
                Assert.AreEqual(2, Assert.ThrowsException<EntryPointException>(() => testengine.AddEntryScript("./TestClasses/Contract_multilecontracts.cs")).Count);
            }

            using (var testengine = new TestEngine())
            {
                Assert.AreEqual(2, Assert.ThrowsException<EntryPointException>(() => testengine.AddEntryScript("./TestClasses/Contract_multilecontracts2.cs")).Count);
            }
        }

        [TestMethod]
        public void Test_NoEntryPoint()
        {
            using var testengine = new TestEngine();
            Assert.AreEqual(0, Assert.ThrowsException<EntryPointException>(() => testengine.AddEntryScript("./TestClasses/NoContract.cs")).Count);
        }
    }
}
