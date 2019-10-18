using System.Collections.Generic;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Iterator<TKey, TValue>
    {
        [Syscall("Neo.Iterator.Create")]
        public static extern Iterator<TKey, TValue> Create(IDictionary<TKey, TValue> entry);

        [Syscall("Neo.Iterator.Create")]
        public static extern Iterator<TKey, TValue> Create(IEnumerable<TValue> entry);

        [Syscall("Neo.Iterator.Concat")]
        public extern Iterator<TKey, TValue> Concat(Iterator<TKey, TValue> value);

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

        public extern Enumerator<TKey> Keys
        {
            [Syscall("Neo.Iterator.Keys")]
            get;
        }

        public extern Enumerator<TValue> Values
        {
            [Syscall("Neo.Iterator.Values")]
            get;
        }
    }
}
