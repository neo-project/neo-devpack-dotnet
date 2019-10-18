namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Native
    {
        [Syscall("Neo.Native.Tokens.NEO")]
        public static extern Contract NEO(string method, object[] arguments);

        [Syscall("Neo.Native.Tokens.GAS")]
        public static extern Contract GAS(string method, object[] arguments);

        [Syscall("Neo.Native.Policy")]
        public static extern Contract Policy(string method, object[] arguments);
    }
}
