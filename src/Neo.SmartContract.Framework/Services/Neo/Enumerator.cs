using System.Collections.Generic;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Enumerator<TValue> : IApiInterface
    {
        [Syscall("System.Enumerator.Create")]
        public static extern Enumerator<TValue> Create(IEnumerable<TValue> entry);

        [Syscall("System.Enumerator.Concat")]
        public extern Enumerator<TValue> Concat(Enumerator<TValue> value);

        [Syscall("System.Enumerator.Next")]
        public extern bool Next();

        public extern TValue Value
        {
            [Syscall("System.Enumerator.Value")]
            get;
        }
    }
}
