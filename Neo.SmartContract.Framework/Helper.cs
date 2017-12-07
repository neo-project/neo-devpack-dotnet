using Neo.VM;
using System.Numerics;

namespace Neo.SmartContract.Framework
{
    public static class Helper
    {
        [Nonemit]
        public extern static BigInteger AsBigInteger(this byte[] source);

        [Nonemit]
        public extern static byte[] AsByteArray(this BigInteger source);

        [Nonemit]
        public extern static byte[] AsByteArray(this string source);

        [Nonemit]
        public extern static string AsString(this byte[] source);

        [OpCode(OpCode.CAT)]
        public extern static byte[] Concat(this byte[] first, byte[] second);

        [OpCode(OpCode.SUBSTR)]
        public extern static byte[] Range(this byte[] source, int index, int count);

        [OpCode(OpCode.LEFT)]
        public extern static byte[] Take(this byte[] source, int count);

        [NonemitWithConvert(ConvertMethod.HexString2Bytes)]
        public extern static byte[] HexString2Bytes(string hex);

        [NonemitWithConvert(ConvertMethod.AddressString2ScriptHashBytes)]
        public extern static byte[] AddressString2ScriptHashBytes(string address);
    }
}
