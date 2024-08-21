using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property_Method : DebugAndTestBase<Contract_PropertyMethod>
    {
        [TestMethod]
        public void TestPropertyMethod()
        {
            var arr = Contract.TestProperty()!;
            AssertGasConsumed(2053500);

            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual((arr[0] as StackItem)!.GetString(), "NEO3");
            Assert.AreEqual(arr[1], new BigInteger(10));
        }

        [TestMethod]
        public void TestPropertyMethod2()
        {
            Contract.TestProperty2();
            AssertGasConsumed(1557360);
            // No errors
        }
    }
}
