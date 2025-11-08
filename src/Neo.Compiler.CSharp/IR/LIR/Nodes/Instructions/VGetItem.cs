using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VGetItem : VNode
{
    internal VGetItem(VNode obj, VNode keyOrIndex, LirType type)
        : base(type)
    {
        Object = obj;
        KeyOrIndex = keyOrIndex;
    }

    internal VNode Object { get; }
    internal VNode KeyOrIndex { get; }
}
