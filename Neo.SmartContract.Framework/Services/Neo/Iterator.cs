namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Iterator
    {
        [Syscall("Neo.Iterator.Next")]
        public extern Iterator Next();

        public extern byte[] Key
        {
            [Syscall("Neo.Iterator.Key")]
            get;
        }

        public extern byte[] Value
        {
            [Syscall("Neo.Iterator.Value")]
            get;
        }
    }
}
