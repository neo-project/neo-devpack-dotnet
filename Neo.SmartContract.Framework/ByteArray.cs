using Neo.VM;
using System;
using System.Numerics;

namespace Neo.SmartContract.Framework
{
    public abstract class ByteArray
    {
        public abstract byte this[int index]
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
