using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property_Method : TestBase2<Contract_PropertyMethod>
    {
        [TestMethod]
        public void TestPropertyMethod()
        {
            var arr = Contract.TestProperty()!;
            Assert.AreEqual(2053500, Engine.FeeConsumed.Value);

            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual((arr[0] as StackItem)!.GetString(), "NEO3");
            Assert.AreEqual(arr[1], new BigInteger(10));
        }

        [TestMethod]
        public void TestPropertyMethod2()
        {
            Contract.TestProperty2();
            Assert.AreEqual(1557360, Engine.FeeConsumed.Value);
            // No errors
        }
    }
}
