// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DivisionOverflow.cs file belongs to the neo project and is free
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
    /// <summary>
    /// Unit tests for division overflow detection in checked context.
    /// 
    /// Problem: Division overflow occurs when dividing the minimum value of a signed
    /// integer type by -1. For example:
    ///   - int.MinValue (-2147483648) / -1 = 2147483648, which exceeds int.MaxValue
    ///   - long.MinValue / -1 exceeds long.MaxValue
    /// 
    /// The compiler checks Int32 and Int64 for division overflow in checked context,
    /// throwing an exception when overflow would occur.
    /// 
    /// BigInteger has arbitrary precision, so no overflow check is needed.
    /// Unsigned types (uint, ulong) cannot have negative values, so no overflow possible.
    /// </summary>
    [TestClass]
    public class UnitTest_DivisionOverflow : DebugAndTestBase<Contract_DivisionOverflow>
    {
        #region Int32 Tests

        /// <summary>
        /// Test: int.MinValue / -1 in checked context should throw.
        /// This demonstrates the overflow problem with Int32.
        /// </summary>
        [TestMethod]
        public void Test_DivideCheckedInt32_Overflow_ShouldThrow()
        {
            Assert.ThrowsException<TestException>(() =>
            {
                Contract.DivideCheckedInt32(int.MinValue, -1);
            });
        }

        /// <summary>
        /// Test: Normal division in checked context should work.
        /// </summary>
        [TestMethod]
        public void Test_DivideCheckedInt32_Normal_ShouldWork()
        {
            var result = Contract.DivideCheckedInt32(100, 5);
            Assert.AreEqual(20, result);
        }

        /// <summary>
        /// Test: int.MinValue / -1 in unchecked context should NOT throw.
        /// Unchecked allows overflow without exception.
        /// </summary>
        [TestMethod]
        public void Test_DivideUncheckedInt32_Overflow_ShouldNotThrow()
        {
            // In unchecked context, overflow is allowed
            var result = Contract.DivideUncheckedInt32(int.MinValue, -1);
            // Result wraps around due to overflow
            Assert.IsNotNull(result);
        }

        #endregion

        #region Int64 Tests

        /// <summary>
        /// Test: long.MinValue / -1 in checked context should throw.
        /// This demonstrates the overflow problem with Int64.
        /// </summary>
        [TestMethod]
        public void Test_DivideCheckedInt64_Overflow_ShouldThrow()
        {
            Assert.ThrowsException<TestException>(() =>
            {
                Contract.DivideCheckedInt64(long.MinValue, -1);
            });
        }

        /// <summary>
        /// Test: Normal division in checked context should work.
        /// </summary>
        [TestMethod]
        public void Test_DivideCheckedInt64_Normal_ShouldWork()
        {
            var result = Contract.DivideCheckedInt64(1000L, 10L);
            Assert.AreEqual(100L, result);
        }

        /// <summary>
        /// Test: long.MinValue / -1 in unchecked context should NOT throw.
        /// </summary>
        [TestMethod]
        public void Test_DivideUncheckedInt64_Overflow_ShouldNotThrow()
        {
            var result = Contract.DivideUncheckedInt64(long.MinValue, -1);
            Assert.IsNotNull(result);
        }

        #endregion

        #region BigInteger Tests

        /// <summary>
        /// Test: BigInteger division never overflows (arbitrary precision).
        /// Even BigInteger.MinValue equivalent / -1 works fine.
        /// </summary>
        [TestMethod]
        public void Test_DivideCheckedBigInteger_NoOverflow()
        {
            // BigInteger has arbitrary precision, so no overflow is possible
            var minValue = new BigInteger(long.MinValue);
            var result = Contract.DivideCheckedBigInteger(minValue, -1);
            Assert.AreEqual(-minValue, result);
        }

        /// <summary>
        /// Test: Normal BigInteger division works.
        /// </summary>
        [TestMethod]
        public void Test_DivideCheckedBigInteger_Normal_ShouldWork()
        {
            var result = Contract.DivideCheckedBigInteger(1000, 10);
            Assert.AreEqual(new BigInteger(100), result);
        }

        #endregion
    }
}
