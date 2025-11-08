using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VCompareBranch : VTerminator
{
    internal VCompareBranch(VCompareOp op, bool unsigned, VNode left, VNode right, VBlock trueTarget, VBlock falseTarget)
    {
        Op = op;
        Unsigned = unsigned;
        Left = left;
        Right = right;
        TrueTarget = trueTarget;
        FalseTarget = falseTarget;
    }

    internal VCompareOp Op { get; }
    internal bool Unsigned { get; }
    internal VNode Left { get; }
    internal VNode Right { get; }
    internal VBlock TrueTarget { get; }
    internal VBlock FalseTarget { get; }
}
