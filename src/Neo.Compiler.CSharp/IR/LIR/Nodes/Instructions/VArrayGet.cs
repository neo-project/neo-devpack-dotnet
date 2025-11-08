using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VArrayGet : VNode
{
    internal VArrayGet(VNode array, VNode index, LirType elementType)
        : base(elementType)
    {
        Array = array;
        Index = index;
    }

    internal VNode Array { get; }
    internal VNode Index { get; }
}
