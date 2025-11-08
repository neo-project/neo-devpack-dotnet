using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VMapSet : VNode
{
    internal VMapSet(VNode map, VNode key, VNode value)
        : base(map.Type)
    {
        Map = map;
        Key = key;
        Value = value;
    }

    internal VNode Map { get; }
    internal VNode Key { get; }
    internal VNode Value { get; }
}
