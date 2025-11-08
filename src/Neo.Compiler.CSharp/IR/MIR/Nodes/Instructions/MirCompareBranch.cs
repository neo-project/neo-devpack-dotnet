using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirCompareBranch : MirTerminator
{
    internal MirCompareBranch(
        MirCompare.Op operation,
        MirValue left,
        MirValue right,
        MirBlock trueTarget,
        MirBlock falseTarget,
        bool unsigned = false)
    {
        Operation = operation;
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
        TrueTarget = trueTarget ?? throw new ArgumentNullException(nameof(trueTarget));
        FalseTarget = falseTarget ?? throw new ArgumentNullException(nameof(falseTarget));
        Unsigned = unsigned;
    }

    internal MirCompare.Op Operation { get; }
    internal MirValue Left { get; }
    internal MirValue Right { get; }
    internal MirBlock TrueTarget { get; }
    internal MirBlock FalseTarget { get; }
    internal bool Unsigned { get; }
}
