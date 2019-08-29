namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Notification : IScriptContainer
    {
        public byte[] ScriptHash { get; }
        public object State { get; }
    }
}
