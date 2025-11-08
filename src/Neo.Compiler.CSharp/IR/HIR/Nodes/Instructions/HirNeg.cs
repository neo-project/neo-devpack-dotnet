using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirNeg : HirInst
{
    public HirNeg(HirValue operand, HirType type)
        : base(type)
    {
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    public HirValue Operand { get; set; }
}
