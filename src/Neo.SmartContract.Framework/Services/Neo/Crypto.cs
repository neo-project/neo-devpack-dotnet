namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Crypto
    {
        [Syscall("Neo.Crypto.SHA256")]
        public extern static byte[] SHA256(byte[] value);

        [Syscall("Neo.Crypto.RIPEMD160")]
        public extern static byte[] RIPEMD160(byte[] value);

        [Syscall("Neo.Crypto.SHA256")]
        [Syscall("Neo.Crypto.RIPEMD160")]
        public extern static byte[] Hash160(byte[] value);

        [Syscall("Neo.Crypto.SHA256")]
        [Syscall("Neo.Crypto.SHA256")]
        public extern static byte[] Hash256(byte[] value);

        public static class ECDsa
        {
            public static class Secp256r1
            {
                [Syscall("Neo.Crypto.VerifyWithECDsaSecp256r1")]
                public extern static bool Verify(byte[] message, Cryptography.ECC.ECPoint pubkey, byte[] signature);

                [Syscall("Neo.Crypto.CheckMultisigWithECDsaSecp256r1")]
                public extern static bool CheckMultiSig(byte[] message, Cryptography.ECC.ECPoint[] pubkey, byte[][] signature);
            }

            public static class Secp256k1
            {
                [Syscall("Neo.Crypto.VerifyWithECDsaSecp256k1")]
                public extern static bool Verify(byte[] message, Cryptography.ECC.ECPoint pubkey, byte[] signature);

                [Syscall("Neo.Crypto.CheckMultisigWithECDsaSecp256k1")]
                public extern static bool CheckMultiSig(byte[] message, Cryptography.ECC.ECPoint[] pubkey, byte[][] signature);
            }
        }
    }
}
