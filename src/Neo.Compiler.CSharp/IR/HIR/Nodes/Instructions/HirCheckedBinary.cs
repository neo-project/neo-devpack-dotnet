using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirCheckedBinary : HirInst
{
    public HirCheckedBinary(HirCheckedOp op, HirValue left, HirValue right, HirType type, HirFailPolicy policy)
        : base(type)
    {
        Operation = op;
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
        Policy = policy;
    }

    public HirCheckedOp Operation { get; }
    public HirValue Left { get; set; }
    public HirValue Right { get; set; }
    public HirFailPolicy Policy { get; }
}
