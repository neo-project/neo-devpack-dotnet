using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Continue : DebugAndTestBase<Contract_Continue>
    {
        [TestMethod]
        public void TestContinueInTry()
        {
            Contract.ContinueInTryCatch(true);
            Contract.ContinueInTryCatch(false);
        }
    }
}
