using System.Collections.Generic;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Iterator<TKey, TValue> : IApiInterface
    {
        [Syscall("System.Iterator.Create")]
        public static extern Iterator<TKey, TValue> Create(Map<TKey, TValue> entry);

        [Syscall("System.Iterator.Create")]
        public static extern Iterator<TKey, TValue> Create(IEnumerable<TValue> entry);

        [Syscall("System.Iterator.Concat")]
        public extern Iterator<TKey, TValue> Concat(Iterator<TKey, TValue> value);

        [Syscall("System.Enumerator.Next")]
        public extern bool Next();

        public extern TKey Key
        {
            [Syscall("System.Iterator.Key")]
            get;
        }

        public extern TValue Value
        {
            [Syscall("System.Enumerator.Value")]
            get;
        }

        public extern Enumerator<TKey> Keys
        {
            [Syscall("System.Iterator.Keys")]
            get;
        }

        public extern Enumerator<TValue> Values
        {
            [Syscall("System.Iterator.Values")]
            get;
        }
    }
}
