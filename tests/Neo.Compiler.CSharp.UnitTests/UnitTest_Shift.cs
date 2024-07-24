using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Shift : TestBase<Contract_shift>
    {
        public UnitTest_Shift() : base(Contract_shift.Nef, Contract_shift.Manifest) { }

        [TestMethod]
        public void Test_Shift()
        {
            var list = Contract.TestShift()?.Cast<BigInteger>().ToArray();
            Assert.AreEqual(1048770, Engine.FeeConsumed.Value);
            CollectionAssert.AreEqual(new BigInteger[] { 16, 4 }, list);
        }

        [TestMethod]
        public void Test_Shift_BigInteger()
        {
            var list = Contract.TestShiftBigInt()?.Cast<BigInteger>().ToArray();
            Assert.AreEqual(1049370, Engine.FeeConsumed.Value);
            CollectionAssert.AreEqual(new BigInteger[] { 8, 16, 4, 2 }, list);
        }
    }
}
