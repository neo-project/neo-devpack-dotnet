namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Crypto
    {
        [Syscall("Neo.Crypto.SHA256")]
        public extern static byte[] SHA256(byte[] value);

        public static class ECDsa
        {
            public static class Secp256r1
            {
                [Syscall("Neo.Crypto.ECDsa.Secp256r1.Verify")]
                public extern static bool Verify(byte[] message, byte[] pubkey, byte[] signature);

                [Syscall("Neo.Crypto.ECDsa.Secp256r1.CheckMultiSig")]
                public extern static bool CheckMultiSig(byte[] message, byte[][] pubkey, byte[][] signature);
            }

            public static class Secp256k1
            {
                [Syscall("Neo.Crypto.ECDsa.Secp256k1.Verify")]
                public extern static bool Verify(byte[] message, byte[] pubkey, byte[] signature);

                [Syscall("Neo.Crypto.ECDsa.Secp256k1.CheckMultiSig")]
                public extern static bool CheckMultiSig(byte[] message, byte[][] pubkey, byte[][] signature);
            }
        }
    }
}
