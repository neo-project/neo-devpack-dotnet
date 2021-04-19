using System;
using System.Collections;
using System.Collections.Generic;

namespace Neo.SmartContract.Framework.Services
{
    public class Iterator : IApiInterface, IEnumerable
    {
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
