using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_AllowOverflow : SmartContract.Framework.SmartContract
    {
        public static byte ArrayElement(BigInteger i)
        {
            int[] arr = [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07];
            return (byte)arr[(int)i];
        }

        public static BigInteger ShiftRight(BigInteger b, BigInteger e) => b >> (int)e;
    }
}
