using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirNot : HirInst
{
    public HirNot(HirValue operand)
        : base(HirType.BoolType)
    {
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    public HirValue Operand { get; set; }
}
