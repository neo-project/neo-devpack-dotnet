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
            AssertGasConsumed(1049550);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(false, false, true));
            AssertGasConsumed(1050210);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(true, true, false));
            AssertGasConsumed(1065420);
            Assert.AreEqual(new BigInteger(4), Contract.Try01(true, true, true));
            AssertGasConsumed(1066080);
        }

        [TestMethod]
        public void Test_Try02_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try02(false, false, false));
            AssertGasConsumed(1067010);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(false, false, true));
            AssertGasConsumed(1067670);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(true, true, false));
            AssertGasConsumed(1082880);
            Assert.AreEqual(new BigInteger(4), Contract.Try02(true, true, true));
            AssertGasConsumed(1083540);
        }

        [TestMethod]
        public void Test_Try03_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try03(false, false, false));
            AssertGasConsumed(1049550);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(false, false, true));
            AssertGasConsumed(1050210);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(true, true, false));
            AssertGasConsumed(1080780);
            Assert.AreEqual(new BigInteger(4), Contract.Try03(true, true, true));
            AssertGasConsumed(1081440);
        }

        [TestMethod]
        public void Test_TryNest_AllPaths()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, false, false));
            AssertGasConsumed(1050480);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, false, false, false));
            AssertGasConsumed(1081710);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, true, false, false));
            AssertGasConsumed(1050480);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, true, true));
            AssertGasConsumed(1081500);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, true, true, true));
            AssertGasConsumed(1143570);
        }

        [TestMethod]
        public void Test_ThrowInCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(false, false, true));
            AssertGasConsumed(1049730);
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(true, false, true));
            AssertGasConsumed(1065600);
            Assert.ThrowsException<TestException>(() => Contract.ThrowInCatch(true, true, true));
            AssertGasConsumed(1080930);
        }

        [TestMethod]
        public void Test_TryFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinally(false, false));
            AssertGasConsumed(1049520);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinally(false, true));
            AssertGasConsumed(1050180);
            Assert.ThrowsException<TestException>(() => Contract.TryFinally(true, true));
            AssertGasConsumed(1065600);
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinallyAndRethrow(false, false));
            AssertGasConsumed(1049520);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinallyAndRethrow(false, true));
            AssertGasConsumed(1050180);
            Assert.ThrowsException<TestException>(() => Contract.TryFinallyAndRethrow(true, true));
            AssertGasConsumed(1080960);
        }

        [TestMethod]
        public void Test_TryCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryCatch(false, false));
            AssertGasConsumed(1049280);
            Assert.AreEqual(new BigInteger(3), Contract.TryCatch(true, true));
            AssertGasConsumed(1081080);
        }

        [TestMethod]
        public void Test_TryWithTwoFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(1), Contract.TryWithTwoFinally(false, false, false, false, false, false));
            AssertGasConsumed(1050810);
            Assert.AreEqual(new BigInteger(4), Contract.TryWithTwoFinally(false, false, false, false, true, false));
            AssertGasConsumed(1051500);
            Assert.AreEqual(new BigInteger(6), Contract.TryWithTwoFinally(false, false, false, false, false, true));
            AssertGasConsumed(1051500);
            Assert.AreEqual(new BigInteger(3), Contract.TryWithTwoFinally(true, false, true, false, false, false));
            AssertGasConsumed(1067280);
            Assert.AreEqual(new BigInteger(10), Contract.TryWithTwoFinally(false, true, false, true, false, true));
            AssertGasConsumed(1067970);
            Assert.AreEqual(new BigInteger(15), Contract.TryWithTwoFinally(true, true, true, true, true, true));
            AssertGasConsumed(1085130);
        }

        [TestMethod]
        public void Test_TryECPointCast_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryecpointCast(false, false, false));
            AssertGasConsumed(1050120);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(false, false, true));
            AssertGasConsumed(1050780);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(true, true, false));
            AssertGasConsumed(1065750);
            Assert.AreEqual(new BigInteger(4), Contract.TryecpointCast(true, true, true));
            AssertGasConsumed(1066410);
        }

        [TestMethod]
        public void Test_TryValidByteString2Ecpoint_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(false, false));
            AssertGasConsumed(1049970);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(false, true));
            AssertGasConsumed(1050630);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(true, false));
            AssertGasConsumed(1049970);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(true, true));
            AssertGasConsumed(1050630);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt160(false, false, false));
            AssertGasConsumed(1050120);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(false, false, true));
            AssertGasConsumed(1050780);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(true, true, false));
            AssertGasConsumed(1065750);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt160(true, true, true));
            AssertGasConsumed(1066410);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(false, false));
            AssertGasConsumed(1049970);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(false, true));
            AssertGasConsumed(1050630);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(true, false));
            AssertGasConsumed(1049970);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(true, true));
            AssertGasConsumed(1050630);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt256(false, false, false));
            AssertGasConsumed(1049670);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(false, false, true));
            AssertGasConsumed(1050330);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(true, true, false));
            AssertGasConsumed(1065690);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt256(true, true, true));
            AssertGasConsumed(1066350);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(false, false));
            AssertGasConsumed(1049520);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(false, true));
            AssertGasConsumed(1050180);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(true, false));
            AssertGasConsumed(1049520);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(true, true));
            AssertGasConsumed(1050180);
        }

        [TestMethod]
        public void Test_TryNULL2Ecpoint_1_AllPaths()
        {
            var result = Contract.TryNULL2Ecpoint_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1796940);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1798350);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1797600);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint160_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint160_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1796940);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint160_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1798350);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint160_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1797600);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint256_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint256_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1796940);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint256_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1798350);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint256_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1797600);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Bytestring_1_AllPaths()
        {
            var result = Contract.TryNULL2Bytestring_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1543260);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1544670);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1543920);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryUncatchableException_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryUncatchableException(false, false, false));
            AssertGasConsumed(1049550);
            Assert.AreEqual(new BigInteger(3), Contract.TryUncatchableException(false, false, true));
            AssertGasConsumed(1050210);
            Assert.ThrowsException<TestException>(() => Contract.TryUncatchableException(true, true, true));
            AssertGasConsumed(1049130);
        }

        [TestMethod]
        public void Test_ThrowCall()
        {
            Assert.ThrowsException<TestException>(Contract.ThrowCall);
        }
    }
}
