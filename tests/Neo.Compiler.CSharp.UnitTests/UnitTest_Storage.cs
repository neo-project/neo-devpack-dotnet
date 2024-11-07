using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Storage : DebugAndTestBase<Contract_Storage>
    {
        [TestMethod]
        public void Test_Storage()
        {
            var a = Contract;
            Engine.SetTransactionSigners(Bob);
            var b = Engine.Deploy<Contract_Storage>(Contract_Storage.Nef, Contract_Storage.Manifest);

            a.MainA(b.Hash, true);

            Assert.AreEqual(0x01, a.Storage.Get(0xA0).ToArray()[0]);
            Assert.AreEqual(0x02, a.Storage.Get(0xA1).ToArray()[0]);

            Assert.IsTrue(b.Storage.Get(0xB0).IsEmpty);
            Assert.IsTrue(b.Storage.Get(0xB1).IsEmpty);
        }
    }
}
