using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirConvert : HirInst
{
    public HirConvert(HirConvKind kind, HirValue value, HirType targetType)
        : base(targetType)
    {
        Kind = kind;
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public HirConvKind Kind { get; }
    public HirValue Value { get; set; }
}
