using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VSetItem : VNode
{
    internal VSetItem(VNode obj, VNode keyOrIndex, VNode value)
        : base(obj.Type)
    {
        Object = obj;
        KeyOrIndex = keyOrIndex;
        Value = value;
    }

    internal VNode Object { get; }
    internal VNode KeyOrIndex { get; }
    internal VNode Value { get; }
}
