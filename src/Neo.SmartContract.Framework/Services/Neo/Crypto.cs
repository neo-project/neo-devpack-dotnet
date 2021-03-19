namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Crypto
    {
        [Syscall("Neo.Crypto.CheckSig")]
        public extern static bool CheckSig(Cryptography.ECC.ECPoint pubkey, ByteString signature);

        [Syscall("Neo.Crypto.CheckMultisig")]
        public extern static bool CheckMultisig(Cryptography.ECC.ECPoint[] pubkey, ByteString[] signature);

        public static ByteString Hash160(ByteString value)
        {
            return CryptoLib.ripemd160(CryptoLib.Sha256(value));
        }

        public static ByteString Hash256(ByteString value)
        {
            return CryptoLib.Sha256(CryptoLib.Sha256(value));
        }
    }
}
