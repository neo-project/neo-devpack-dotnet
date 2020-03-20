using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_EntryPoints
    {
        [TestMethod]
        public void Test_MultipleContracts()
        {
            using (var testengine = new TestEngine())
            {
                Assert.AreEqual(2, Assert.ThrowsException<EntryPointException>(() => testengine.AddEntryScript("./TestClasses/Contract_MultipleContracts.cs")).Count);
            }

            using (var testengine = new TestEngine())
            {
                Assert.AreEqual(2, Assert.ThrowsException<EntryPointException>(() => testengine.AddEntryScript("./TestClasses/Contract_MultipleContracts2.cs")).Count);
            }
        }

        [TestMethod]
        public void Test_NoEntryPoint()
        {
            using (var testengine = new TestEngine())
            {
                Assert.AreEqual(0, Assert.ThrowsException<EntryPointException>(() => testengine.AddEntryScript("./TestClasses/NoContract.cs")).Count);
            }
        }
    }
}
