using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Contract2 : TestBase2<Contract2>
    {
        public UnitTest_Contract2() : base(Contract2.Nef, Contract2.Manifest) { }

        [TestMethod]
        public void Test_ByteArrayPick()
        {
            Assert.AreEqual(3, Contract.UnitTest_002("hello", 1));
            Assert.AreEqual(1295280, Engine.FeeConsumed.Value);
        }
    }
}
