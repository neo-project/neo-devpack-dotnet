using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirFinally : MirInst
{
    internal MirFinally(MirTry parent)
        : base(MirType.TVoid)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    internal MirTry Parent { get; }
}
