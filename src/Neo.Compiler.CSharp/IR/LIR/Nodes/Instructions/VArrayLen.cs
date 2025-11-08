using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VArrayLen : VNode
{
    internal VArrayLen(VNode array)
        : base(LirType.TInt)
    {
        Array = array;
    }

    internal VNode Array { get; }
}
