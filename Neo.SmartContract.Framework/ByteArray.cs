using Neo.VM;
using System;
using System.Numerics;

namespace Neo.SmartContract.Framework
{
    public struct ByteArray
    {
        public extern byte this[int index]
        {
            get;
            set;
        }

        [OpCode]
        public extern static implicit operator ByteArray(byte[] source);

        [OpCode]
        public extern static implicit operator byte[](ByteArray source);
    }
}
