using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_AllowOverflow : DebugAndTestBase<Contract_AllowOverflow>
    {
        public UnitTest_AllowOverflow() : base(compilationOptions: TestCleanup.TestCompilationOptionsSimOverFlow) { }

        [TestMethod]
        public void Test_ArrayElement()
        {
            for (int i = 0; i < 8; ++i)
                Assert.AreEqual(i, Contract.ArrayElement(i));
            AssertGasConsumed(1110780);
        }

        [TestMethod]
        public void Test_Shift()
        {
            Assert.AreEqual(1, Contract.ShiftRight(8, 3));
            AssertGasConsumed(1047360);
        }
    }
}
