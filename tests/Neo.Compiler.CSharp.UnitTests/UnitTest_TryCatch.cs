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
            AssertGasConsumed(1047870);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(false, false, true));
            AssertGasConsumed(1048530);
            Assert.AreEqual(new BigInteger(3), Contract.Try01(true, true, false));
            AssertGasConsumed(1063740);
            Assert.AreEqual(new BigInteger(4), Contract.Try01(true, true, true));
            AssertGasConsumed(1064400);
        }

        [TestMethod]
        public void Test_Try02_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try02(false, false, false));
            AssertGasConsumed(1065330);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(false, false, true));
            AssertGasConsumed(1065990);
            Assert.AreEqual(new BigInteger(3), Contract.Try02(true, true, false));
            AssertGasConsumed(1081200);
            Assert.AreEqual(new BigInteger(4), Contract.Try02(true, true, true));
            AssertGasConsumed(1081860);
        }

        [TestMethod]
        public void Test_Try03_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try03(false, false, false));
            AssertGasConsumed(1047870);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(false, false, true));
            AssertGasConsumed(1048530);
            Assert.AreEqual(new BigInteger(3), Contract.Try03(true, true, false));
            AssertGasConsumed(1079100);
            Assert.AreEqual(new BigInteger(4), Contract.Try03(true, true, true));
            AssertGasConsumed(1079760);
        }

        [TestMethod]
        public void Test_TryNest_AllPaths()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, false, false));
            AssertGasConsumed(1048800);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, false, false, false));
            AssertGasConsumed(1080030);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, true, false, false));
            AssertGasConsumed(1048800);
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, true, true));
            AssertGasConsumed(1079820);
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, true, true, true));
            AssertGasConsumed(1141890);
        }

        [TestMethod]
        public void Test_ThrowInCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(false, false, true));
            AssertGasConsumed(1048050);
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(true, false, true));
            AssertGasConsumed(1063920);
            Assert.ThrowsException<TestException>(() => Contract.ThrowInCatch(true, true, true));
            AssertGasConsumed(1079250);
        }

        [TestMethod]
        public void Test_TryFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinally(false, false));
            AssertGasConsumed(1047840);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinally(false, true));
            AssertGasConsumed(1048500);
            Assert.ThrowsException<TestException>(() => Contract.TryFinally(true, true));
            AssertGasConsumed(1063920);
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinallyAndRethrow(false, false));
            AssertGasConsumed(1047840);
            Assert.AreEqual(new BigInteger(3), Contract.TryFinallyAndRethrow(false, true));
            AssertGasConsumed(1048500);
            Assert.ThrowsException<TestException>(() => Contract.TryFinallyAndRethrow(true, true));
            AssertGasConsumed(1079280);
        }

        [TestMethod]
        public void Test_TryCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryCatch(false, false));
            AssertGasConsumed(1047600);
            Assert.AreEqual(new BigInteger(3), Contract.TryCatch(true, true));
            AssertGasConsumed(1079400);
        }

        [TestMethod]
        public void Test_TryWithTwoFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(1), Contract.TryWithTwoFinally(false, false, false, false, false, false));
            AssertGasConsumed(1049130);
            Assert.AreEqual(new BigInteger(4), Contract.TryWithTwoFinally(false, false, false, false, true, false));
            AssertGasConsumed(1049820);
            Assert.AreEqual(new BigInteger(6), Contract.TryWithTwoFinally(false, false, false, false, false, true));
            AssertGasConsumed(1049820);
            Assert.AreEqual(new BigInteger(3), Contract.TryWithTwoFinally(true, false, true, false, false, false));
            AssertGasConsumed(1065600);
            Assert.AreEqual(new BigInteger(10), Contract.TryWithTwoFinally(false, true, false, true, false, true));
            AssertGasConsumed(1066290);
            Assert.AreEqual(new BigInteger(15), Contract.TryWithTwoFinally(true, true, true, true, true, true));
            AssertGasConsumed(1083450);
        }

        [TestMethod]
        public void Test_TryECPointCast_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryecpointCast(false, false, false));
            AssertGasConsumed(1048620);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(false, false, true));
            AssertGasConsumed(1049280);
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(true, true, false));
            AssertGasConsumed(1064250);
            Assert.AreEqual(new BigInteger(4), Contract.TryecpointCast(true, true, true));
            AssertGasConsumed(1064910);
        }

        [TestMethod]
        public void Test_TryValidByteString2Ecpoint_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(false, false));
            AssertGasConsumed(1048470);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(false, true));
            AssertGasConsumed(1049130);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(true, false));
            AssertGasConsumed(1048470);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(true, true));
            AssertGasConsumed(1049130);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt160(false, false, false));
            AssertGasConsumed(1048620);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(false, false, true));
            AssertGasConsumed(1049280);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(true, true, false));
            AssertGasConsumed(1064250);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt160(true, true, true));
            AssertGasConsumed(1064910);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(false, false));
            AssertGasConsumed(1048470);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(false, true));
            AssertGasConsumed(1049130);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(true, false));
            AssertGasConsumed(1048470);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(true, true));
            AssertGasConsumed(1049130);
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt256(false, false, false));
            AssertGasConsumed(1048170);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(false, false, true));
            AssertGasConsumed(1048830);
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(true, true, false));
            AssertGasConsumed(1064190);
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt256(true, true, true));
            AssertGasConsumed(1064850);
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(false, false));
            AssertGasConsumed(1048020);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(false, true));
            AssertGasConsumed(1048680);
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(true, false));
            AssertGasConsumed(1048020);
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(true, true));
            AssertGasConsumed(1048680);
        }

        [TestMethod]
        public void Test_TryNULL2Ecpoint_1_AllPaths()
        {
            var result = Contract.TryNULL2Ecpoint_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1363680);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1365090);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1364340);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint160_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint160_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1363680);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint160_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1365090);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint160_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1364340);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint256_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint256_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1363680);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint256_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1365090);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint256_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1364340);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Bytestring_1_AllPaths()
        {
            var result = Contract.TryNULL2Bytestring_1(false, false, false);
            Assert.IsNotNull(result);
            AssertGasConsumed(1110000);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(true, false, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1111410);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(false, true, true);
            Assert.IsNotNull(result);
            AssertGasConsumed(1110660);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryUncatchableException_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryUncatchableException(false, false, false));
            AssertGasConsumed(1047870);
            Assert.AreEqual(new BigInteger(3), Contract.TryUncatchableException(false, false, true));
            AssertGasConsumed(1048530);
            Assert.ThrowsException<TestException>(() => Contract.TryUncatchableException(true, true, true));
            AssertGasConsumed(1047450);
        }

        [TestMethod]
        public void Test_ThrowCall()
        {
            Assert.ThrowsException<TestException>(Contract.ThrowCall);
        }
    }
}
