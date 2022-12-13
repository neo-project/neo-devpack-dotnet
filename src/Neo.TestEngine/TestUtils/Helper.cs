using Neo.VM;
using Neo.SmartContract;

namespace Neo.TestingEngine
{
    internal class TestHelper
    {
        public static UInt160 GetContractHash(uint nefCheckSum, string name)
        {
            using var sb = new ScriptBuilder();
            sb.Emit(OpCode.ABORT);
            sb.EmitPush(Engine.Instance.Sender);
            sb.EmitPush(nefCheckSum);
            sb.EmitPush(name);

            return sb.ToArray().ToScriptHash();
        }
    }
}
