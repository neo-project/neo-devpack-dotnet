// Copyright (C) 2015-2025 The Neo Project.
//
// List.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
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

        [OpCode(OpCode.POPITEM)]
        public extern T PopItem();

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
