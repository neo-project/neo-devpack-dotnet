namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Iterator<TKey, TValue>
    {
        [Syscall("Neo.Enumerator.Next")]
        public extern bool Next();

        public extern TKey Key
        {
            [Syscall("Neo.Iterator.Key")]
            get;
        }

        public extern TValue Value
        {
            [Syscall("Neo.Enumerator.Value")]
            get;
        }
    }
}
