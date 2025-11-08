using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirTryFinallyScope : HirInst
{
    public HirTryFinallyScope(
        HirBlock tryBlock,
        HirBlock finallyBlock,
        HirBlock mergeBlock,
        IReadOnlyList<HirCatchClause>? catches = null)
        : base(HirType.VoidType)
    {
        TryBlock = tryBlock ?? throw new ArgumentNullException(nameof(tryBlock));
        FinallyBlock = finallyBlock ?? throw new ArgumentNullException(nameof(finallyBlock));
        MergeBlock = mergeBlock ?? throw new ArgumentNullException(nameof(mergeBlock));
        CatchHandlers = catches ?? Array.Empty<HirCatchClause>();
    }

    public HirBlock TryBlock { get; }
    public HirBlock FinallyBlock { get; }
    public HirBlock MergeBlock { get; }
    public IReadOnlyList<HirCatchClause> CatchHandlers { get; }
}

internal sealed class HirCatchClause
{
    public HirCatchClause(HirBlock block)
    {
        Block = block ?? throw new ArgumentNullException(nameof(block));
    }

    public HirBlock Block { get; }
}
