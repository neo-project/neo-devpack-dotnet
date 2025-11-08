using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VConcat : VNode
{
    internal VConcat(VNode left, VNode right)
        : base(LirType.TByteString)
    {
        Left = left;
        Right = right;
    }

    internal VNode Left { get; }
    internal VNode Right { get; }
}
