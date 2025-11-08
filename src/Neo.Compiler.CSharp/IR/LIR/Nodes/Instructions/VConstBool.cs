using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VConstBool : VNode
{
    internal VConstBool(bool value)
        : base(LirType.TBool)
    {
        Value = value;
    }

    internal bool Value { get; }
}
