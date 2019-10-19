namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Native
    {
        [Syscall("Neo.Native.Tokens.NEO")]
        public static extern object NEO(string method, object[] arguments);

        [Syscall("Neo.Native.Tokens.GAS")]
        public static extern object GAS(string method, object[] arguments);

        [Syscall("Neo.Native.Policy")]
        public static extern object Policy(string method, object[] arguments);
    }
}
