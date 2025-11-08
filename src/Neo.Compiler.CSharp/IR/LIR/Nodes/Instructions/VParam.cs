using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VParam : VNode
{
    internal VParam(int index, LirType type)
        : base(type)
    {
        Index = index;
    }

    internal int Index { get; }
}
