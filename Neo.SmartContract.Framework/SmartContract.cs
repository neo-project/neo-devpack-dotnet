using Neo.VM;

namespace Neo.SmartContract.Framework
{
    public class SmartContract
    {
        [OpCode(OpCode.SHA1)]
        protected extern static byte[] Sha1(byte[] data);

        [OpCode(OpCode.SHA256)]
        protected extern static byte[] Sha256(byte[] data);

        [OpCode(OpCode.HASH160)]
        protected extern static byte[] Hash160(byte[] data);

        [OpCode(OpCode.HASH256)]
        protected extern static byte[] Hash256(byte[] data);

        [OpCode(OpCode.CHECKSIG)]
        protected extern static bool VerifySignature(byte[] signature, byte[] pubkey);

        [OpCode(OpCode.CHECKMULTISIG)]
        protected extern static bool VerifySignatures(byte[][] signature, byte[][] pubkey);

        [OpCode(OpCode.DYNAMICCALL)]
        public extern static object DynamicAppCall(byte[] script_hash, params object[] parameters);
    }
}
