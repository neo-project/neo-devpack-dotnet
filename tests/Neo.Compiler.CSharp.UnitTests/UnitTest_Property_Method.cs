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
            AssertGasConsumed(2053530);

            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual((arr[0] as StackItem)!.GetString(), "NEO3");
            Assert.AreEqual(arr[1], new BigInteger(10));
        }

        [TestMethod]
        public void TestPropertyMethod2()
        {
            Contract.TestProperty2();
            AssertGasConsumed(1557390);
            // No errors
        }

        [TestMethod]
        public void TestPropertyInit()
        {
            var arr = Contract.TestPropertyInit()!;
            AssertGasConsumed(2547510);

            Assert.AreEqual(3, arr.Count);
            Assert.AreEqual((arr[0] as StackItem)!.GetString(), "NEO3");
            Assert.AreEqual(arr[1], new BigInteger(10));
            Assert.AreEqual((arr[2] as StackItem)!.GetString(), "123 Blockchain St");
        }
    }
}
