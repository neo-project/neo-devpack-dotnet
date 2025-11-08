using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirCompare : HirInst
{
    public HirCompare(HirCmpKind kind, HirValue left, HirValue right, bool unsigned = false)
        : base(HirType.BoolType)
    {
        Kind = kind;
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
        Unsigned = unsigned;
    }

    public HirCmpKind Kind { get; }
    public bool Unsigned { get; }
    public HirValue Left { get; set; }
    public HirValue Right { get; set; }
}
