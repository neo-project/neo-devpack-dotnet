using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VCheckedArithmetic : VNode
{
    internal VCheckedArithmetic(VCheckedOp op, VNode left, VNode right, LirType type, VGuardFailKind failKind, VBlock? failTarget)
        : base(type)
    {
        Op = op;
        Left = left;
        Right = right;
        FailKind = failKind;
        FailTarget = failTarget;
    }

    internal VCheckedOp Op { get; }
    internal VNode Left { get; }
    internal VNode Right { get; }
    internal VGuardFailKind FailKind { get; }
    internal VBlock? FailTarget { get; }
}
