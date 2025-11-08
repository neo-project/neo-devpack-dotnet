using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VModMul : VNode
{
    internal VModMul(VNode left, VNode right, VNode modulus, LirType type)
        : base(type)
    {
        Left = left;
        Right = right;
        Modulus = modulus;
    }

    internal VNode Left { get; }
    internal VNode Right { get; }
    internal VNode Modulus { get; }
}
