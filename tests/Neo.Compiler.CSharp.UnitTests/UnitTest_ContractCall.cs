using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ContractCall : TestBase<Contract_ContractCall>
    {
        public UnitTest_ContractCall() : base(Contract_ContractCall.Nef, Contract_ContractCall.Manifest) { }

        [TestInitialize]
        public void Init()
        {
            Alice.Account = UInt160.Parse("0102030405060708090A0102030405060708090A");
            var c1 = Engine.Deploy<Contract1>(Contract1.Nef, Contract1.Manifest);
            Assert.AreEqual("0x143bbd1ee2bd7cd445073ea883d24f324bde4872", c1.Hash.ToString());
        }

        [TestMethod]
        public void Test_ContractCall()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, Contract.TestContractCall());
        }

        [TestMethod]
        public void Test_ContractCall_Void()
        {
            Contract.TestContractCallVoid(); // No error
        }
    }
}
