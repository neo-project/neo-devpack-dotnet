using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirConstBool : HirInst
{
    public HirConstBool(bool value)
        : base(HirType.BoolType)
    {
        Value = value;
    }

    public bool Value { get; }
}
