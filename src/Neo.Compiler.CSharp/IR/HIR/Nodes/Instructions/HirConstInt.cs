using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirConstInt : HirInst
{
    public HirConstInt(BigInteger value, HirIntType? hintedType = null)
        : base(hintedType ?? HirType.IntType)
    {
        Value = value;
    }

    public BigInteger Value { get; }
}
