using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VMapHas : VNode
{
    internal VMapHas(VNode map, VNode key)
        : base(LirType.TBool)
    {
        Map = map;
        Key = key;
    }

    internal VNode Map { get; }
    internal VNode Key { get; }
}
