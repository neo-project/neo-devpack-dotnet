using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_IndexOrRange() : TestBase<Contract_IndexOrRange>(Contract_IndexOrRange.Nef, Contract_IndexOrRange.Manifest)
    {
        [TestMethod]
        public void Test_Main()
        {
            // Contract.TestMain();
        }
    }
}
