using System;
using System.Collections.Generic;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirTry : MirInst
{
    internal MirTry(MirBlock tryBlock, MirBlock finallyBlock, MirBlock mergeBlock, IReadOnlyList<MirCatchHandler> catchHandlers)
        : base(MirType.TVoid)
    {
        TryBlock = tryBlock ?? throw new ArgumentNullException(nameof(tryBlock));
        FinallyBlock = finallyBlock ?? throw new ArgumentNullException(nameof(finallyBlock));
        MergeBlock = mergeBlock ?? throw new ArgumentNullException(nameof(mergeBlock));
        CatchHandlers = catchHandlers ?? throw new ArgumentNullException(nameof(catchHandlers));
    }

    internal MirBlock TryBlock { get; }
    internal MirBlock FinallyBlock { get; }
    internal MirBlock MergeBlock { get; }
    internal IReadOnlyList<MirCatchHandler> CatchHandlers { get; }
}
