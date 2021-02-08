namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Crypto
    {
        [Syscall("Neo.Crypto.VerifyWithECDsaSecp256r1")]
        public extern static bool CheckSig(Cryptography.ECC.ECPoint pubkey, ByteString signature);

        [Syscall("Neo.Crypto.CheckMultisigWithECDsaSecp256r1")]
        public extern static bool CheckMultisig(Cryptography.ECC.ECPoint[] pubkey, ByteString[] signature);
    }
}
