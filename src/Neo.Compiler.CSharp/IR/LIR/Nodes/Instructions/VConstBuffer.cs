using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VConstBuffer : VNode
{
    internal VConstBuffer(byte[] value)
        : base(LirType.TBuffer)
    {
        Value = value;
    }

    internal byte[] Value { get; }
}
