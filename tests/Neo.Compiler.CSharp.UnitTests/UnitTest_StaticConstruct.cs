using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticConstruct : TestBase<Contract_StaticConstruct>
    {
        public UnitTest_StaticConstruct() : base(Contract_StaticConstruct.Nef, Contract_StaticConstruct.Manifest) { }

        [TestMethod]
        public void Test_StaticConsturct()
        {
            var var1 = Contract.TestStatic();
            // static byte[] callscript = ExecutionEngine.EntryScriptHash;
            // ...
            // return callscript

            Assert.IsNotNull(var1);
            Assert.AreEqual(4, var1);
        }
    }
}
