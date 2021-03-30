using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_BigInteger : SmartContract.Framework.SmartContract
    {
        public static object TestPow(BigInteger x, int y)
        {
            return BigInteger.Pow(x, y);
        }

        public static object TestSqrt(BigInteger x)
        {
            return x.Sqrt();
        }
    }
}
