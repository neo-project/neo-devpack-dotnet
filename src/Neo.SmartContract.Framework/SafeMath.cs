// Copyright (C) 2015-2026 The Neo Project.
//
// SafeMath.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.SmartContract.Framework
{
    /// <summary>
    /// Provides unsigned-style arithmetic helpers for BigInteger that fault on
    /// negative inputs, underflow, and division/modulo by zero.
    /// In NEO, any overflow or underflow will cause the execution to abort whether or not the SafeMath methods are used.
    /// </summary>
    public static class SafeMath
    {
        /// <summary>
        /// Adds two non-negative BigIntegers and returns the result.
        /// Faults if any operand is negative or the result overflows.
        /// </summary>
        public static BigInteger UnsignedAdd(BigInteger left, BigInteger right)
        {
            ExecutionEngine.Assert(left >= 0 && right >= 0, "negative values are not supported");
            return left + right;
        }

        /// <summary>
        /// Subtracts a non-negative BigInteger from another non-negative BigInteger and returns the result.
        /// Faults if any operand is negative or the result is negative.
        /// </summary>
        public static BigInteger UnsignedSub(BigInteger left, BigInteger right)
        {
            ExecutionEngine.Assert(left >= 0 && right >= 0, "negative values are not supported");
            ExecutionEngine.Assert(left >= right, "result would be negative");
            return left - right;
        }

        /// <summary>
        /// Multiplies two non-negative BigIntegers and returns the result.
        /// Faults if any operand is negative or the result overflows.
        /// </summary>
        public static BigInteger UnsignedMul(BigInteger left, BigInteger right)
        {
            ExecutionEngine.Assert(left >= 0 && right >= 0, "negative values are not supported");
            return left * right;
        }

        /// <summary>
        /// Divides a non-negative BigInteger by another non-negative BigInteger and returns the result.
        /// Faults if the dividend is negative or the divisor is non-positive.
        /// </summary>
        public static BigInteger UnsignedDiv(BigInteger dividend, BigInteger divisor)
        {
            ExecutionEngine.Assert(dividend >= 0, "the dividend must be non-negative");
            ExecutionEngine.Assert(divisor > 0, "the divisor must be positive");
            return dividend / divisor;
        }

        /// <summary>
        /// Returns the remainder of the division of a non-negative BigInteger by another non-negative BigInteger.
        /// Faults if the dividend is negative or the divisor is non-positive.
        /// </summary>
        public static BigInteger UnsignedMod(BigInteger dividend, BigInteger divisor)
        {
            ExecutionEngine.Assert(dividend >= 0, "the dividend must be non-negative");
            ExecutionEngine.Assert(divisor > 0, "the divisor must be positive");
            return dividend % divisor;
        }
    }
}
