extern alias scfx;
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
        [TestMethod]
        public void Test_Try01_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try01(false, false, false));
            Assert.AreEqual(1049670, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(false, false, true));
            Assert.AreEqual(1050330, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(true, true, false));
            Assert.AreEqual(1065660, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.Try01(true, true, true));
            Assert.AreEqual(1066320, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Try02_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try02(false, false, false));
            Assert.AreEqual(1067130, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(false, false, true));
            Assert.AreEqual(1067790, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(true, true, false));
            Assert.AreEqual(1083120, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.Try02(true, true, true));
            Assert.AreEqual(1083780, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Try03_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try03(false, false, false));
            Assert.AreEqual(1049670, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(false, false, true));
            Assert.AreEqual(1050330, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(true, true, false));
            Assert.AreEqual(1081020, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.Try03(true, true, true));
            Assert.AreEqual(1081680, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryNest_AllPaths()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, false, false));
            Assert.AreEqual(1050600, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, false, false, false));
            Assert.AreEqual(1081950, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, true, false, false));
            Assert.AreEqual(1050600, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, true, true));
            Assert.AreEqual(1081620, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, true, true, true));
            Assert.AreEqual(1143810, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ThrowInCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(false, false, true));
            Assert.AreEqual(1050090, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(true, false, true));
            Assert.AreEqual(1066080, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.ThrowInCatch(true, true, true));
            Assert.AreEqual(1081290, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinally(false, false));
            Assert.AreEqual(1049640, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinally(false, true));
            Assert.AreEqual(1050300, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TryFinally(true, true));
            Assert.AreEqual(1065720, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinallyAndRethrow(false, false));
            Assert.AreEqual(1049640, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinallyAndRethrow(false, true));
            Assert.AreEqual(1050300, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TryFinallyAndRethrow(true, true));
            Assert.AreEqual(1081080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryCatch(false, false));
            Assert.AreEqual(1049400, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryCatch(true, true));
            Assert.AreEqual(1081200, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryWithTwoFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(1), Contract.TryWithTwoFinally(false, false, false, false, false, false));
            Assert.AreEqual(1050810, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.TryWithTwoFinally(false, false, false, false, true, false));
            Assert.AreEqual(1051620, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(6), Contract.TryWithTwoFinally(false, false, false, false, false, true));
            Assert.AreEqual(1051620, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryWithTwoFinally(true, false, true, false, false, false));
            Assert.AreEqual(1067400, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(10), Contract.TryWithTwoFinally(false, true, false, true, false, true));
            Assert.AreEqual(1068210, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(15), Contract.TryWithTwoFinally(true, true, true, true, true, true));
            Assert.AreEqual(1085610, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryECPointCast_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryecpointCast(false, false, false));
            Assert.AreEqual(1050240, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(false, false, true));
            Assert.AreEqual(1050900, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(true, true, false));
            Assert.AreEqual(1065990, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.TryecpointCast(true, true, true));
            Assert.AreEqual(1066650, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryValidByteString2Ecpoint_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(false, false));
            Assert.AreEqual(1050090, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(false, true));
            Assert.AreEqual(1050750, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(true, false));
            Assert.AreEqual(1050090, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(true, true));
            Assert.AreEqual(1050750, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt160(false, false, false));
            Assert.AreEqual(1050240, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(false, false, true));
            Assert.AreEqual(1050900, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(true, true, false));
            Assert.AreEqual(1065990, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt160(true, true, true));
            Assert.AreEqual(1066650, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(false, false));
            Assert.AreEqual(1050090, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(false, true));
            Assert.AreEqual(1050750, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(true, false));
            Assert.AreEqual(1050090, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(true, true));
            Assert.AreEqual(1050750, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt256(false, false, false));
            Assert.AreEqual(1049790, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(false, false, true));
            Assert.AreEqual(1050450, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(true, true, false));
            Assert.AreEqual(1065930, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt256(true, true, true));
            Assert.AreEqual(1066590, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(false, false));
            Assert.AreEqual(1049640, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(false, true));
            Assert.AreEqual(1050300, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(true, false));
            Assert.AreEqual(1049640, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(true, true));
            Assert.AreEqual(1050300, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryNULL2Ecpoint_1_AllPaths()
        {
            var result = Contract.TryNULL2Ecpoint_1(false, false, false);
            Assert.AreEqual(1797060, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(true, false, true);
            Assert.AreEqual(1798590, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(false, true, true);
            Assert.AreEqual(1797720, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint160_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint160_1(false, false, false);
            Assert.AreEqual(1797060, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint160_1(true, false, true);
            Assert.AreEqual(1798590, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint160_1(false, true, true);
            Assert.AreEqual(1797720, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint256_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint256_1(false, false, false);
            Assert.AreEqual(1797060, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint256_1(true, false, true);
            Assert.AreEqual(1798590, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint256_1(false, true, true);
            Assert.AreEqual(1797720, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Bytestring_1_AllPaths()
        {
            var result = Contract.TryNULL2Bytestring_1(false, false, false);
            Assert.AreEqual(1543380, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(true, false, true);
            Assert.AreEqual(1544910, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(false, true, true);
            Assert.AreEqual(1544040, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryUncatchableException_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryUncatchableException(false, false, false));
            Assert.AreEqual(1049670, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), Contract.TryUncatchableException(false, false, true));
            Assert.AreEqual(1050330, Engine.FeeConsumed.Value);
            Assert.ThrowsException<TestException>(() => Contract.TryUncatchableException(true, true, true));
            Assert.AreEqual(1049250, Engine.FeeConsumed.Value);
        }


        [TestMethod]
        public void Test_ThrowCall()
        {
            Assert.ThrowsException<TestException>(() => Contract.ThrowCall());
        }
    }
}
