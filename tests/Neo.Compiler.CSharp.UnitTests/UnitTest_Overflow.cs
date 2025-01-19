using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Overflow : DebugAndTestBase<Contract_Overflow>
    {
        [TestMethod]
        public void Test_AddInt()
        {
            Assert.AreEqual(unchecked(int.MinValue - 1), Contract.AddInt(int.MinValue, -1));
            Assert.AreEqual(unchecked(int.MaxValue + 1), Contract.AddInt(int.MaxValue, 1));
            Assert.AreEqual(unchecked(int.MinValue - int.MaxValue), Contract.AddInt(int.MinValue, -int.MaxValue));
            Assert.AreEqual(unchecked(int.MaxValue - int.MinValue), Contract.AddInt(int.MaxValue, unchecked(-int.MinValue)));
        }

        [TestMethod]
        public void Test_MulInt()
        {
            Assert.AreEqual(unchecked(int.MinValue * 2), Contract.MulInt(int.MinValue, 2));
            Assert.AreEqual(unchecked(int.MinValue * (-2)), Contract.MulInt(int.MinValue, -2));
            Assert.AreEqual(unchecked(int.MaxValue * 2), Contract.MulInt(int.MaxValue, 2));
            Assert.AreEqual(unchecked(int.MaxValue * (-2)), Contract.MulInt(int.MaxValue, -2));
            Assert.AreEqual(unchecked(int.MinValue * int.MaxValue), Contract.MulInt(int.MinValue, int.MaxValue));
            Assert.AreEqual(unchecked(int.MinValue * (-int.MaxValue)), Contract.MulInt(int.MinValue, -int.MaxValue));
            Assert.AreEqual(unchecked((-int.MinValue) * int.MaxValue), Contract.MulInt(unchecked(-int.MinValue), int.MaxValue));
        }

        [TestMethod]
        public void Test_AddUInt()
        {
            Assert.AreEqual(unchecked(uint.MinValue - 1), Contract.AddUInt(uint.MinValue, -1));
            Assert.AreEqual(unchecked(uint.MaxValue + 1), Contract.AddUInt(uint.MaxValue, 1));
            Assert.AreEqual(unchecked(uint.MinValue - uint.MaxValue), Contract.AddUInt(uint.MinValue, -uint.MaxValue));
            Assert.AreEqual(unchecked(uint.MaxValue - uint.MinValue), Contract.AddUInt(uint.MaxValue, unchecked(-uint.MinValue)));
        }

        [TestMethod]
        public void Test_MulUInt()
        {
            Assert.AreEqual(unchecked(uint.MinValue * 2), Contract.MulUInt(uint.MinValue, 2));
            Assert.AreEqual(unchecked(uint.MinValue * (-2)), Contract.MulUInt(uint.MinValue, -2));
            Assert.AreEqual(unchecked(uint.MaxValue * 2), Contract.MulUInt(uint.MaxValue, 2));
            Assert.AreEqual(unchecked(uint.MaxValue * (uint)(-2)), Contract.MulUInt(uint.MaxValue, -2));
            Assert.AreEqual(unchecked(uint.MinValue * uint.MaxValue), Contract.MulUInt(uint.MinValue, uint.MaxValue));
            Assert.AreEqual(unchecked(uint.MinValue * (-uint.MaxValue)), Contract.MulUInt(uint.MinValue, -uint.MaxValue));
            Assert.AreEqual(unchecked((-uint.MinValue) * uint.MaxValue), Contract.MulUInt(unchecked(-uint.MinValue), uint.MaxValue));
        }

        [TestMethod]
        public void Test_NegateChecked()
        {
            Assert.AreEqual(-2147483647, Contract.NegateIntChecked(2147483647));

            // VMUnhandledException -int.MinValue
            Assert.ThrowsException<TestException>(() => Contract.NegateIntChecked(int.MinValue));

            Assert.AreEqual(-9223372036854775807, Contract.NegateLongChecked(9223372036854775807));
            Assert.ThrowsException<TestException>(() => Contract.NegateLongChecked(long.MinValue));

            // -short -> int
            Assert.AreEqual(-32767, Contract.NegateShortChecked(32767));
            Assert.AreEqual(32768, Contract.NegateShort(short.MinValue));

            // unchecked(-int.MinValue) == int.MinValue
            Assert.AreEqual(int.MinValue, unchecked(-int.MinValue));

            // unchecked(-long.MinValue) == long.MinValue
            Assert.AreEqual(long.MinValue, unchecked(-long.MinValue));

            // it is different for short.MinValue, because `-short` is an int
            Assert.AreEqual(32768, unchecked(-short.MinValue));
        }
    }
}
