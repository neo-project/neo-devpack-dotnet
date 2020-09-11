namespace Neo.SmartContract.Framework.Services.System
{
    public class Binary
    {
        [Syscall("System.Binary.Base64Decode")]
        public static extern byte[] Base64Decode(string input);

        [Syscall("System.Binary.Base64Encode")]
        public static extern string Base64Encode(byte[] input);

        [Syscall("System.Binary.Base58Decode")]
        public static extern byte[] Base58Decode(string input);

        [Syscall("System.Binary.Base58Encode")]
        public static extern string Base58Encode(byte[] input);
    }
}
