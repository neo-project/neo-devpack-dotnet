using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Fee : SmartContract
    {
        public static BigInteger TestGAS(ulong amount)
        {
            return Fee.GAS(amount);
        }

        public static BigInteger TestSatoshi(ulong amount)
        {
            return Fee.Satoshi(amount);
        }

        public static BigInteger TestkSatoshi(ulong amount)
        {
            return Fee.kSatoshi(amount);
        }

        public static BigInteger TestmSatoshi(ulong amount)
        {
            return Fee.mSatoshi(amount);
        }
    }
}
