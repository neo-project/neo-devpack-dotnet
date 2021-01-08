using System.Collections.Generic;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Iterator<TKey, TValue> : IApiInterface
    {
        [Syscall("System.Iterator.Create")]
        public static extern Iterator<TKey, TValue> Create(Map<TKey, TValue> entry);

        [Syscall("System.Iterator.Create")]
        public static extern Iterator<TKey, TValue> Create(IEnumerable<TValue> entry);

        [Syscall("System.Iterator.Next")]
        public extern bool Next();

        public extern TValue Value
        {
            [Syscall("System.Iterator.Value")]
            get;
        }
    }
}
