using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Overflow : DebugAndTestBase<Contract_Overflow>
    {
        [TestMethod]
        public void Test_Add()
        {
            Assert.AreEqual(unchecked(int.MinValue - 1), Contract.Add(int.MinValue, -1));
            Assert.AreEqual(unchecked(int.MaxValue + 1), Contract.Add(int.MaxValue, 1));
            Assert.AreEqual(unchecked(int.MinValue - int.MaxValue), Contract.Add(int.MinValue, -int.MaxValue));
            Assert.AreEqual(unchecked(int.MaxValue - int.MinValue), Contract.Add(int.MaxValue, unchecked(-int.MinValue)));
        }

        [TestMethod]
        public void Test_Mul()
        {
            Assert.AreEqual(unchecked(int.MinValue * 2), Contract.Mul(int.MinValue, 2));
            Assert.AreEqual(unchecked(int.MinValue * (-2)), Contract.Mul(int.MinValue, -2));
            Assert.AreEqual(unchecked(int.MaxValue * 2), Contract.Mul(int.MaxValue, 2));
            Assert.AreEqual(unchecked(int.MaxValue * (-2)), Contract.Mul(int.MaxValue, -2));
            Assert.AreEqual(unchecked(int.MinValue * int.MaxValue), Contract.Mul(int.MinValue, int.MaxValue));
            Assert.AreEqual(unchecked(int.MinValue * (-int.MaxValue)), Contract.Mul(int.MinValue, -int.MaxValue));
            Assert.AreEqual(unchecked((-int.MinValue) * int.MaxValue), Contract.Mul(unchecked(-int.MinValue), int.MaxValue));
        }
    }
}
