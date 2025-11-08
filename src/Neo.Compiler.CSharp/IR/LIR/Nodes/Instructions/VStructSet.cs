using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VStructSet : VNode
{
    internal VStructSet(VNode obj, int index, VNode value, LirStructType type)
        : base(type)
    {
        Object = obj;
        Index = index;
        Value = value;
    }

    internal VNode Object { get; }
    internal int Index { get; }
    internal VNode Value { get; }
}
