using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VAdd : VBinary
{
    internal VAdd(VNode left, VNode right)
        : base(VBinaryOp.Add, left, right, LirType.TInt)
    {
    }
}
