namespace AntShares.SmartContract.Framework
{
    public class Contract
    {
        protected static ScriptEngine ScriptEngine { get; } = new ScriptEngine();

        [ScriptOp(ScriptOp.OP_CHECKSIG)]
        protected extern static bool VerifySignature(byte[] signature, byte[] pubkey);
    }
}
