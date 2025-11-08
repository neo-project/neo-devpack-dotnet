using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirLeave : MirTerminator
{
    internal MirLeave(MirTry scope, MirBlock target)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }

    internal MirTry Scope { get; }
    internal MirBlock Target { get; }
}
