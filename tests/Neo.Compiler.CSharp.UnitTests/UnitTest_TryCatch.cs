// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_TryCatch.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

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
            Assert.AreEqual(new BigInteger(3), Contract.Try01(false, false, true));
            Assert.AreEqual(new BigInteger(3), Contract.Try01(true, true, false));
            Assert.AreEqual(new BigInteger(4), Contract.Try01(true, true, true));
        }

        [TestMethod]
        public void Test_Try02_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try02(false, false, false));
            Assert.AreEqual(new BigInteger(3), Contract.Try02(false, false, true));
            Assert.AreEqual(new BigInteger(3), Contract.Try02(true, true, false));
            Assert.AreEqual(new BigInteger(4), Contract.Try02(true, true, true));
        }

        [TestMethod]
        public void Test_Try03_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.Try03(false, false, false));
            Assert.AreEqual(new BigInteger(3), Contract.Try03(false, false, true));
            Assert.AreEqual(new BigInteger(3), Contract.Try03(true, true, false));
            Assert.AreEqual(new BigInteger(4), Contract.Try03(true, true, true));
        }

        [TestMethod]
        public void Test_TryNest_AllPaths()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, false, false));
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, false, false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, true, false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryNest(false, false, true, true));
            Assert.AreEqual(new BigInteger(4), Contract.TryNest(true, true, true, true));
        }

        [TestMethod]
        public void Test_ThrowInCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(false, false, true));
            Assert.AreEqual(new BigInteger(4), Contract.ThrowInCatch(true, false, true));
            Assert.ThrowsExactly<TestException>(() => Contract.ThrowInCatch(true, true, true));
        }

        [TestMethod]
        public void Test_TryFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinally(false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryFinally(false, true));
            Assert.ThrowsExactly<TestException>(() => Contract.TryFinally(true, true));
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryFinallyAndRethrow(false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryFinallyAndRethrow(false, true));
            Assert.ThrowsExactly<TestException>(() => Contract.TryFinallyAndRethrow(true, true));
        }

        [TestMethod]
        public void Test_TryCatch_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryCatch(false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryCatch(true, true));
        }

        [TestMethod]
        public void Test_TryWithTwoFinally_AllPaths()
        {
            Assert.AreEqual(new BigInteger(1), Contract.TryWithTwoFinally(false, false, false, false, false, false));
            Assert.AreEqual(new BigInteger(4), Contract.TryWithTwoFinally(false, false, false, false, true, false));
            Assert.AreEqual(new BigInteger(6), Contract.TryWithTwoFinally(false, false, false, false, false, true));
            Assert.AreEqual(new BigInteger(3), Contract.TryWithTwoFinally(true, false, true, false, false, false));
            Assert.AreEqual(new BigInteger(10), Contract.TryWithTwoFinally(false, true, false, true, false, true));
            Assert.AreEqual(new BigInteger(15), Contract.TryWithTwoFinally(true, true, true, true, true, true));
        }

        [TestMethod]
        public void Test_TryECPointCast_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryecpointCast(false, false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(false, false, true));
            Assert.AreEqual(new BigInteger(3), Contract.TryecpointCast(true, true, false));
            Assert.AreEqual(new BigInteger(4), Contract.TryecpointCast(true, true, true));
        }

        [TestMethod]
        public void Test_TryValidByteString2Ecpoint_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(false, true));
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteString2Ecpoint(true, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint(true, true));
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt160(false, false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(false, false, true));
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt160(true, true, false));
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt160(true, true, true));
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt160_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(false, true));
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt160(true, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160(true, true));
        }

        [TestMethod]
        public void Test_TryInvalidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryinvalidByteArray2UInt256(false, false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(false, false, true));
            Assert.AreEqual(new BigInteger(3), Contract.TryinvalidByteArray2UInt256(true, true, false));
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt256(true, true, true));
        }

        [TestMethod]
        public void Test_TryValidByteArray2UInt256_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(false, true));
            Assert.AreEqual(new BigInteger(2), Contract.TryvalidByteArray2UInt256(true, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256(true, true));
        }

        [TestMethod]
        public void Test_TryNULL2Ecpoint_1_AllPaths()
        {
            var result = Contract.TryNULL2Ecpoint_1(false, false, false);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(true, false, true);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Ecpoint_1(false, true, true);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint160_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint160_1(false, false, false);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint160_1(true, false, true);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint160_1(false, true, true);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Uint256_1_AllPaths()
        {
            var result = Contract.TryNULL2Uint256_1(false, false, false);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Uint256_1(true, false, true);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Uint256_1(false, true, true);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryNULL2Bytestring_1_AllPaths()
        {
            var result = Contract.TryNULL2Bytestring_1(false, false, false);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(2), result[0]);
            Assert.IsNotNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(true, false, true);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(4), result[0]);
            Assert.IsNull(result[1]);

            result = Contract.TryNULL2Bytestring_1(false, true, true);
            Assert.IsNotNull(result);
            Assert.AreEqual(new BigInteger(3), result[0]);
            Assert.IsNotNull(result[1]);
        }

        [TestMethod]
        public void Test_TryUncatchableException_AllPaths()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TryUncatchableException(false, false, false));
            Assert.AreEqual(new BigInteger(3), Contract.TryUncatchableException(false, false, true));
            Assert.ThrowsExactly<TestException>(() => Contract.TryUncatchableException(true, true, true));
        }

        [TestMethod]
        public void Test_ThrowCall()
        {
            Assert.ThrowsExactly<TestException>(Contract.ThrowCall);
        }

        [TestMethod]
        public void Test_CatchExceptionType()
        {
            string? result = Contract.CatchExceptionType();
            Assert.AreEqual("NoExceptionexception", result);
        }

        [TestMethod]
        public void Test_CatchException()
        {
            string? result = Contract.CatchException();
            Assert.AreEqual("ExceptionExample", result);
        }

        [TestMethod]
        public void Test_CatchAndFinallyAfterHelperThrow_AllPaths()
        {
            Assert.AreEqual(new BigInteger(107), Contract.CatchAndFinallyAfterHelperThrow(0));
            Assert.AreEqual(new BigInteger(105), Contract.CatchAndFinallyAfterHelperThrow(5));
        }
    }
}
