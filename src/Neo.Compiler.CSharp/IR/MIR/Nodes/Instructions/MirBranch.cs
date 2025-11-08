using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirBranch : MirTerminator
{
    internal MirBranch(MirBlock target)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }

    internal MirBlock Target { get; }
}
