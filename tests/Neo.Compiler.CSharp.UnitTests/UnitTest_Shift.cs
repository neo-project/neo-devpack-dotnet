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
        /*
        [TestMethod]
        public void Test_Shift()
        {
            var result = Contract.TestShift();
            var list = ((VM.Types.Array)result.Pop()).Select(u => u.GetInteger()).ToList();

            CollectionAssert.AreEqual(new BigInteger[] { 16, 4 }, list);
        }

        [TestMethod]
        public void Test_Shift_BigInteger()
        {
            var result = Contract.TestShiftBigInt();
            var list = ((VM.Types.Array)result.Pop()).Select(u => u.GetInteger()).ToList();

            CollectionAssert.AreEqual(new BigInteger[] { 8, 16, 4, 2 }, list);
        }*/
    }
}
