using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirModMul : MirInst
{
    internal MirModMul(MirValue left, MirValue right, MirValue modulus, MirType resultType)
        : base(resultType)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
        Modulus = modulus ?? throw new ArgumentNullException(nameof(modulus));
    }

    internal MirValue Left { get; }
    internal MirValue Right { get; }
    internal MirValue Modulus { get; }
}
