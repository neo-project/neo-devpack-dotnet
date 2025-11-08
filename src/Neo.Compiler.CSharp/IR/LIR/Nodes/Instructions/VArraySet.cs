using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VArraySet : VNode
{
    internal VArraySet(VNode array, VNode index, VNode value)
        : base(array.Type)
    {
        Array = array;
        Index = index;
        Value = value;
    }

    internal VNode Array { get; }
    internal VNode Index { get; }
    internal VNode Value { get; }
}
