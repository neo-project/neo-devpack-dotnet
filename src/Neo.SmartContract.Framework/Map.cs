// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework
{
    public class Map<TKey, TValue>
    {
        [OpCode(OpCode.NEWMAP)]
        public extern Map();

        public extern int Count
        {
            [OpCode(OpCode.SIZE)]
            get;
        }

        public extern TValue this[TKey key]
        {
            [OpCode(OpCode.PICKITEM)]
            get;
            [OpCode(OpCode.SETITEM)]
            set;
        }

        [OpCode(OpCode.CLEARITEMS)]
        public extern void Clear();

        public extern TKey[] Keys
        {
            [OpCode(OpCode.KEYS)]
            get;
        }

        public extern TValue[] Values
        {
            [OpCode(OpCode.VALUES)]
            get;
        }

        [OpCode(OpCode.HASKEY)]
        public extern bool HasKey(TKey key);

        [OpCode(OpCode.REMOVE)]
        public extern void Remove(TKey key);
    }
}
