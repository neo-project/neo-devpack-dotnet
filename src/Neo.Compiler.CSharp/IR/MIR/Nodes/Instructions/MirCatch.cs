using System;

namespace Neo.Compiler.MIR;

internal sealed class MirCatch : MirInst
{
    internal MirCatch(MirTry parent, MirCatchHandler handler)
        : base(MirType.TVoid)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    internal MirTry Parent { get; }
    internal MirCatchHandler Handler { get; }
}
