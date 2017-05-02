using AntShares.SmartContract.Framework.Services.System;
using AntShares.VM;

namespace AntShares.SmartContract.Framework
{
    public class Contract
    {
        protected static ScriptEngine ScriptEngine { get; } = new ScriptEngine();

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
    }
}
