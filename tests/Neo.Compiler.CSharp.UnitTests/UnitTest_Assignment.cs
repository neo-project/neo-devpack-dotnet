using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Assignment : DebugAndTestBase<Contract_Assignment>
    {
        [TestMethod]
        public void Test_Assignment()
        {
            Contract.TestAssignment();
            AssertGasConsumed(989490);
        }

        [TestMethod]
        public void Test_CoalesceAssignment()
        {
            Contract.TestCoalesceAssignment();
            AssertGasConsumed(988950);
        }
    }
}
