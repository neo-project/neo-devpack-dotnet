namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Crypto
    {
        [Syscall("Neo.Crypto.CheckSig")]
        public extern static bool CheckSig(Cryptography.ECC.ECPoint pubkey, ByteString signature);

        [Syscall("Neo.Crypto.CheckMultisig")]
        public extern static bool CheckMultisig(Cryptography.ECC.ECPoint[] pubkey, ByteString[] signature);
    }
}
