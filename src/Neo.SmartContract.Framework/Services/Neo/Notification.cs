namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Notification
    {
        public byte[] ScriptHash
        {
            [OpCode(OpCode.PUSH0)]
            [OpCode(OpCode.PICKITEM)]
            get;
        }

        public object State
        {
            [OpCode(OpCode.PUSH1)]
            [OpCode(OpCode.PICKITEM)]
            get;
        }
    }
}
