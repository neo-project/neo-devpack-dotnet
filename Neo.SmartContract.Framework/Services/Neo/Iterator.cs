namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Iterator<TKey, TValue>
    {
        [Syscall("Neo.Iterator.Next")]
        public extern bool Next();

        public extern TKey Key
        {
            [Syscall("Neo.Iterator.Key")]
            get;
        }

        public extern TValue Value
        {
            [Syscall("Neo.Iterator.Value")]
            get;
        }
    }
}
