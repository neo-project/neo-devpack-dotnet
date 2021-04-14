using System;
using System.Collections;
using System.Collections.Generic;

namespace Neo.SmartContract.Framework
{
    public class List<T> : IEnumerable<T>
    {
        [OpCode(OpCode.NEWARRAY0)]
        public extern List();

        public extern int Count
        {
            [OpCode(OpCode.SIZE)]
            get;
        }

        public extern T this[int key]
        {
            [OpCode(OpCode.PICKITEM)]
            get;
            [OpCode(OpCode.SETITEM)]
            set;
        }

        [OpCode(OpCode.APPEND)]
        public extern void Add(T item);

        [OpCode(OpCode.REMOVE)]
        public extern void RemoveAt(int index);

        [OpCode(OpCode.CLEARITEMS)]
        public extern void Clear();

        [OpCode(OpCode.VALUES)]
        public extern List<T> Clone();

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        [OpCode(OpCode.NOP)]
        public static extern implicit operator List<T>(T[] array);

        [OpCode(OpCode.NOP)]
        public static extern implicit operator T[](List<T> array);
    }
}
