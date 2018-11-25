namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Account
    {
        public extern byte[] ScriptHash
        {
            [Syscall("Neo.Account.GetScriptHash")]
            get;
        }

        public extern byte[][] Votes
        {
            [Syscall("Neo.Account.GetVotes")]
            get;
        }

        [Syscall("Neo.Account.GetBalance")]
        public extern long GetBalance(byte[] asset_id);
        
        [Syscall("Neo.Account.IsStandard")]
        public static extern bool IsStandard(byte[] scripthash);
    }
}
