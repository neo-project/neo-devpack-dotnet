using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirConstInt : MirInst
{
    internal MirConstInt(BigInteger value, MirIntType? hintedType = null)
        : base(hintedType ?? MirType.TInt)
    {
        Value = value;
    }

    internal BigInteger Value { get; }
}
