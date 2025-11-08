using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VConstInt : VNode
{
    internal VConstInt(BigInteger value)
        : base(LirType.TInt)
    {
        Value = value;
    }

    internal BigInteger Value { get; }
}
