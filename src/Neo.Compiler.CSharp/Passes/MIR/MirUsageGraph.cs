using System.Collections.Generic;

namespace Neo.Compiler.MIR.Optimization;

internal sealed class MirUsageGraph
{
    internal MirUsageGraph(
        Dictionary<MirValue, int> useCounts,
        Dictionary<MirInst, MirBlock> instBlocks,
        Dictionary<MirPhi, MirBlock> phiBlocks)
    {
        UseCounts = useCounts;
        InstructionBlocks = instBlocks;
        PhiBlocks = phiBlocks;
    }

    internal Dictionary<MirValue, int> UseCounts { get; }
    internal Dictionary<MirInst, MirBlock> InstructionBlocks { get; }
    internal Dictionary<MirPhi, MirBlock> PhiBlocks { get; }
}

