using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VMapDelete : VNode
{
    internal VMapDelete(VNode map, VNode key)
        : base(map.Type)
    {
        Map = map;
        Key = key;
    }

    internal VNode Map { get; }
    internal VNode Key { get; }
}
