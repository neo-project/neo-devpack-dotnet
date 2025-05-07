using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.IntegrationTests.TestContracts
{
    [DisplayName("IntegerOverflowContract")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "A contract with integer overflow vulnerabilities for testing")]
    public class IntegerOverflowContract : Framework.SmartContract
    {
        // Vulnerable to integer overflow
        public static BigInteger Add(BigInteger a, BigInteger b)
        {
            return a + b; // No overflow check
        }

        // Vulnerable to integer overflow
        public static BigInteger Multiply(BigInteger a, BigInteger b)
        {
            return a * b; // No overflow check
        }

        // Vulnerable to integer underflow
        public static BigInteger Subtract(BigInteger a, BigInteger b)
        {
            return a - b; // No underflow check
        }

        // Safe version with overflow check
        public static BigInteger SafeAdd(BigInteger a, BigInteger b)
        {
            BigInteger result = a + b;
            // Check for overflow
            if ((a > 0 && b > 0 && result < 0) || (a < 0 && b < 0 && result > 0))
                throw new Exception("Integer overflow");
            return result;
        }

        // Safe version with overflow check
        public static BigInteger SafeMultiply(BigInteger a, BigInteger b)
        {
            if (a == 0 || b == 0)
                return 0;
                
            BigInteger result = a * b;
            // Check for overflow
            if (result / a != b)
                throw new Exception("Integer overflow");
            return result;
        }

        // Safe version with underflow check
        public static BigInteger SafeSubtract(BigInteger a, BigInteger b)
        {
            BigInteger result = a - b;
            // Check for underflow
            if ((a > 0 && b < 0 && result < 0) || (a < 0 && b > 0 && result > 0))
                throw new Exception("Integer underflow");
            return result;
        }
    }
}
