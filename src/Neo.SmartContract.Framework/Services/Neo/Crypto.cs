namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Crypto
    {
        [Syscall("Neo.Crypto.CheckSig")]
        public extern static bool VerifySignature(byte[] pubkey, byte[] signature);

        [Syscall("System.Crypto.Verify")]
        public extern static bool VerifySignature(byte[] message, byte[] pubkey, byte[] signature);

        [Syscall("Neo.Crypto.CheckMultiSig")]
        public extern static bool VerifySignatures(byte[][] pubkey, byte[][] signature);
    }
}
