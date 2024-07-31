using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TryCatch : TestBase<Contract_TryCatch>
    {
        public UnitTest_TryCatch() : base(Contract_TryCatch.Nef, Contract_TryCatch.Manifest) { }

        [TestMethod]
        public void Test_TryCatch_Succ()
        {
            Assert.AreEqual(new BigInteger(3), Contract.Try01(false));
            Assert.AreEqual(new BigInteger(4), Contract.Try01(true));
        }

        [TestMethod]
        public void Test_UncatchableException()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryUncatchableException(false));
            _ = Assert.ThrowsException<TestException>(() => Contract.TryUncatchableException(true));
        }

        [TestMethod]
        public void Test_TryCatch_ThrowByCall()
        {
            Assert.AreEqual(new BigInteger(3), Contract.Try03(false));
            Assert.AreEqual(new BigInteger(4), Contract.Try03(true));
        }

        [TestMethod]
        public void Test_TryCatch_Throw()
        {
            Assert.AreEqual(new BigInteger(3), Contract.Try02(false));
            Assert.AreEqual(new BigInteger(4), Contract.Try02(true));
        }

        [TestMethod]
        public void Test_TryNest()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, false));
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, true, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, true));
        }

        [TestMethod]
        public void Test_ThrowInCatch()
        {
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(false, false));
            var exception = Assert.ThrowsException<TestException>(() => Contract.ThrowInCatch(true, false));
            Assert.AreEqual(new BigInteger(3), exception.CurrentContext?.LocalVariables?[0].GetInteger());
            exception = Assert.ThrowsException<TestException>(() => Contract.ThrowInCatch(true, true));
            Assert.AreEqual(new BigInteger(3), exception.CurrentContext?.LocalVariables?[0].GetInteger());
        }

        [TestMethod]
        public void Test_TryFinally()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryFinally(false));
            _ = Assert.ThrowsException<TestException>(() => Contract.TryFinally(true));
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryFinallyAndRethrow(false));
            _ = Assert.ThrowsException<TestException>(() => Contract.TryFinallyAndRethrow(true));
        }

        [TestMethod]
        public void Test_TryCatch()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryCatch(false));
            Assert.AreEqual(new BigInteger(3), Contract.TryCatch(true));
        }

        [TestMethod]
        public void Test_TryWithTwoFinally()
        {
            Assert.AreEqual(new BigInteger(9), Contract.TryWithTwoFinally(false, false));
            Assert.AreEqual(new BigInteger(11), Contract.TryWithTwoFinally(true, false));
            Assert.AreEqual(new BigInteger(13), Contract.TryWithTwoFinally(false, true));
            Assert.AreEqual(new BigInteger(15), Contract.TryWithTwoFinally(true, true));
        }

        [TestMethod]
        public void Test_TryECPointCast()
        {
            Assert.AreEqual(new BigInteger(4), Contract.TryecpointCast(true));
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(false));
        }

        [TestMethod]
        public void Test_TryValidECPointCast()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint());
        }

        [TestMethod]
        public void Test_TryInvalidUInt160Cast()
        {
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt160(true));
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(false));
        }

        [TestMethod]
        public void Test_TryValidUInt160Cast()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160());
        }

        [TestMethod]
        public void Test_TryInvalidUInt256Cast()
        {
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt256(true));
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(false));
        }

        [TestMethod]
        public void Test_TryValidUInt256Cast()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256());
        }

        [TestMethod]
        public void Test_TryNULLECPointCast_1()
        {
            var array = Contract.TryNULL2Ecpoint_1(true);
            Assert.AreEqual(new BigInteger(4), array?[0]);
            Assert.IsNull(array?[1]);

            array = Contract.TryNULL2Ecpoint_1(false);
            Assert.AreEqual(new BigInteger(3), array?[0]);
            Assert.IsNotNull(array?[1]);
        }

        [TestMethod]
        public void Test_TryNULLUInt160Cast_1()
        {
            var array = Contract.TryNULL2Uint160_1(true);
            Assert.AreEqual(new BigInteger(4), array?[0]);
            Assert.IsNull(array?[1]);

            array = Contract.TryNULL2Uint160_1(false);
            Assert.AreEqual(new BigInteger(3), array?[0]);
            Assert.IsNotNull(array?[1]);
        }

        [TestMethod]
        public void Test_TryNULLUInt256Cast_1()
        {
            var array = Contract.TryNULL2Uint256_1(true);
            Assert.AreEqual(new BigInteger(4), array?[0]);
            Assert.IsNull(array?[1]);

            array = Contract.TryNULL2Uint256_1(false);
            Assert.AreEqual(new BigInteger(3), array?[0]);
            Assert.IsNotNull(array?[1]);
        }

        [TestMethod]
        public void Test_TryNULLBytestringCast_1()
        {
            var array = Contract.TryNULL2Bytestring_1(true);
            Assert.AreEqual(new BigInteger(4), array?[0]);
            Assert.IsNull(array?[1]);

            array = Contract.TryNULL2Bytestring_1(false);
            Assert.AreEqual(new BigInteger(3), array?[0]);
            Assert.IsNotNull(array?[1]);
        }
    }
}
