using System.Collections.Generic;
using Neo.Compiler.HIR;

namespace Neo.Compiler.HIR.Optimization;

internal sealed class HirUsageGraph
{
    public HirUsageGraph(
        Dictionary<HirValue, int> useCounts,
        Dictionary<HirInst, HirBlock> instructionBlocks,
        Dictionary<HirPhi, HirBlock> phiBlocks)
    {
        UseCounts = useCounts;
        InstructionBlocks = instructionBlocks;
        PhiBlocks = phiBlocks;
    }

    public Dictionary<HirValue, int> UseCounts { get; }
    public Dictionary<HirInst, HirBlock> InstructionBlocks { get; }
    public Dictionary<HirPhi, HirBlock> PhiBlocks { get; }
}

