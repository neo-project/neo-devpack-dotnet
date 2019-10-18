namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Cryto
    {
        [Syscall("Neo.Crypto.CheckSig")]
        public extern static bool VerifySignature(byte[] signature, byte[] pubkey);

        [Syscall("System.Crypto.Verify")]
        public extern static bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey);

        [Syscall("Neo.Crypto.CheckMultiSig")]
        public extern static bool VerifySignatures(byte[][] signature, byte[][] pubkey);
    }
}
