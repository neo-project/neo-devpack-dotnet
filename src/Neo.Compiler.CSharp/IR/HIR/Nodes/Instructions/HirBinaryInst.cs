using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal abstract class HirBinaryInst : HirInst
{
    protected HirBinaryInst(HirValue left, HirValue right, HirType type)
        : base(type)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public HirValue Left { get; set; }
    public HirValue Right { get; set; }
}
