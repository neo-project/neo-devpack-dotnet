using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VBufferSet : VNode
{
    internal VBufferSet(VNode buffer, VNode index, VNode value)
        : base(buffer.Type)
    {
        Buffer = buffer;
        Index = index;
        Value = value;
    }

    internal VNode Buffer { get; }
    internal VNode Index { get; }
    internal VNode Value { get; }
}
