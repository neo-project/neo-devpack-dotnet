namespace Neo.SmartContract.Framework
{
    public class SmartContract
    {
        [OpCode(OpCode.SHA1)]
        protected extern static byte[] Sha1(byte[] data);

        [OpCode(OpCode.SHA256)]
        protected extern static byte[] Sha256(byte[] data);

        [Syscall("Neo.Crypto.Hash160")]
        protected extern static byte[] Hash160(byte[] data);

        [Syscall("Neo.Crypto.Hash256")]
        protected extern static byte[] Hash256(byte[] data);

        [Syscall("Neo.Crypto.CheckSig")]
        protected extern static bool VerifySignature(byte[] signature, byte[] pubkey);

        [Syscall("System.Crypto.Verify")]
        protected extern static bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey);

        [Syscall("Neo.Crypto.CheckMultiSig")]
        protected extern static bool VerifySignatures(byte[][] signature, byte[][] pubkey);
    }
}
