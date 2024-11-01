using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ContractCall : DebugAndTestBase<Contract_ContractCall>
    {
        [TestInitialize]
        public void Init()
        {
            Alice.Account = UInt160.Parse("0102030405060708090A0102030405060708090A");
            var c1 = Engine.Deploy<Contract1>(Contract1.Nef, Contract1.Manifest);
            Assert.AreEqual("0x86a42a1199f644e7ad18166de02eea45a1884954", c1.Hash.ToString());
        }

        [TestMethod]
        public void Test_ContractCall()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, Contract.TestContractCall());
            AssertGasConsumed(2461230);
        }

        [TestMethod]
        public void Test_ContractCall_Void()
        {
            Contract.TestContractCallVoid(); // No error
            AssertGasConsumed(2215050);
        }
    }
}
