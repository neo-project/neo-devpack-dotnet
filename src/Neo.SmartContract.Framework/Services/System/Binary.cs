using System.Numerics;

namespace Neo.SmartContract.Framework.Services.System
{
    public class Binary
    {
        [Syscall("System.Binary.Base64Decode")]
        public static extern ByteString Base64Decode(string input);

        [Syscall("System.Binary.Base64Encode")]
        public static extern string Base64Encode(ByteString input);

        [Syscall("System.Binary.Base58Decode")]
        public static extern ByteString Base58Decode(string input);

        [Syscall("System.Binary.Base58Encode")]
        public static extern string Base58Encode(ByteString input);

        [Syscall("System.Binary.Itoa")]
        public static extern string Itoa(BigInteger value, int @base = 10);

        [Syscall("System.Binary.Itoa")]
        public static extern string Itoa(int value, int @base = 10);

        [Syscall("System.Binary.Itoa")]
        public static extern string Itoa(uint value, int @base = 10);

        [Syscall("System.Binary.Itoa")]
        public static extern string Itoa(long value, int @base = 10);

        [Syscall("System.Binary.Itoa")]
        public static extern string Itoa(ulong value, int @base = 10);

        [Syscall("System.Binary.Itoa")]
        public static extern string Itoa(short value, int @base = 10);

        [Syscall("System.Binary.Itoa")]
        public static extern string Itoa(ushort value, int @base = 10);

        [Syscall("System.Binary.Itoa")]
        public static extern string Itoa(byte value, int @base = 10);

        [Syscall("System.Binary.Itoa")]
        public static extern string Itoa(sbyte value, int @base = 10);

        [Syscall("System.Binary.Atoi")]
        public static extern BigInteger Atoi(string value, int @base = 10);
    }
}
