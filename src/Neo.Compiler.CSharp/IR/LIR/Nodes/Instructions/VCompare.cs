using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VCompare : VNode
{
    internal VCompare(VCompareOp op, bool unsigned, VNode left, VNode right)
        : base(LirType.TBool)
    {
        Op = op;
        Unsigned = unsigned;
        Left = left;
        Right = right;
    }

    internal VCompareOp Op { get; }
    internal bool Unsigned { get; }
    internal VNode Left { get; }
    internal VNode Right { get; }
}
