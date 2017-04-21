namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public class Account
    {
        public extern byte[] ScriptHash
        {
            [Syscall("AntShares.Account.GetScriptHash")]
            get;
        }

        public extern byte[][] Votes
        {
            [Syscall("AntShares.Account.GetVotes")]
            get;
        }

        [Syscall("AntShares.Account.GetBalance")]
        public extern long GetBalance(byte[] asset_id);
    }
}
