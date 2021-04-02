using System;
using System.Collections;
using System.Collections.Generic;

namespace Neo.SmartContract.Framework.Services
{
    public class Iterator : IApiInterface, IEnumerable
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

        public extern object Value
        {
            [Syscall("System.Iterator.Value")]
            get;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class Iterator<T> : Iterator, IEnumerable<T>
    {
        public extern new T Value
        {
            [Syscall("System.Iterator.Value")]
            get;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
