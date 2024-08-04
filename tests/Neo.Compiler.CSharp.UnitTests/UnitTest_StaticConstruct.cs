using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticConstruct : TestBase2<Contract_StaticConstruct>
    {
        public UnitTest_StaticConstruct() : base(Contract_StaticConstruct.Nef, Contract_StaticConstruct.Manifest) { }

        [TestMethod]
        public void Test_StaticConsturct()
        {
            var var1 = Contract.TestStatic();
            Assert.AreEqual(987390, Engine.FeeConsumed.Value);
            // static byte[] callscript = ExecutionEngine.EntryScriptHash;
            // ...
            // return callscript

            Assert.IsNotNull(var1);
            Assert.AreEqual(4, var1);
        }
    }
}
