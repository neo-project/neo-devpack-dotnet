using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VConstByteString : VNode
{
    internal VConstByteString(byte[] value)
        : base(LirType.TByteString)
    {
        Value = value;
    }

    internal byte[] Value { get; }
}
