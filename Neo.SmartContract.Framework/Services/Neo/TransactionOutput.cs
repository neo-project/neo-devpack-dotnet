namespace Neo.SmartContract.Framework.Services.Neo
{
    public class TransactionOutput : IApiInterface
    {
        public extern byte[] AssetId
        {
            [Syscall("Neo.Output.GetAssetId")]
            get;
        }

        public extern long Value
        {
            [Syscall("Neo.Output.GetValue")]
            get;
        }

        public extern byte[] ScriptHash
        {
            [Syscall("Neo.Output.GetScriptHash")]
            get;
        }
    }
}
