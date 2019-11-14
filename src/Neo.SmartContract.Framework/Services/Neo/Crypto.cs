namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Crypto
    {

        [Syscall("Neo.Crypto.ECDsaVerify")]
        public extern static bool ECDsaVerify(byte[] message, byte[] pubkey, byte[] signature);

        [Syscall("Neo.Crypto.ECDsaCheckMultiSig")]
        public extern static bool ECDsaCheckMultiSig(byte[] message, byte[][] pubkey, byte[][] signature);
    }
}
