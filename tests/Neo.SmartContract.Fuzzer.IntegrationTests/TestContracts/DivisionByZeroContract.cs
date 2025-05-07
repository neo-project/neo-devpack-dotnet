using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.IntegrationTests.TestContracts
{
    [DisplayName("DivisionByZeroContract")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "A contract with division by zero vulnerabilities for testing")]
    public class DivisionByZeroContract : Framework.SmartContract
    {
        // Vulnerable to division by zero
        public static BigInteger Divide(BigInteger a, BigInteger b)
        {
            return a / b; // No zero check
        }

        // Vulnerable to division by zero
        public static BigInteger Modulo(BigInteger a, BigInteger b)
        {
            return a % b; // No zero check
        }

        // Vulnerable to division by zero in a complex expression
        public static BigInteger ComplexDivision(BigInteger a, BigInteger b, BigInteger c)
        {
            return a / (b - c); // No check if b - c is zero
        }

        // Safe version with zero check
        public static BigInteger SafeDivide(BigInteger a, BigInteger b)
        {
            if (b == 0)
                throw new Exception("Division by zero");
            return a / b;
        }

        // Safe version with zero check
        public static BigInteger SafeModulo(BigInteger a, BigInteger b)
        {
            if (b == 0)
                throw new Exception("Division by zero");
            return a % b;
        }

        // Safe version with zero check
        public static BigInteger SafeComplexDivision(BigInteger a, BigInteger b, BigInteger c)
        {
            BigInteger divisor = b - c;
            if (divisor == 0)
                throw new Exception("Division by zero");
            return a / divisor;
        }
    }
}
