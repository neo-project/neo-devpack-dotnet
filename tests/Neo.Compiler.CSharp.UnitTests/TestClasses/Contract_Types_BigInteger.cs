using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Types_BigInteger : SmartContract.Framework.SmartContract
    {
        public static BigInteger Zero() { return BigInteger.Zero; }
        public static BigInteger One() { return BigInteger.One; }
        public static BigInteger MinusOne() { return BigInteger.MinusOne; }
        public static BigInteger Parse(string value) { return BigInteger.Parse(value); }
    }
}
