namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class TransactionOutput : IApiInterface
    {
        public extern byte[] AssetId
        {
            [Syscall("AntShares.Output.GetAssetId")]
            get;
        }

        public extern long Value
        {
            [Syscall("AntShares.Output.GetValue")]
            get;
        }

        public extern byte[] ScriptHash
        {
            [Syscall("AntShares.Output.GetScriptHash")]
            get;
        }
    }
}
