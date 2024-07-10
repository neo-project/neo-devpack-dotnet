using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Contract2 : TestBase<Contract2>
    {
        public UnitTest_Contract2() : base(Contract2.Nef, Contract2.Manifest) { }

        [TestMethod]
        public void Test_ByteArrayPick()
        {
            Assert.AreEqual(3, Contract.UnitTest_002("hello", 1));
        }
    }
}
