using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Helper : SmartContract
    {
        [InitialValue("0a0b0c0d0E0F", ContractParameterType.ByteArray)]
        private static readonly byte[] data = default;

        [InitialValue("NL1JGjDe22U44R57ZXVSeTYFBavEkVmkgF", ContractParameterType.Hash160)]
        private static readonly byte[] hashResult = default;

        public static byte[] TestHexToBytes()
        {
            return data;
        }

        public static int AssertCall(bool value)
        {
            ExecutionEngine.Assert(value == true);
            return 5;
        }

        public static BigInteger TestToBigInteger(byte[] data)
        {
            return new BigInteger(data);
        }

        public static BigInteger TestBigIntegerCast(byte[] data)
        {
            return (BigInteger)(ByteString)data;
        }

        public static BigInteger TestBigIntegerParseHexString(string data)
        {
            return BigInteger.Parse(data);
        }

        public static void VoidAssertCall(bool value)
        {
            ExecutionEngine.Assert(value == true);
        }

        public static byte[] TestByteToByteArray()
        {
            byte a = 0x01;
            byte[] result = a.ToByteArray();
            return result;
        }

        public static byte[] testReverse()
        {
            var result = (new byte[] { 0x01, 0x02, 0x03 }).Reverse();
            return result;
        }

        public static byte[] testSbyteToByteArray()
        {
            sbyte a = -1;
            var result = a.ToByteArray();
            return result;
        }

        public static byte[] testStringToByteArray()
        {
            string a = "hello world";
            var result = a.ToByteArray();
            return result;
        }

        public static byte[] testConcat()
        {
            var a = new byte[] { 0x01, 0x02, 0x03 };
            var b = new byte[] { 0x04, 0x05, 0x06 };
            var result = a.Concat(b);
            return result;
        }

        public static byte[] testRange()
        {
            var a = new byte[] { 0x01, 0x02, 0x03 };
            var result = a.Range(1, 1);
            return result;
        }

        public static byte[] testTake()
        {
            var a = new byte[] { 0x01, 0x02, 0x03 };
            var result = a.Take(2);
            return result;
        }

        public static byte[] testLast()
        {
            var a = new byte[] { 0x01, 0x02, 0x03 };
            var result = a.Last(2);
            return result;
        }

        public static byte[] testToScriptHash()
        {
            return hashResult;
        }
    }
}
