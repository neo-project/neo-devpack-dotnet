using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TryCatch : DebugAndTestBase<Contract_TryCatch>
    {
        [TestMethod]
        public void Test_Try01_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try01(false, false, false));
            AssertGasConsumed(1049670);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(false, false, true));
            AssertGasConsumed(1050330);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(true, true, false));
            AssertGasConsumed(1065660);
            Assert.AreEqual(new BigInteger(4), Contract.Try01(true, true, true));
            AssertGasConsumed(1066320);
        }

        [TestMethod]
        public void Test_Try02_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try02(false, false, false));
            AssertGasConsumed(1067130);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(false, false, true));
            AssertGasConsumed(1067790);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(true, true, false));
            AssertGasConsumed(1083120);
            Assert.AreEqual(new BigInteger(4), Contract.Try02(true, true, true));
            AssertGasConsumed(1083780);
        }

        [TestMethod]
        public void Test_Try03_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try03(false, false, false));
            AssertGasConsumed(1049670);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(false, false, true));
            AssertGasConsumed(1050330);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(true, true, false));
            AssertGasConsumed(1081020);
            Assert.AreEqual(new BigInteger(4), Contract.Try03(true, true, true));
            AssertGasConsumed(1081680);
        }

        [TestMethod]
        public void Test_TryNest_AllPaths()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, false, false));
            AssertGasConsumed(1050600);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, false, false, false));
            AssertGasConsumed(1081950);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, true, false, false));
            AssertGasConsumed(1050600);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, true, true));
            AssertGasConsumed(1081620);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, true, true, true));
            AssertGasConsumed(1143810);
        }

        [TestMethod]
        public void Test_ThrowInCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(false, false, true));
            AssertGasConsumed(1050090);
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(true, false, true));
            AssertGasConsumed(1066080);
            Assert.ThrowsException<TestException>(() => Contract.ThrowInCatch(true, true, true));
            AssertGasConsumed(1081290);
        }

        [TestMethod]
        public void Test_TryFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinally(false, false));
            AssertGasConsumed(1049640);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinally(false, true));
            AssertGasConsumed(1050300);
            Assert.ThrowsException<TestException>(() => Contract.TryFinally(true, true));
            AssertGasConsumed(1065720);
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinallyAndRethrow(false, false));
            AssertGasConsumed(1049640);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinallyAndRethrow(false, true));
            AssertGasConsumed(1050300);
            Assert.ThrowsException<TestException>(() => Contract.TryFinallyAndRethrow(true, true));
            AssertGasConsumed(1081080);
        }

        [TestMethod]
        public void Test_TryCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryCatch(false, false));
            AssertGasConsumed(1049400);
            Assert.AreEqual(new BigInteger(3), Contract.TryCatch(true, true));
            AssertGasConsumed(1081200);
        }

        [TestMethod]
        public void Test_TryWithTwoFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(1), Contract.TryWithTwoFinally(false, false, false, false, false, false));
            AssertGasConsumed(1050810);
            Assert.AreEqual(new BigInteger(4), Contract.TryWithTwoFinally(false, false, false, false, true, false));
            AssertGasConsumed(1051620);
            Assert.AreEqual(new BigInteger(6), Contract.TryWithTwoFinally(false, false, false, false, false, true));
            AssertGasConsumed(1051620);
            Assert.AreEqual(new BigInteger(3), Contract.TryWithTwoFinally(true, false, true, false, false, false));
            AssertGasConsumed(1067400);
            Assert.AreEqual(new BigInteger(10), Contract.TryWithTwoFinally(false, true, false, true, false, true));
            AssertGasConsumed(1068210);
            Assert.AreEqual(new BigInteger(15), Contract.TryWithTwoFinally(true, true, true, true, true, true));
            AssertGasConsumed(1085610);
        }

        [TestMethod]
        public void Test_TryECPointCast_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryecpointCast(false, false, false));
            AssertGasConsumed(1050240);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(false, false, true));
            AssertGasConsumed(1050900);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(true, true, false));
            AssertGasConsumed(1065990);
            Assert.AreEqual(new BigInteger(4), Contract.TryecpointCast(true, true, true));
            AssertGasConsumed(1066650);
        }

        [TestMethod]
        public void Test_TryValidByteString2Ecpoint_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(false, false));
            AssertGasConsumed(1050090);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(false, true));
            AssertGasConsumed(1050750);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(true, false));
            AssertGasConsumed(1050090);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(true, true));
            AssertGasConsumed(1050750);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt160(false, false, false));
            AssertGasConsumed(1050240);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(false, false, true));
            AssertGasConsumed(1050900);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(true, true, false));
            AssertGasConsumed(1065990);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt160(true, true, true));
            AssertGasConsumed(1066650);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(false, false));
            AssertGasConsumed(1050090);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(false, true));
            AssertGasConsumed(1050750);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(true, false));
            AssertGasConsumed(1050090);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(true, true));
            AssertGasConsumed(1050750);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt256(false, false, false));
            AssertGasConsumed(1049790);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(false, false, true));
            AssertGasConsumed(1050450);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(true, true, false));
            AssertGasConsumed(1065930);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt256(true, true, true));
            AssertGasConsumed(1066590);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(false, false));
            AssertGasConsumed(1049640);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(false, true));
            AssertGasConsumed(1050300);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(true, false));
            AssertGasConsumed(1049640);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(true, true));
            AssertGasConsumed(1050300);
        }

        [TestMethod]
        public void Test_TryNULL2Ecpoint_1_AllPaths()
        {
            var result = Contract.TryNULL2Ecpoint_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1797060);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1798590);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1797720);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint160_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint160_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1797060);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint160_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1798590);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint160_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1797720);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint256_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint256_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1797060);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint256_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1798590);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint256_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1797720);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Bytestring_1_AllPaths()
        {
            var result = Contract.TryNULL2Bytestring_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1543380);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1544910);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1544040);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryUncatchableException_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryUncatchableException(false, false, false));
            AssertGasConsumed(1049670);
            Assert.AreEqual(new BigInteger(3), Contract.TryUncatchableException(false, false, true));
            AssertGasConsumed(1050330);
            Assert.ThrowsException<TestException>(() => Contract.TryUncatchableException(true, true, true));
            AssertGasConsumed(1049250);
        }

        [TestMethod]
        public void Test_ThrowCall()
        {
            Assert.ThrowsException<TestException>(Contract.ThrowCall);
        }
    }
}
