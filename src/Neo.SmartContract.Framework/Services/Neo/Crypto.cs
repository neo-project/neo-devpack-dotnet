namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Crypto
    {
        [Syscall("Neo.Crypto.SHA256")]
        public extern static byte[] SHA256(byte[] value);

        [Syscall("Neo.Crypto.ECDsa.Secp256r1.Verify")]
        public extern static bool ECDsaSecp256r1Verify(byte[] message, byte[] pubkey, byte[] signature);

        [Syscall("Neo.Crypto.ECDsa.Secp256k1.Verify")]
        public extern static bool ECDsaSecp256k1Verify(byte[] message, byte[] pubkey, byte[] signature);

        [Syscall("Neo.Crypto.ECDsa.Secp256r1.CheckMultiSig")]
        public extern static bool ECDsaSecp256r1CheckMultiSig(byte[] message, byte[][] pubkey, byte[][] signature);

        [Syscall("Neo.Crypto.ECDsa.Secp256k1.CheckMultiSig")]
        public extern static bool ECDsaSecp256k1CheckMultiSig(byte[] message, byte[][] pubkey, byte[][] signature);
    }
}
