using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VStructGet : VNode
{
    internal VStructGet(VNode obj, int index, LirType type)
        : base(type)
    {
        Object = obj;
        Index = index;
    }

    internal VNode Object { get; }
    internal int Index { get; }
}
