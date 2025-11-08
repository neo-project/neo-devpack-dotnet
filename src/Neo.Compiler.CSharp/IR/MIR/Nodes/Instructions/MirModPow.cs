using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirModPow : MirInst
{
    internal MirModPow(MirValue value, MirValue exponent, MirValue modulus, MirType resultType)
        : base(resultType)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Exponent = exponent ?? throw new ArgumentNullException(nameof(exponent));
        Modulus = modulus ?? throw new ArgumentNullException(nameof(modulus));
    }

    internal MirValue Value { get; }
    internal MirValue Exponent { get; }
    internal MirValue Modulus { get; }
}
