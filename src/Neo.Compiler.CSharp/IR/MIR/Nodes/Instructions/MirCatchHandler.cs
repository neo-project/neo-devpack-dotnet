using System;

namespace Neo.Compiler.MIR;

internal sealed class MirCatchHandler
{
    internal MirCatchHandler(MirBlock block)
    {
        Block = block ?? throw new ArgumentNullException(nameof(block));
    }

    internal MirBlock Block { get; }
}
