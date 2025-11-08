using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal class VBinary : VNode
{
    internal VBinary(VBinaryOp op, VNode left, VNode right, LirType type)
        : base(type)
    {
        Op = op;
        Left = left;
        Right = right;
    }

    internal VBinaryOp Op { get; }
    internal VNode Left { get; }
    internal VNode Right { get; }
}
