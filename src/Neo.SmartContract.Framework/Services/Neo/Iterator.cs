namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Iterator
    {
        [Syscall("System.Iterator.Create")]
        public static extern Iterator<T> Create<T>(T[] array);

        [Syscall("System.Iterator.Create")]
        public static extern Iterator<(TKey, TValue)> Create<TKey, TValue>(Map<TKey, TValue> map);

        [Syscall("System.Iterator.Create")]
        public static extern Iterator<byte> Create(byte[] buffer);

        [Syscall("System.Iterator.Create")]
        public static extern Iterator<byte> Create(ByteString buffer);

        [Syscall("System.Iterator.Next")]
        public extern bool Next();
    }

    public class Iterator<T> : Iterator, IApiInterface
    {
        public extern T Value
        {
            [Syscall("System.Iterator.Value")]
            get;
        }
    }
}
