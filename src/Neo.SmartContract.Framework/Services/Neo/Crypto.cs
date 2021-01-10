namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Crypto
    {
        [Syscall("Neo.Crypto.SHA256")]
        public extern static ByteString SHA256(ByteString value);

        [Syscall("Neo.Crypto.RIPEMD160")]
        public extern static ByteString RIPEMD160(ByteString value);

        [Syscall("Neo.Crypto.SHA256")]
        [Syscall("Neo.Crypto.RIPEMD160")]
        public extern static ByteString Hash160(ByteString value);

        [Syscall("Neo.Crypto.SHA256")]
        [Syscall("Neo.Crypto.SHA256")]
        public extern static ByteString Hash256(ByteString value);

        public static class ECDsa
        {
            public static class Secp256r1
            {
                [Syscall("Neo.Crypto.VerifyWithECDsaSecp256r1")]
                public extern static bool Verify(ByteString message, Cryptography.ECC.ECPoint pubkey, ByteString signature);

                [Syscall("Neo.Crypto.CheckMultisigWithECDsaSecp256r1")]
                public extern static bool CheckMultiSig(ByteString message, Cryptography.ECC.ECPoint[] pubkey, ByteString[] signature);
            }

            public static class Secp256k1
            {
                [Syscall("Neo.Crypto.VerifyWithECDsaSecp256k1")]
                public extern static bool Verify(ByteString message, Cryptography.ECC.ECPoint pubkey, ByteString signature);

                [Syscall("Neo.Crypto.CheckMultisigWithECDsaSecp256k1")]
                public extern static bool CheckMultiSig(ByteString message, Cryptography.ECC.ECPoint[] pubkey, ByteString[] signature);
            }
        }
    }
}
