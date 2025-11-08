using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirCondBranch : MirTerminator
{
    internal MirCondBranch(MirValue condition, MirBlock trueTarget, MirBlock falseTarget)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        TrueTarget = trueTarget ?? throw new ArgumentNullException(nameof(trueTarget));
        FalseTarget = falseTarget ?? throw new ArgumentNullException(nameof(falseTarget));
    }

    internal MirValue Condition { get; }
    internal MirBlock TrueTarget { get; }
    internal MirBlock FalseTarget { get; }
}
