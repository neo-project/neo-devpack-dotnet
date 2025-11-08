using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirConcat : HirInst
{
    public HirConcat(HirValue left, HirValue right)
        : base(HirType.ByteStringType)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public HirValue Left { get; set; }
    public HirValue Right { get; set; }
}
