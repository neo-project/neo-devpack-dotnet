using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Shift : DebugAndTestBase<Contract_shift>
    {
        [TestMethod]
        public void Test_Shift()
        {
            var list = Contract.TestShift()?.Cast<BigInteger>().ToArray();
            AssertGasConsumed(1048710);
            CollectionAssert.AreEqual(new BigInteger[] { 16, 4 }, list);
        }

        [TestMethod]
        public void Test_Shift_BigInteger()
        {
            var list = Contract.TestShiftBigInt()?.Cast<BigInteger>().ToArray();
            AssertGasConsumed(1049310);
            CollectionAssert.AreEqual(new BigInteger[] { 8, 16, 4, 2 }, list);
        }
    }
}
