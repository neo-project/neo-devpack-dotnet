using System.Numerics;
using Neo.SmartContract.Framework.Services;
using System;

namespace Neo.SmartContract.Framework.TestContracts
{
    public class Contract_GuardHelpers_Inline : SmartContract
    {
        // Inline guard helper methods for testing
        private static void Require(bool condition, string errorCode)
        {
            if (!condition)
                throw new Exception(errorCode);
        }

        private static void Ensure(bool condition, string errorCode)
        {
            if (!condition)
                throw new Exception($"POST:{errorCode}");
        }

        private static void Revert(string errorCode)
        {
            throw new Exception(errorCode);
        }

        private static void RequireNotNull(object? value, string paramName)
        {
            if (value is null)
                throw new Exception($"NULL:{paramName}");
        }

        private static void RequireNonNegative(BigInteger amount)
        {
            if (amount < 0)
                throw new Exception("NEGATIVE");
        }

        private static void RequirePositive(BigInteger amount)
        {
            if (amount <= 0)
                throw new Exception("NOT_POSITIVE");
        }

        private static void RequireValidAddress(UInt160 address)
        {
            if (address is null || address == UInt160.Zero)
                throw new Exception("INVALID_ADDR");
        }

        private static void RequireWitness(UInt160 account, string errorCode = "NO_WITNESS")
        {
            if (!Runtime.CheckWitness(account))
                throw new Exception(errorCode);
        }

        private static void RequireInRange(BigInteger value, BigInteger min, BigInteger max)
        {
            if (value < min || value > max)
                throw new Exception("OUT_OF_RANGE");
        }

        private static void RequireEquals(object actual, object expected, string errorCode = "NOT_EQUAL")
        {
            if (!actual.Equals(expected))
                throw new Exception(errorCode);
        }

        private static void RequireCaller(UInt160 expectedCaller)
        {
            if (Runtime.CallingScriptHash != expectedCaller)
                throw new Exception("INVALID_CALLER");
        }

        private static void RequireNotEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new Exception($"EMPTY:{paramName}");
        }

        // Test methods
        public static void TestRequire(bool condition)
        {
            Require(condition, "FAILED");
        }

        public static void TestRequireNotNull(object? value)
        {
            RequireNotNull(value, "myParam");
        }

        public static void TestRequireNonNegative(BigInteger amount)
        {
            RequireNonNegative(amount);
        }

        public static void TestRequirePositive(BigInteger amount)
        {
            RequirePositive(amount);
        }

        public static void TestRequireValidAddress(UInt160 address)
        {
            RequireValidAddress(address);
        }

        public static void TestRequireWitness(UInt160 account)
        {
            RequireWitness(account, "NO_WITNESS");
        }

        public static void TestRequireWitnessCustom(UInt160 account, string errorCode)
        {
            RequireWitness(account, errorCode);
        }

        public static void TestRequireInRange(BigInteger value, BigInteger min, BigInteger max)
        {
            RequireInRange(value, min, max);
        }

        public static void TestRequireEquals(int actual, int expected)
        {
            RequireEquals(actual, expected, "NOT_EQUAL");
        }

        public static void TestRequireEqualsCustom(int actual, int expected, string errorCode)
        {
            RequireEquals(actual, expected, errorCode);
        }

        public static void TestRequireCaller(UInt160 expectedCaller)
        {
            RequireCaller(expectedCaller);
        }

        public static void TestRequireNotEmpty(string value)
        {
            RequireNotEmpty(value, "myString");
        }

        public static void TestEnsure(bool condition)
        {
            Ensure(condition, "POSTCOND");
        }

        public static void TestRevert()
        {
            Revert("REVERTED");
        }

        // Combined test for real-world scenario
        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
        {
            // Validate inputs
            RequireValidAddress(from);
            RequireValidAddress(to);
            RequirePositive(amount);
            RequireWitness(from, "NO_WITNESS");

            // Simulate transfer logic
            return true;
        }
    }
}
