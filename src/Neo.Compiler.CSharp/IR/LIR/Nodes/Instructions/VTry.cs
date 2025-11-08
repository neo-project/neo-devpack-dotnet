using System.Collections.Generic;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VTry : VNode
{
    internal VTry(VBlock tryBlock, VBlock finallyBlock, VBlock mergeBlock, IReadOnlyList<VBlock> catchBlocks)
        : base(LirType.TVoid)
    {
        TryBlock = tryBlock;
        FinallyBlock = finallyBlock;
        MergeBlock = mergeBlock;
        CatchBlocks = catchBlocks;
    }

    internal VBlock TryBlock { get; }
    internal VBlock FinallyBlock { get; }
    internal VBlock MergeBlock { get; }
    internal IReadOnlyList<VBlock> CatchBlocks { get; }
}
