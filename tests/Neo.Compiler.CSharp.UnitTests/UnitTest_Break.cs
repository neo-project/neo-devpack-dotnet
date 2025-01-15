using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Break : DebugAndTestBase<Contract_Break>
    {
        [TestMethod]
        public void TestBreakInTry()
        {
            Contract.BreakInTryCatch();
        }
    }
}
