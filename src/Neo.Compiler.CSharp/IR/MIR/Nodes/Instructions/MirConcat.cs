using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirConcat : MirInst
{
    internal MirConcat(MirValue left, MirValue right)
        : base(MirType.TByteString)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    internal MirValue Left { get; }
    internal MirValue Right { get; }
}
