using System.Collections.Generic;
using Neo.Compiler.LIR;

namespace Neo.Compiler.LIR.Backend;

internal sealed partial class StackScheduler
{
    private sealed record TryScopeInfo(VBlock TryBlock, VBlock FinallyBlock, VBlock MergeBlock, IReadOnlyList<VBlock> CatchBlocks);
}
