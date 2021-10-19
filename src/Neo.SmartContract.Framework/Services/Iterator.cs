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
