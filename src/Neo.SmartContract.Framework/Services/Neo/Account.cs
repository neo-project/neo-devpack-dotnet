namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Account
    {
        [Syscall("Neo.Account.IsStandard")]
        public static extern bool IsStandard(byte[] scripthash);
    }
}
