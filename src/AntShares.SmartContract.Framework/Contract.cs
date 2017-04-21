using AntShares.VM;

namespace AntShares.SmartContract.Framework
{
    public class Contract
    {
        protected static ScriptEngine ScriptEngine { get; } = new ScriptEngine();

        [OpCode(OpCode.CHECKSIG)]
        protected extern static bool VerifySignature(byte[] signature, byte[] pubkey);
    }
}
