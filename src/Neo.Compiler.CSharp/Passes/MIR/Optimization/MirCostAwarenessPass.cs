using System;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Computes coarse instruction and gas estimates for MIR functions, feeding release-mode heuristics.
/// </summary>
internal sealed class MirCostAwarenessPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var instructionCount = 0;
        var effectfulInstructionCount = 0;
        ulong estimatedGas = 0;

        foreach (var block in function.Blocks)
        {
            foreach (var inst in block.Instructions)
            {
                instructionCount++;
                if (inst.Effect != MirEffect.None)
                    effectfulInstructionCount++;

                estimatedGas += EstimateInstructionCost(inst);
            }

            if (block.Terminator is not null)
            {
                instructionCount++;
                estimatedGas += 1;
            }
        }

        var summary = new MirCostSummary(instructionCount, effectfulInstructionCount, estimatedGas);
        if (!summary.Equals(function.CostSummary))
        {
            function.SetCostSummary(summary);
            return true;
        }

        return false;
    }

    private static ulong EstimateInstructionCost(MirInst inst)
    {
        switch (inst)
        {
            case MirSyscall syscall:
                return syscall.GasHint ?? 10UL;
            case MirCall call when !call.IsPure:
                return 5;
            case MirPointerCall pointerCall when !pointerCall.IsPure:
                return 5;
            default:
                return inst.Effect == MirEffect.None ? 1UL : 2UL;
        }
    }
}
