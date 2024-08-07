using Neo.SmartContract.Framework.Attributes;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Types_BigInteger : SmartContract.Framework.SmartContract
    {
        [Integer("100000000000000000000000000")]
        private static readonly BigInteger publicBigInteger = default!;

        public static BigInteger Attribute() { return publicBigInteger; }
        public static BigInteger Zero() { return BigInteger.Zero; }
        public static BigInteger One() { return BigInteger.One; }
        public static BigInteger MinusOne() { return BigInteger.MinusOne; }
        public static BigInteger Parse(string value) { return BigInteger.Parse(value); }
        public static BigInteger ConvertFromChar() { return (BigInteger)'A'; }
    }
}
