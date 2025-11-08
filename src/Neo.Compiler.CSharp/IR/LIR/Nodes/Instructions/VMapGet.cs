using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VMapGet : VNode
{
    internal VMapGet(VNode map, VNode key, LirType valueType)
        : base(valueType)
    {
        Map = map;
        Key = key;
    }

    internal VNode Map { get; }
    internal VNode Key { get; }
}
