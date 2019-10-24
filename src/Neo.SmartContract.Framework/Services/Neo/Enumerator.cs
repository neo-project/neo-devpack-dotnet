using System.Collections.Generic;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Enumerator<TValue>
    {
        [Syscall("Neo.Enumerator.Create")]
        public static extern Enumerator<TValue> Create(IEnumerable<TValue> entry);

        [Syscall("Neo.Enumerator.Concat")]
        public extern Enumerator<TValue> Concat(Enumerator<TValue> value);

        [Syscall("Neo.Enumerator.Next")]
        public extern bool Next();

        public extern TValue Value
        {
            [Syscall("Neo.Enumerator.Value")]
            get;
        }
    }
}
