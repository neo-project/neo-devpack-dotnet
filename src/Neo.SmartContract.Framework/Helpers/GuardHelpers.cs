// Copyright (C) 2015-2025 The Neo Project.
//
// GuardHelpers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace Neo.SmartContract.Framework.Helpers
{
    /// <summary>
    /// Provides guard utilities for contract preconditions and postconditions
    /// </summary>
    public static class GuardHelpers
    {
        /// <summary>
        /// Requires that a condition is true, otherwise throws with the specified error code
        /// </summary>
        /// <param name="condition">The condition to check</param>
        /// <param name="errorCode">Short error code to reduce GAS costs</param>
        public static void Require(bool condition, string errorCode)
        {
            if (!condition)
                throw new Exception(errorCode);
        }

        /// <summary>
        /// Ensures that a postcondition is true, otherwise throws with the specified error code
        /// </summary>
        /// <param name="condition">The postcondition to check</param>
        /// <param name="errorCode">Short error code to reduce GAS costs</param>
        public static void Ensure(bool condition, string errorCode)
        {
            if (!condition)
                throw new Exception($"POST:{errorCode}");
        }

        /// <summary>
        /// Reverts the transaction with the specified error code
        /// </summary>
        /// <param name="errorCode">Short error code to reduce GAS costs</param>
        public static void Revert(string errorCode)
        {
            throw new Exception(errorCode);
        }

        /// <summary>
        /// Requires that a value is not null
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="paramName">The parameter name for error reporting</param>
        public static void RequireNotNull(object? value, string paramName)
        {
            if (value is null)
                throw new Exception($"NULL:{paramName}");
        }

        /// <summary>
        /// Requires that an amount is non-negative
        /// </summary>
        /// <param name="amount">The amount to check</param>
        public static void RequireNonNegative(BigInteger amount)
        {
            if (amount < 0)
                throw new Exception("Negative");
        }

        /// <summary>
        /// Requires that an amount is positive (greater than zero)
        /// </summary>
        /// <param name="amount">The amount to check</param>
        public static void RequirePositive(BigInteger amount)
        {
            if (amount <= 0)
                throw new Exception("NOT_POSITIVE");
        }

        /// <summary>
        /// Requires that a UInt160 address is valid (not zero)
        /// </summary>
        /// <param name="address">The address to check</param>
        public static void RequireValidAddress(UInt160 address)
        {
            if (address is null || address == UInt160.Zero)
                throw new Exception("INVALID_ADDR");
        }

        /// <summary>
        /// Requires that the caller has witness for the specified account
        /// </summary>
        /// <param name="account">The account to check witness for</param>
        /// <param name="errorCode">Optional custom error code</param>
        public static void RequireWitness(UInt160 account, string errorCode = "NO_WITNESS")
        {
            if (!Runtime.CheckWitness(account))
                throw new Exception(errorCode);
        }

        /// <summary>
        /// Requires that a value is within the specified range (inclusive)
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="min">Minimum allowed value (inclusive)</param>
        /// <param name="max">Maximum allowed value (inclusive)</param>
        public static void RequireInRange(BigInteger value, BigInteger min, BigInteger max)
        {
            if (value < min || value > max)
                throw new Exception("OUT_OF_RANGE");
        }

        /// <summary>
        /// Requires that two values are equal
        /// </summary>
        /// <param name="actual">The actual value</param>
        /// <param name="expected">The expected value</param>
        /// <param name="errorCode">Optional custom error code</param>
        public static void RequireEquals(object actual, object expected, string errorCode = "NOT_EQUAL")
        {
            if (!actual.Equals(expected))
                throw new Exception(errorCode);
        }

        /// <summary>
        /// Requires that the calling script hash matches the expected contract
        /// </summary>
        /// <param name="expectedCaller">The expected calling script hash</param>
        public static void RequireCaller(UInt160 expectedCaller)
        {
            if (Runtime.CallingScriptHash != expectedCaller)
                throw new Exception("INVALID_CALLER");
        }

        /// <summary>
        /// Requires that a string is not null or empty
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <param name="paramName">The parameter name for error reporting</param>
        public static void RequireNotEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new Exception($"EMPTY:{paramName}");
        }
    }
}
