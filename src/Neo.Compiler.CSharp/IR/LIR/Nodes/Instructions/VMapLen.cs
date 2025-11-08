using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VMapLen : VNode
{
    internal VMapLen(VNode map)
        : base(LirType.TInt)
    {
        Map = map;
    }

    internal VNode Map { get; }
}
